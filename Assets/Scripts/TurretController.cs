using UnityEngine;
using Unity.Netcode;

public class TurretController : NetworkBehaviour
{
    public float rotateSpeed = 120f;
    public bool controlWithMouse = true;
    public Transform tankBody; // referencia al cuerpo del tanque

    void Update()
    {
        // --- seguir al cuerpo ---
        if (tankBody != null)
        {
            transform.position = tankBody.position;
        }


        // --- control de rotación ---
        if (controlWithMouse)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = mousePos - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            if (Input.GetKey(KeyCode.N))
                transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
            else if (Input.GetKey(KeyCode.M))
                transform.Rotate(Vector3.forward * -rotateSpeed * Time.deltaTime);
        }
    }
}
