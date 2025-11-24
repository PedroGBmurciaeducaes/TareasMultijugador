using Unity.Netcode;
using UnityEngine;

public class Torretas : NetworkBehaviour
{

    public float rotateSpeed = 120f;
    public bool controlWithMouse = true;
    public Transform tankBody; // referencia al cuerpo del tanque


    [SerializeField] private InputReader inputReader;
    private void Start()
    {
        inputReader.MoveEvent += HandleMove;
        inputReader.PrimaryFireEvent += HandlePrimaryFire;
    }


    private void OnDestroy()
    {
        inputReader.MoveEvent -= HandleMove;
        inputReader.PrimaryFireEvent -= HandlePrimaryFire;
    }


    private void HandleMove(Vector2 movement)
    {
        Debug.Log("Movimiento: " + movement);
    }
    private void HandlePrimaryFire(bool isFiring)
    {
        Debug.Log("Disparando: " + isFiring);
    }
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
