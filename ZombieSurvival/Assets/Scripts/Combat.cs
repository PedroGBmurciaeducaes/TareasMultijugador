using UnityEngine;
using UnityEngine.AI;

public class Combat : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private bool isPlayer;

    [Header("Damage")]
    [SerializeField]
    private Vector2 dmgRange = new Vector2(2f, 5f);

    [SerializeField]
    private float attackRadius = 1.5f;

    private LayerMask opponentMask;

    private Animator anim;
    private NavMeshAgent agent;

    private EnemyAI enemyAI;

    [Header("Rotation")]
    [SerializeField]
    private float lookAtSpeed = 10f;

    private Transform currentTarget;

    [Header("Attack")]
    [SerializeField]
    private float attackExitTime = 0.85f;

    private bool movementLocked;

    private float unlockTimer;



    private void Awake()
    {
        isPlayer =
            gameObject.CompareTag("Player");

        anim =
            GetComponent<Animator>();

        agent =
            GetComponent<NavMeshAgent>();

        opponentMask =
            isPlayer
            ? LayerMask.GetMask("Enemy")
            : LayerMask.GetMask("Player");

        enemyAI =
            GetComponent<EnemyAI>();
    }



    private void Start()
    {
        if (enemyAI != null)
        {
            attackRadius =
                enemyAI.AttackRange;
        }
        else
        {
            attackRadius = 1f;
        }
    }



    private void Update()
    {
        if (!isPlayer)
            return;

        HandleAttack();
    }





    private void HandleAttack()
    {
        bool attackInput =
            Input.GetMouseButton(1);

        // Mientras el ataque sigue bloqueado
        if (movementLocked)
        {
            unlockTimer -= Time.deltaTime;

            agent.velocity = Vector3.zero;

            if (currentTarget != null)
            {
                RotateTowardsTarget();
            }

            // Solo desbloquea cuando:
            // 1. Ha terminado el tiempo
            // 2. El jugador NO mantiene click
            if (unlockTimer <= 0f && !attackInput)
            {
                movementLocked = false;

                agent.isStopped = false;

                anim.SetBool("attack", false);
            }

            return;
        }

        // Empezar ataque
        if (attackInput)
        {
            movementLocked = true;

            unlockTimer = attackExitTime;

            // Cancelamos movimiento actual
            agent.ResetPath();

            agent.velocity = Vector3.zero;

            agent.isStopped = true;

            FindClosestTarget();

            if (currentTarget != null)
            {
                RotateTowardsTarget();
            }

            anim.SetBool("attack", true);
        }
    }

    public void ImpactEvent()
    {
        Vector3 offset =
            transform.forward * 0.5f +
            transform.up;

        Collider[] hits =
            Physics.OverlapSphere(
                transform.position + offset,
                attackRadius,
                opponentMask);

        if (hits.Length > 0)
        {
            foreach (Collider hit in hits)
            {
                float dmgAmt =
                    Mathf.Round(
                        Random.Range(
                            dmgRange.x,
                            dmgRange.y));

                if (isPlayer)
                {
                    EnemyAI enemyAI =
                        hit.GetComponent<EnemyAI>();

                    if (enemyAI != null)
                    {
                        enemyAI.priorityBonus--;   //HAY QUE IMPLEMENTAR PRIORITYBONUS (Finalmente no implementado)
                    }
                }

                Health health =
                    hit.GetComponent<Health>();

                if (health != null)
                {
                    health.ChangeHealth(-dmgAmt);
                }
            }
        }
    }



    private void FindClosestTarget()
    {
        Collider[] hits =
            Physics.OverlapSphere(
                transform.position,
                10f,
                opponentMask);

        float closestDistance =
            Mathf.Infinity;

        Transform bestTarget = null;

        foreach (Collider hit in hits)
        {
            float distance =
                Vector3.Distance(
                    transform.position,
                    hit.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;

                bestTarget = hit.transform;
            }
        }

        currentTarget = bestTarget;
    }



    private void RotateTowardsTarget()
    {
        if (currentTarget == null)
            return;

        Vector3 direction =
            currentTarget.position -
            transform.position;

        direction.y = 0f;

        if (direction == Vector3.zero)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                lookAtSpeed * Time.deltaTime);
    }
}