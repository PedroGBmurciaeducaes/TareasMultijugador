using UnityEngine;
using UnityEngine.AI; 

// Obliga a que el GameObject tenga un componente NavMeshAgent
[RequireComponent(typeof(NavMeshAgent))]
public class ClickToMove : MonoBehaviour
{
    // Prefab que aparecerá donde hagamos click
    [SerializeField] private GameObject clickTarget;

    private NavMeshAgent agent;

    private Camera cam;

    // Variable para guardar información del Raycast
    private RaycastHit hit;

    // Contenedor temporal donde se guardarán los objetos instanciados
    private Transform tempContainer;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        cam = Camera.main;

        // Busca un objeto llamado "TempContainer" en la escena
        // y guarda su Transform
        tempContainer = GameObject.Find("TempContainer").transform;
    }

    private void Update()
    {
        // Comprueba si se ha pulsado el botón izquierdo del ratón
        if (Input.GetMouseButtonDown(0))
        {
            // Crea un rayo desde la cámara hacia la posición del mouse
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            // Lanza el Raycast para detectar si golpea algo
            if (Physics.Raycast(ray, out hit))
            {
                // Instancia el prefab clickTarget en el punto donde se hizo click
                GameObject goClick = Instantiate(
                    clickTarget,          // Prefab a crear
                    hit.point,            // Posición del impacto
                    Quaternion.identity,  // Sin rotación
                    tempContainer);       // Padre del objeto instanciado

                goClick.name = "ClickTarget";

                agent.SetDestination(hit.point);
            }
        }
    }
}