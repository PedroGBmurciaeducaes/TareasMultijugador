using UnityEngine;

public class SpawPoint : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            LevelManager manager = FindAnyObjectByType<LevelManager>();
            manager.setSpawnPoint(this.transform);

        }


    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
