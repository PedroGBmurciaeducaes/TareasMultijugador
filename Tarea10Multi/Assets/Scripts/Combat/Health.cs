using System;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class Health : NetworkBehaviour
{
    // Salud actual sincronizada en red (solo el servidor puede modificarla)
    public NetworkVariable<int> currentHealth = new NetworkVariable<int>( writePerm: NetworkVariableWritePermission.Server);

    [field: SerializeField]
    public int MaxHealth { get; private set; } = 100;

    private bool isDead = false;

    public event Action<Health> OnDie;

    public ReSpawnManager reSpawnManager;

    Transform spawnpoint;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            currentHealth.Value = MaxHealth;
            reSpawnManager = FindFirstObjectByType<ReSpawnManager>();
        }
    }


    public void TakeDamage(int damageValue)
    {
        ModifyHealth(-damageValue);
    }


    public void RestoreHealth(int healValue)
    {
        ModifyHealth(healValue);
    }


    private void ModifyHealth(int value)
    {
        if (!IsServer) return;
        if (isDead) return;

        int newHealth = currentHealth.Value + value;
        currentHealth.Value = Mathf.Clamp(newHealth, 0, MaxHealth);

        if (currentHealth.Value <= 0)
        {
            isDead = true;
            OnDie?.Invoke(this);

            // Respawn
            Respawn();
        }
    }

    private void Respawn()              //NO FUNCIONA LA MECANICA DE RESPAWN
    {
        currentHealth.Value = MaxHealth;
        isDead = false;

        spawnpoint = reSpawnManager.GetRandomSpawnPoint();
        Debug.Log(spawnpoint);

        NetworkObject netObj = GetComponentInParent<NetworkObject>();
        NetworkTransform netTransform = netObj.GetComponent<NetworkTransform>();

        Debug.Log(netObj);
        Debug.Log(netTransform);


        netTransform.Teleport(
            spawnpoint.position,
            netObj.transform.rotation,
            netObj.transform.localScale
        );
    }

}
