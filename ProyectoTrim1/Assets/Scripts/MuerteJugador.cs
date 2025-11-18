using UnityEngine;

public class MuerteJugador : MonoBehaviour
{



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Zona de muerte");
            LevelManager manager = FindAnyObjectByType<LevelManager>();
            manager.reespawnPlayer();

        }
    }





    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
