using System.ComponentModel;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Standby
    }

    [Header("References")]
    [SerializeField] private Transform player;
    private NavMeshAgent agent;
    private Animator anim;


    [SerializeField] private float pathCalculationFrequency = 0.5f;
    public float distanceToPlayer;

    private Vector3 lastPlayerPosition;
    private float lastDestinationCalculation;
    private LayerMask losMask;
    private Transform crumb;
    [SerializeField]
    private LayerMask breadcrumbMask;



    [Header("Patrol")]
    [SerializeField] private float patrolRadius = 5f;
    private Vector3 startPosition;
    private Vector3 patrolPosition;



    [Header("Chase")]
    [Range(5, 25)]
    [SerializeField] public float chaseRange = 12f;

    

    [Header("Standby")]         //STANDBY variables
    [SerializeField]
    private float maxStandbyReachTime = 4f;   //Reccomended 4f 

    private float standbyTimer;

    [Range(2f, 6f)]
    private float standbyRange = 4f;

    private Vector3 standbyPos;

    [SerializeField]
    private float standbyTriggerRange = 2.5f; //Añadimos una variable para que no necesite entrar en rango de ataque para entrar en standby evitando bloqueos entre enemigos

    private bool reachedStandbyPosition;




    [Header("Attack")]
    [SerializeField]
    [Range(0.5f, 5f)]
    private float meleeRange = 0.75f;

    [SerializeField]
    private float attackRange;
    public float AttackRange => attackRange;  //Añadimos esta variable publica para que en combat el enemigo ataque correctamente

    [SerializeField]
    private float minimumAttackTime = 1.1f;     //Añadimos variables para controlar el tiempo minimo que el enemigo está en estado de ataque

    private float attackTimer;

    private bool canExitAttack;

    public EnemyState state;




    [Header("Idle")]
    [SerializeField] private Vector2 idleDelay = new Vector2(3f, 8f);
    private float idleCounter;              //IDLE variables
    private float idleTimeOut;



  



    private NavMeshObstacle obstacle;    //ToggleAgent variables
    private bool isWaitingToEnableAgent;




    public int priorityBonus = 0;   // No funcionalidad implementada por el momento





    private void OnValidate()
    {
        chaseRange = Mathf.Round(chaseRange);
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        obstacle = GetComponent<NavMeshObstacle>();

        startPosition = transform.position;

        losMask = ~LayerMask.GetMask("Player", "Enemy", "Breadcrumb");

        state = EnemyState.Idle;

        idleTimeOut = Random.Range(idleDelay.x, idleDelay.y);

        idleCounter = idleTimeOut;

        agent.updateRotation = true;

        if (player == null)
        {
            GameObject goPlayer = GameObject.FindGameObjectWithTag("Player");

            if (goPlayer != null)
            {
                player = goPlayer.transform;


                NavMeshAgent playerAgent = player.GetComponent<NavMeshAgent>(); // Configuramos el attackrange en el awake

                if (playerAgent != null)
                {
                    attackRange = playerAgent.radius + agent.radius + meleeRange;

                   // Debug.Log($"Attack Range set to {attackRange} for {gameObject.name}");
                }
                else
                {
                    attackRange = agent.radius + meleeRange;
                    //Debug.LogWarning($"Player does not have a NavMeshAgent. Attack Range set to {attackRange} for {gameObject.name}");
                }
            }
        }
        else 
        {
            NavMeshAgent playerAgent = player.GetComponent<NavMeshAgent>(); // Configuramos el attackrange en el awake en caso de que el player ya esté asignado en el inspector
            if (playerAgent != null)
            {
                attackRange = playerAgent.radius + agent.radius + meleeRange;

               // Debug.Log($"Attack Range set to {attackRange} for {gameObject.name}");
            }
            else
            {
                attackRange = agent.radius + meleeRange;
              //  Debug.LogWarning($"Player does not have a NavMeshAgent. Attack Range set to {attackRange} for {gameObject.name}");
            }
        }
    }


    private void Update()
    {
        bool validPlayer = player != null && player.CompareTag("Player");

        if (!validPlayer) //El player ha muerto o no está en la esca
        {
            if (state == EnemyState.Chase ||
                state == EnemyState.Attack ||
                state == EnemyState.Standby)
            {
                ResetToIdleState();
            }

            Idle();

            UpdateAnimator();

            return;
        }


        distanceToPlayer = Vector3.Distance(transform.position, player.position);

        crumb = GetBreadcrumb();

        bool canSeePlayer =
            distanceToPlayer <= chaseRange &&
            CheckLineOfSight(player);

        if (state != EnemyState.Attack && state != EnemyState.Standby)
        {
            if (canSeePlayer || crumb != null)
            {
                idleCounter = 0f;

                state = EnemyState.Chase;
            }
        }

        switch (state)
        {
            case EnemyState.Idle:
                Idle();
                break;

            case EnemyState.Patrol:
                Patrol();
                break;

            case EnemyState.Chase:
                Chase();
                break;

            case EnemyState.Attack:
                Attack();
                break;

            case EnemyState.Standby:
                Standby();
                break;
        }

        UpdateAnimator();
    }




    private Vector3 SetStandbyPosition()  //Modificamos el standby position para que solo se establezca si el enemigo puede ver al player
    {
        float minDistance =
            attackRange + 1.5f;

        int maxAttempts = 15;

        for (int i = 0; i < maxAttempts; i++)
        {
            float randomDistance =
                Random.Range(
                    minDistance,
                    standbyRange);

            Vector2 randomCircle =
                Random.insideUnitCircle.normalized *
                randomDistance;

            Vector3 desiredPosition =
                player.position +
                new Vector3(
                    randomCircle.x,
                    0f,
                    randomCircle.y);

            NavMeshHit hit;

            bool validPosition =
                NavMesh.SamplePosition(
                    desiredPosition,
                    out hit,
                    2f,
                    NavMesh.AllAreas);

            if (!validPosition)
                continue;

            Vector3 enemyEyePos =
                hit.position + Vector3.up;

            Vector3 playerEyePos =
                player.position + Vector3.up;

            Vector3 direction =
                (playerEyePos - enemyEyePos).normalized;

            float distance =
                Vector3.Distance(
                    enemyEyePos,
                    playerEyePos);

            bool blocked =
                Physics.Raycast(
                    enemyEyePos,
                    direction,
                    distance,
                    losMask);

            if (blocked)
                continue;

            return hit.position;
        }

        return transform.position;
    }




    private void ToggleAgent(bool isOn)
    {
        if (isOn)
        {
            if (agent.enabled)
                return;

            if (obstacle.enabled &&
                !isWaitingToEnableAgent)
            {
                obstacle.enabled = false;

                isWaitingToEnableAgent = true;

                return;
            }

            if (isWaitingToEnableAgent)
            {
                agent.enabled = true;

                isWaitingToEnableAgent = false;
            }
        }
        else
        {
            if (!agent.enabled)
                return;

            agent.enabled = false;

            obstacle.enabled = true;
        }
    }




    private Transform GetBreadcrumb()
    {
        Collider[] hits =
            Physics.OverlapSphere(
                transform.position,
                chaseRange,
                breadcrumbMask);

        if (hits.Length == 0)
            return null;

        Breadcrumb bestCrumb = null;

        float bestLife = -1f;

        foreach (Collider hit in hits)
        {
            Breadcrumb crumb =
                hit.GetComponent<Breadcrumb>();

            if (crumb == null)
                continue;

            bool hasLOS =
                CheckLineOfSight(crumb.transform);

            if (!hasLOS)
                continue;

            if (crumb.lifespan > bestLife)
            {
                bestLife = crumb.lifespan;
                bestCrumb = crumb;
            }
        }

        if (bestCrumb == null)
            return null;

        return bestCrumb.transform;
    }


    //==================ESTADOS==================

    private void Idle()
    {
        ToggleAgent(false);
        idleCounter -= Time.deltaTime;


        if (idleCounter <= 0f)
        {
            idleCounter = 0f;

            idleTimeOut =
                Random.Range(idleDelay.x,
                             idleDelay.y);

            idleCounter = idleTimeOut;

            Vector3 randomDirection =
                Random.insideUnitSphere *
                patrolRadius;

            randomDirection += startPosition;

            NavMeshHit hit;

            if (NavMesh.SamplePosition(
                randomDirection,
                out hit,
                patrolRadius,
                NavMesh.AllAreas))
            {
                patrolPosition = hit.position;
            }
            else
            {
                patrolPosition = startPosition;
            }

            state = EnemyState.Patrol;
        }
    }

    private void Patrol()   
    {
        ToggleAgent(true);

        if (isWaitingToEnableAgent)
            return;

        agent.stoppingDistance = 0f;

        if (!agent.hasPath || Vector3.Distance(agent.destination, patrolPosition) > 0.25f)
        {
            agent.SetDestination(patrolPosition);

            if (agent.pathStatus != NavMeshPathStatus.PathComplete)
            {
                patrolPosition = agent.destination;

                return;
            }
        }

        float remainingDistance = Vector3.Distance(transform.position, patrolPosition);

        if (remainingDistance <= 0.1f)
        {
            idleTimeOut = Random.Range(idleDelay.x, idleDelay.y);

            idleCounter = idleTimeOut;

            state = EnemyState.Idle;
        }
    }

    private void Chase()
    {
        ToggleAgent(true);

        if (isWaitingToEnableAgent)
            return;

        bool canSeePlayer = CheckLineOfSight(player);

        if (canSeePlayer)
        {
            agent.stoppingDistance = attackRange * 0.9f;

            //Debug.Log(agent.stoppingDistance+" StoppingDistance "+ agent.gameObject.name);

            idleCounter = idleTimeOut;

            bool playerMoved =
                Vector3.Distance(
                    player.position,
                    lastPlayerPosition) > 0.05f;

            bool canRecalculate =
                Time.timeSinceLevelLoad >
                lastDestinationCalculation +
                pathCalculationFrequency;




            if (distanceToPlayer <= attackRange)
            {
                bool canAttack = AttackPriorityManager.Instance.RequestAttack(this);

                if (canAttack)
                {
                    Debug.Log($"{gameObject.name} Can Attack");

                    attackTimer = 0f;

                    canExitAttack = false;

                    state = EnemyState.Attack;

                    return;
                }
            }

            bool allAttackSlotsOccupied =AttackPriorityManager.Instance.AreAllSlotsOccupied(); //Problema spam chase/standby al entrar al rango de standby, fixed, ahora el enemigo solo entra en standby si está en el rango correcto y todos los slots están ocupados

            if (distanceToPlayer <= standbyTriggerRange && allAttackSlotsOccupied)   //Los enemigos se bloqueaban entre si y daba lugar a que se quedasen en chase infinitamente al no llegar al attack range, ahora
            {                                              //con el stanbyrange, al estar cerca, ya pueden entrar en standby sin tener que estar en rango de ataque
                standbyPos = SetStandbyPosition();

                reachedStandbyPosition = false;

                standbyTimer = 0f;

                state = EnemyState.Standby;

               // Debug.Log($"{gameObject.name} Going Standby");

                return;
            }



            if (playerMoved || canRecalculate)
            {
                agent.SetDestination(player.position);

                lastPlayerPosition =
                    player.position;

                lastDestinationCalculation =
                    Time.timeSinceLevelLoad;
            }
        }
        else if (crumb != null)
        {
            agent.stoppingDistance = 0f;

            if (!agent.hasPath ||Vector3.Distance(agent.destination, crumb.position) > 0.25f)
            {
                agent.SetDestination(crumb.position);
            }
        }
        else
        {
            idleTimeOut = Random.Range(idleDelay.x, idleDelay.y);

            idleCounter = idleTimeOut;

            state = EnemyState.Idle;
        }
    }


    private void Standby()
    {
        ToggleAgent(true);

        if (isWaitingToEnableAgent)
            return;

        agent.stoppingDistance = 0.2f;

        if (!agent.hasPath || Vector3.Distance(agent.destination, standbyPos) > 0.25f)
        {
            agent.SetDestination(standbyPos);

            standbyPos = agent.destination;
        }

        float remainingDistance = agent.remainingDistance;

        bool hasFreeSlot =                              //El enemigo no volvia a chase inmediatamente al haber un slot libre para atacar, ahora si que lo hace
            AttackPriorityManager.Instance
            .HasFreeAttackSlot();

        if (hasFreeSlot)
        {
            agent.updateRotation = true;

            reachedStandbyPosition = false;

            state = EnemyState.Chase;

            return;
        }


        if (!reachedStandbyPosition)
        {
            standbyTimer += Time.deltaTime;

            bool reachedDestination = !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.05f;

            if (reachedDestination)
            {
                reachedStandbyPosition = true;

                agent.ResetPath();

                agent.velocity = Vector3.zero;
            }
            else if (standbyTimer >= maxStandbyReachTime)     //Introducimos variables para controlar el tiempo que el enemigo lleva intentando llegar a la posición
            {                                                   //Para evitar que se quede atascado intentando llegar a una posición inalcanzable, si pasa el tiempo máximo se recalcula la posición de standby
                Debug.Log($"{gameObject.name} Failed to reach standby position in time, recalculating...");

                standbyPos = SetStandbyPosition();

                standbyTimer = 0f;

                agent.SetDestination(standbyPos);
            }
        }


        if (reachedStandbyPosition)  //Si ha llegado a la posicion, se gira hacia el player aunque no pueda atacarlo
        {
            agent.updateRotation = false;

           // Debug.Log($"{gameObject.name} Reached Standby Position, rotating towards player.");
            RotateTowardsPlayer();
        }
        else
        {
            agent.updateRotation = true;
        }


        bool lostPlayer = distanceToPlayer > chaseRange && crumb == null;

        if (lostPlayer)
        {
            agent.updateRotation = true;

            state = EnemyState.Idle;
        }
    }


    private void Attack()
    {
        ToggleAgent(false);

        agent.updateRotation = false;

        RotateTowardsPlayer();

        attackTimer += Time.deltaTime;          //Implementamos el timer de ataque para controlar el tiempo minimo de ataque en el que el enemigo debe de estar. De esta manera
                                                    // Si el enemigo entra en rango/estado de ataque en algún momento, minimo deberá de estar este tiempo (1 animación más o menos)
        if (attackTimer >= minimumAttackTime)   //En este estado sin poder salir. De esta manera evitamos flickerings raros y problemas  por colisiones de enemigos y estados.    
        {
            canExitAttack = true;
        }

        bool lostPlayer =
            distanceToPlayer > chaseRange ||
            !CheckLineOfSight(player);

        bool tooFarToAttack =
            distanceToPlayer > attackRange + 0.5f;

        if (canExitAttack && (lostPlayer || tooFarToAttack))
        {
            AttackPriorityManager.Instance.ReleaseAttack(this);

            agent.updateRotation = true;

            state = EnemyState.Chase;

            return;
        }
    }








    private void RotateTowardsPlayer()
    {
        Vector3 direction =
            player.position - transform.position;

        direction.y = 0f;

        if (direction == Vector3.zero)
            return;

        Quaternion lookRotation =
            Quaternion.LookRotation(direction);

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                lookRotation,
                8f * Time.deltaTime);
    }


    private bool CheckLineOfSight(Transform target)
    {
        Vector3 myPos =
            transform.position + Vector3.up;

        Vector3 targetPos = new Vector3(
            target.position.x,
            myPos.y,
            target.position.z);

        float rayDistance =
            Vector3.Distance(myPos,
                             targetPos);

        Vector3 direction =
            (targetPos - myPos).normalized;

        RaycastHit hit;

        if (Physics.Raycast(myPos,
                            direction,
                            out hit,
                            rayDistance,
                            losMask))
        {
            Debug.DrawRay(myPos,
                          direction *
                          hit.distance,
                          Color.red);

            return false;
        }
        else
        {
            Debug.DrawRay(myPos,
                          direction *
                          rayDistance,
                          Color.green);

            return true;
        }
    }







    private void ResetToIdleState()
    {
        AttackPriorityManager.Instance?.ReleaseAttack(this);

        agent.updateRotation = true;

        reachedStandbyPosition = false;

        standbyTimer = 0f;

        attackTimer = 0f;

        canExitAttack = false;

        if (agent.enabled)
        {
            agent.ResetPath();
        }

        idleTimeOut =
            Random.Range(
                idleDelay.x,
                idleDelay.y);

        idleCounter = idleTimeOut;

        state = EnemyState.Idle;
    }



    private void UpdateAnimator()                               //En mi caso he decidido prescindir de utilizar el NavigationSimpleAgent para controlar las animaciones, pasando a controlarlas desde este script
    {
        if (anim == null)
            return;

        float moveAmount = 0f;

        if (agent.enabled)
        {
            moveAmount = agent.velocity.magnitude;
        }

        if (state == EnemyState.Standby && reachedStandbyPosition)  //Si está en standby y ha llegado a la posición, no se mueve aunque el agente esté habilitado
        {
            moveAmount = 0f;
        }

        anim.SetFloat("move", moveAmount);

        anim.SetBool(
            "attack",
            state == EnemyState.Attack);

        anim.SetBool(
            "standby",
            state == EnemyState.Standby && reachedStandbyPosition); //Si está en standby y ha llegado a la posición, establecemos el bool para la anim

    }


    private void OnDisable()
    {
        if (AttackPriorityManager.Instance != null)
        {
            AttackPriorityManager.Instance.ReleaseAttack(this);
        }
    }
}