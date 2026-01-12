using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader", menuName = "Scriptable Objects/InputReader")]
public class InputReader : ScriptableObject, Controls.IPlayerActions
{
    private Controls controls;

    public event Action<Vector2> MoveEvent;           // Cuerpo adelante/atrás
    public event Action<float> RotateBodyEvent;       // Rotación cuerpo Q/E
    public event Action<float> RotateTurretEvent;     // Rotación torreta N/M
    public event Action<bool> PrimaryFireEvent;       // Disparo

    private void OnEnable()
    {
        if (controls == null)
        {
            controls = new Controls();
            controls.Player.SetCallbacks(this);
        }

        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    // ================================================
    // ACTION CALLBACKS DEL INPUT SYSTEM
    // ================================================

    // Movimiento del cuerpo (W/S)
    public void OnMoveBody(InputAction.CallbackContext context)
    {
        MoveEvent?.Invoke(context.ReadValue<Vector2>());
    }

    // Rotación del cuerpo (Q/E)
    public void OnRotateBody(InputAction.CallbackContext context)
    {
        RotateBodyEvent?.Invoke(context.ReadValue<float>());
    }

    // Rotación de la torreta (N/M)
    public void OnRotateTurret(InputAction.CallbackContext context)
    {
        RotateTurretEvent?.Invoke(context.ReadValue<float>());
    }

    // Disparo
    public void OnPrimaryFire(InputAction.CallbackContext context)
    {
        if (context.performed)
            PrimaryFireEvent?.Invoke(true);
        else if (context.canceled)
            PrimaryFireEvent?.Invoke(false);
    }


}
