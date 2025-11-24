using Unity.Netcode;
using UnityEngine;

public class TankController : NetworkBehaviour
{
    [SerializeField] private InputReader inputReader;

    private Vector2 moveInput;
    private float rotateBodyInput;

    public float moveSpeed = 5f;
    public float rotateSpeed = 100f;

    private void Start()
    {
        if (!IsOwner) return;

        inputReader.MoveEvent += OnMove;
        inputReader.RotateBodyEvent += OnRotateBody;
        inputReader.PrimaryFireEvent += OnFire;
    }

    private void OnDestroy()
    {
        if (!IsOwner) return;

        inputReader.MoveEvent -= OnMove;
        inputReader.RotateBodyEvent -= OnRotateBody;
        inputReader.PrimaryFireEvent -= OnFire;
    }

    private void OnMove(Vector2 move) => moveInput = move;

    private void OnRotateBody(float rotate) => rotateBodyInput = rotate;

    private void OnFire(bool firing)
    {
        Debug.Log("Firing: " + firing);
    }

    private void Update()
    {
        if (!IsOwner) return;

        // Movimiento adelante/atrás
        transform.Translate(Vector3.up * moveInput.y * moveSpeed * Time.deltaTime, Space.Self);

        // Rotación del cuerpo
        transform.Rotate(Vector3.forward * -rotateBodyInput * rotateSpeed * Time.deltaTime);
    }
}
