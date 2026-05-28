// LookAt.cs
using UnityEngine;
using System.Collections;

// Obliga a que el GameObject tenga un componente Animator
[RequireComponent(typeof(Animator))]
public class LookAt : MonoBehaviour
{

    // Transform de la cabeza del personaje
    public Transform head = null;

    // Posición objetivo hacia donde mirará el personaje
    public Vector3 lookAtTargetPosition;

    // Tiempo que tarda en empezar a mirar
    public float lookAtCoolTime = 0.2f;

    // Tiempo que tarda en dejar de mirar
    public float lookAtHeatTime = 0.2f;

    // Indica si el personaje debe mirar o no
    public bool looking = true;

    // Posición actual suavizada de la mirada
    private Vector3 lookAtPosition;

    // Referencia al Animator
    private Animator anim;

    // Peso de la mirada (0 = no mirar, 1 = mirar completamente)
    private float lookAtWeight = 0.0f;

    // Start se ejecuta al iniciar el objeto
    void Start()
    {
        // Comprueba que exista una referencia a la cabeza
        if (!head)
        {
            // Muestra error en consola
            Debug.LogError("No head transform - LookAt disabled");

            // Desactiva el script
            enabled = false;
            return;
        }

        // Obtiene el componente Animator
        anim = GetComponent<Animator>();

        // Define la posición inicial de la mirada hacia delante
        lookAtTargetPosition = head.position + transform.forward;

        // Inicializa la posición actual de la mirada
        lookAtPosition = lookAtTargetPosition;
    }

    // OnAnimatorIK se ejecuta durante el sistema IK del Animator
    void OnAnimatorIK()
    {
        // Mantiene la mirada a la misma altura de la cabeza
        lookAtTargetPosition.y = head.position.y;

        // Determina el peso objetivo según si debe mirar o no
        float lookAtTargetWeight = looking ? 1.0f : 0.0f;

        // Dirección actual de la mirada
        Vector3 curDir = lookAtPosition - head.position;

        // Dirección futura hacia el objetivo
        Vector3 futDir = lookAtTargetPosition - head.position;

        // Rota suavemente la dirección actual hacia la futura
        curDir = Vector3.RotateTowards(
            curDir,
            futDir,
            6.28f * Time.deltaTime, // Velocidad de rotación
            float.PositiveInfinity);

        // Actualiza la posición de la mirada suavizada
        lookAtPosition = head.position + curDir;

        // Elige el tiempo de transición dependiendo
        // de si empieza o deja de mirar
        float blendTime = lookAtTargetWeight > lookAtWeight
            ? lookAtHeatTime
            : lookAtCoolTime;

        // Interpola suavemente el peso de la mirada
        lookAtWeight = Mathf.MoveTowards(
            lookAtWeight,
            lookAtTargetWeight,
            Time.deltaTime / blendTime);

        // Configura cuánto afecta la mirada al cuerpo
        anim.SetLookAtWeight(
            lookAtWeight, // Peso general
            0.2f,          // Peso del cuerpo
            0.5f,          // Peso de la cabeza
            0.7f,          // Peso de los ojos
            0.5f);         // Limitación de rotación

        // Establece la posición exacta hacia donde mirar
        anim.SetLookAtPosition(lookAtPosition);
    }
}