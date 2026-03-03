using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class HealthDisplay : NetworkBehaviour
{
    public Health health;
    public Image healthBarImage;


    public override void OnNetworkSpawn()
    {
        if (!IsClient) return; // Solo los clientes necesitan la UI

        health.currentHealth.OnValueChanged += HandleHealthChanged;

        // Inicializar la barra con el valor actual
        HandleHealthChanged(0, health.currentHealth.Value);
    }

    public override void OnNetworkDespawn()
    {
        if (!IsClient) return;

        health.currentHealth.OnValueChanged -= HandleHealthChanged;
    }

    private void HandleHealthChanged(int previousHealth, int newHealth)
    {
        float healthPercentage = (float)newHealth / health.MaxHealth;
        healthBarImage.fillAmount = healthPercentage;
    }
}
