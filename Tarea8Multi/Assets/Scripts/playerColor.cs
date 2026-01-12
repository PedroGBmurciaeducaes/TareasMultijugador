using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;

public class PlayerColor : NetworkBehaviour
{
    private List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();

    // Color sincronizado en red
    private NetworkVariable<Color> playerColor = new NetworkVariable<Color>(
        Color.white,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    void Awake()
    {
        // Recolectar TODOS los SpriteRenderers del tanque (ruedas, torreta, cuerpo, etc.)
        spriteRenderers.AddRange(GetComponentsInChildren<SpriteRenderer>(true));
    }

    public override void OnNetworkSpawn()
    {
        // Cada vez que cambie el color en red, aplicarlo localmente
        playerColor.OnValueChanged += OnColorChanged;

        if (IsOwner)
        {
            StartCoroutine(SetRandomColorWithDelay());
        }
        else
        {
            // Aplica el color actual para late joiners
            ApplyColor(playerColor.Value);
        }
    }

    private IEnumerator SetRandomColorWithDelay()
    {
        yield return new WaitForSeconds(0.1f);
        // Generar un color aleatorio brillante
        Color newColor = Random.ColorHSV(0f, 1f, 0.6f, 1f, 0.8f, 1f);
        playerColor.Value = newColor;
    }

    private void OnColorChanged(Color oldColor, Color newColor)
    {
        ApplyColor(newColor);
    }

    private void ApplyColor(Color color)
    {
        foreach (var sr in spriteRenderers)
        {
            if (sr == null) continue;
            sr.color = color;
        }
    }

    private void OnDestroy()
    {
        playerColor.OnValueChanged -= OnColorChanged;
    }
}
