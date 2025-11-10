using Unity.Netcode;
using UnityEngine;

public class MovimientoMano : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public float rotateSpeed = 100f;

    void Update()
    {
        float moveInput = Input.GetAxis("Vertical"); // W/S
        bool turnLeft = Input.GetKey(KeyCode.Q);
        bool turnRight = Input.GetKey(KeyCode.E);

        // Mover hacia adelante o atrás según el eje local del cuerpo
        transform.Translate(Vector3.up * moveInput * moveSpeed * Time.deltaTime, Space.Self);

        // Girar con Q y E
        if (turnLeft)
            transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
        else if (turnRight)
            transform.Rotate(Vector3.forward * -rotateSpeed * Time.deltaTime);
    }
}
