using UnityEngine;
using TMPro;
using Networking.Host;
using Networking.Client;

public class MainMenu : MonoBehaviour
{
    // Asignar este campo en el Inspector con el InputField de TextMeshPro
    [SerializeField] private TMP_InputField joinCodeInput;

    // Botón HOST
    public async void OnClickHostButton()
    {
        Debug.Log("Botón HOST pulsado.");

        if (HostSingleton.Instance == null)
        {
            Debug.LogError("HostSingleton no encontrado en la escena.");
            return;
        }

        if (HostSingleton.Instance.GameManager == null)
        {
            Debug.LogError("HostGameManager no inicializado.");
            return;
        }

        // Llama al método para iniciar el host vía Relay
        await HostSingleton.Instance.GameManager.StartHostAsync();

        // Después de crear el host, el join code debería estar disponible
        string code = HostSingleton.Instance.GameManager.GetJoinCode();
        Debug.Log($"Join Code generado: {code}");
    }

    // Botón CLIENT
    public async void OnClickClientButton()
    {
        Debug.Log("Botón CLIENT pulsado.");

        string joinCode = joinCodeInput.text;

        if (string.IsNullOrEmpty(joinCode))
        {
            Debug.LogWarning("Join Code vacío. Por favor ingresa un código válido.");
            return;
        }

        if (ClienteSingleton.Instance == null || ClienteSingleton.Instance.GameManager == null)
        {
            Debug.LogError("ClienteSingleton o ClienteGameManager no inicializado.");
            return;
        }

        // Llama al método del ClienteGameManager para unirse a un host
        await ClienteSingleton.Instance.GameManager.JoinHostAsync(joinCode);
    }
}