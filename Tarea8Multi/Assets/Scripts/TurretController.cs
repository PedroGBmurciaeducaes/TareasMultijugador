using UnityEngine;
using Unity.Netcode;

public class TurretController : NetworkBehaviour
{
    [SerializeField] private InputReader inputReader;

    public float rotateSpeed = 120f;
    public bool controlWithMouse = false;   //  Si quieres usar el mouse, activa esto
    public Transform tankBody;

    private float rotateInput = 0f;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        inputReader.RotateTurretEvent += OnRotateTurret;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

        inputReader.RotateTurretEvent -= OnRotateTurret;
    }

    private void OnRotateTurret(float value)
    {
        rotateInput = value;
    }

    void Update()
    {
        if (!IsOwner) return;

        // --- Seguir al cuerpo ---
        if (tankBody != null)
            transform.position = tankBody.position;

        // --- Control con ratón ---
        if (controlWithMouse)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = mousePos - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            // --- Control con N/M usando InputReader ---
            transform.Rotate(Vector3.forward * -rotateInput * rotateSpeed * Time.deltaTime);
        }
    }
}
