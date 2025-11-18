using Unity.VisualScripting;
using UnityEngine;

public class PalancaController : MonoBehaviour
{

    bool activa=false;
    public Transform puerta;
    public Transform plataformaMovil;
    private bool jugadorDentro = false;


    // Update is called once per frame
    void Update()
    {
        if (jugadorDentro && Input.GetKeyDown(KeyCode.E))
        {
            activarPalanca();
        }
    }



    private void activarPalanca()
    {
        if (activa == false)
        {
            activa = true;
            puerta.GetComponent<BoxCollider2D>().enabled = false;
            puerta.transform.position += new Vector3(0, 5, 0); 
            plataformaMovil.GetComponent<simplePlatformController>().pingpong = true;

        }
        else 
        {
            activa=false;
            puerta.transform.position -= new Vector3(0, 5, 0);
            puerta.GetComponent<BoxCollider2D>().enabled = true;
            plataformaMovil.GetComponent<simplePlatformController>().pingpong = false;

        }

    }


    // Detección del jugador
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorDentro = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorDentro = false;
        }
    }
}
