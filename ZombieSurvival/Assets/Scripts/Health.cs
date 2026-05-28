using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;
using TMPro;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private bool isPlayer;

    [Header("Health")]
    [SerializeField] public float healthMax = 100f;

    [SerializeField]
    public float health = 100f;



    [Header("Regeneration")]
    [SerializeField]
    private float regionPerSecond = 1f;
    [SerializeField]
    private float regionDistance = 8f;

    private LayerMask opponentMask;

   
    [Header("UI")]
    [SerializeField]
    private Image healthBarFill;

    [SerializeField]
    private TextMeshProUGUI healthText;

    [SerializeField]
    private Image enemyHealthBarFill;




    [Header("VFX")]
    [SerializeField]
    private GameObject bloodPuddlePrefab;

    [SerializeField]
    private GameObject feedbackTextPrefab;





    private Combat combat;
    private EnemyAI enemyAI;

    private bool isDead;


    private Transform tempContainer;

    [SerializeField]
    private int deathAnimations = 12;

    public Transform groundCheck;



    private void Awake()
    {
        isPlayer = gameObject.CompareTag("Player");

        combat = GetComponent<Combat>();

        if (!isPlayer)
        {
            enemyAI = GetComponent<EnemyAI>();
            opponentMask = LayerMask.GetMask("Player");
            enemyHealthBarFill = transform.Find("Canvas/Enemy Health Fill").GetComponent<Image>();
        }
        else 
        {
            opponentMask = LayerMask.GetMask("Enemy");
            healthBarFill = GameObject.Find("Player Health Bar Fill").GetComponent<Image>();
            healthText = GameObject.Find("Player Health Text").GetComponent<TextMeshProUGUI>();
        }

        health = healthMax;

        GameObject goTemp = GameObject.Find("TempContainer");

        if (goTemp != null)
        {
            tempContainer = goTemp.transform;
        }


        InvokeRepeating(nameof(Regeneration),1f,1f);
        UpdateUI();
    }


    private void Start()
    {
        if (isPlayer)
        {
            AttackPriorityManager.Instance.CurrentMaxAttackers(this);
        }
    }


    private void Regeneration()
    {
        if (regionPerSecond <= 0f)
            return;

        bool isAlone =  Physics.OverlapSphere( transform.position,regionDistance,opponentMask).Length == 0;

        if (isAlone && health < healthMax)
        {
            ChangeHealth(regionPerSecond);
        }
    }



    public void ChangeHealth(float amount)
    {
        if (isDead)
            return;

        if (amount < 0)
        {

            SpawnBloodPuddle(this.gameObject.transform.position);
        }


        health += amount;

        health = Mathf.Clamp(health, 0f, healthMax);

        UpdateUI();

        if (feedbackTextPrefab != null && Mathf.Round(amount) != 0)
        {
            GameObject goFeedback =
                Instantiate(
                    feedbackTextPrefab,
                    transform.position,
                    Quaternion.identity,
                    tempContainer);

            goFeedback.name = "Feedback Text";

            FeedbackText feedback = goFeedback.GetComponent<FeedbackText>();

            if (feedback != null)
            {
                feedback.ChangeText(amount);
            }
        }



        if (isPlayer)
        { 
            AttackPriorityManager.Instance.CurrentMaxAttackers(this);
        }

        Debug.Log(
            gameObject.name +
            " Health Reduced to: " +
            health);

        if (health <= 0f)
        {
            Death();
        }
    }


    private void UpdateUI()
    {
        if (!isPlayer && enemyHealthBarFill != null)
        {
            enemyHealthBarFill.fillAmount = health / healthMax;

            bool show = health > 0 && health < healthMax;

            enemyHealthBarFill.transform.parent.gameObject.SetActive(show);
            return;
        }

        if (healthBarFill != null)
        {
            healthBarFill.fillAmount =
                health / healthMax;
        }

        if (healthText != null)
        {
            healthText.text =
                Mathf.Round(health) +" / " + healthMax;
        }

    }


    private void SpawnBloodPuddle(Vector3 position)
    {
        if (bloodPuddlePrefab == null)
            return;

        // Offset aleatorio en un pequeño radio
        Vector2 offset = Random.insideUnitCircle * 0.3f; // 0.3 = radio del desplazamiento
        Vector3 finalPos = position + new Vector3(offset.x, 0.05f, offset.y);

        Quaternion randomRotation = Quaternion.Euler(900f, Random.Range(0f, 360f), 0f);

        if (groundCheck != null)
        { 
            finalPos.y = groundCheck.position.y; // Asegura que la mancha esté a nivel del suelo
        }

        GameObject puddle = Instantiate(
            bloodPuddlePrefab,
            finalPos,
            randomRotation
        );

    }

    private void Death()
    {
        isDead = true;

        Debug.Log(gameObject.name + " died");

        if (combat != null)
        {
            combat.enabled = false;
        }

        if (enemyAI != null)
        {
            enemyAI.enabled = false;
        }

        Animator anim = GetComponent<Animator>();

        if (anim != null)
        {
            int deathID = Random.Range(1,deathAnimations);

            anim.SetInteger("deathID", deathID);
            anim.SetTrigger("death");
        }
            
        NavMeshAgent agent = GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            agent.enabled = false;
        }

        NavMeshObstacle obstacle = GetComponent<NavMeshObstacle>();

        if (obstacle != null)
        {
            obstacle.enabled = false;
        }

        Collider[] colliders = GetComponents<Collider>();

        ClickToMove clickToMove = GetComponent<ClickToMove>();
        if (clickToMove != null)
        {
            clickToMove.enabled = false;
        }

        LocomotionSimpleAgent locomotion = GetComponent<LocomotionSimpleAgent>();
        if (locomotion != null)
        {
            locomotion.enabled = false;
        }

        Breadcrumb breadcrumb = GetComponent<Breadcrumb>();
        if (breadcrumb != null)
        {
            breadcrumb.enabled = false;
        }

        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }


        if (!isPlayer)
        {
            Destroy(gameObject, 8f);
        }
        else 
        {
            gameObject.tag = "DeadPlayer";
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, regionDistance);
    }
}