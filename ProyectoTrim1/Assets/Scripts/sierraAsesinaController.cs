using UnityEngine;

public class sierraAsesinaController : MonoBehaviour
{
    public Transform origin;
    public Transform destination;
    public Transform platform;
    public float speed = 2.0f;

    public bool pingpong = true;

    private bool movingToDestination;







    void Start()
    {
        platform.position = origin.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (platform == null || origin == null || destination == null)
        {
            return;
        }


        Transform target = movingToDestination ? destination : origin;

        platform.position = Vector3.MoveTowards(platform.position,
        target.position, speed * Time.deltaTime);

        if (Vector3.Distance(platform.position, target.position) < 0.001f)
        {
            if (pingpong)
            {
                movingToDestination = !movingToDestination;
            }
        }

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            LevelManager manager = FindAnyObjectByType<LevelManager>();
            manager.reespawnPlayer();
        }
    }



    private void OnDrawGizmos()
    {
        if (origin != null && destination != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(origin.position, 0.2f);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(destination.position, 0.2f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(origin.position, destination.position);
        }
    }

}
