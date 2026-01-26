using UnityEngine;
using Unity.Netcode;

public class DealDamageOnContact : NetworkBehaviour
{
    [SerializeField] private int damage = 5;

    private ulong ownerClientId;

    // Se llama desde el script que lanza el proyectil
    public void SetOwner(ulong ownerClientId)
    {
        this.ownerClientId = ownerClientId;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("TRIGGER: " + col.name);

        if (!IsServer)
        {
            Debug.Log("No soy servidor");
            return;
        }

        if (col.attachedRigidbody == null)
        {
            Debug.Log("No tiene Rigidbody");
            return;
        }

        Debug.Log("Rigidbody encontrado");

        // NetworkObject en el padre
        NetworkObject networkObject = col.attachedRigidbody.GetComponentInParent<NetworkObject>();
        if (networkObject == null)
        {
            Debug.Log("No tiene NetworkObject en padre");
            return;
        }

        Debug.Log("NetworkObject encontrado: " + networkObject.name);

        if (networkObject.OwnerClientId == ownerClientId)
        {
            Debug.Log("Es mi propio jugador, no daño");
            return;
        }

        // Aquí el Health
        if (col.attachedRigidbody.TryGetComponent<Health>(out Health health))
        {
            Debug.Log("Health encontrado en el rigidbody");
            health.TakeDamage(damage);
            Debug.Log("Daño aplicado");
        }
        else
        {
            Debug.Log("NO hay Health en el rigidbody");
        }
    }

}
