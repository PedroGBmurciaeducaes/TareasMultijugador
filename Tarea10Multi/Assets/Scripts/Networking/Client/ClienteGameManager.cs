using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Networking.Client
{
    public class ClienteGameManager
    {
        private const string MenuSceneName = "Menu";

        // Inicializa Unity Services y autentica
        public async Task<bool> InitAsync()
        {
            Debug.Log("Inicializando Unity Services...");

            await UnityServices.InitializeAsync();

            var authState = await AuthenticationWrapper.DoAuth();

            if (authState == AuthState.Authenticated)
            {
                GoToMenu();
                return true;
            }

            Debug.LogError("Falló la autenticación.");
            return false;
        }


        // Cargar la escena de menú
        public void GoToMenu()
        {
            SceneManager.LoadScene(MenuSceneName);
        }




        // Método principal para unirse a un host usando join code
        public async Task JoinHostAsync(string joinCode)
        {
            try
            {
                Debug.Log("Intentando unirse al host con código: " + joinCode);

                //  Unirse a la allocation existente en Relay
                JoinAllocation joinAllocation =
                    await RelayService.Instance.JoinAllocationAsync(joinCode);

                Debug.Log("JoinAllocation recibida correctamente.");

                //  Configurar UnityTransport con los parámetros individuales
                var unityTransport =
                    NetworkManager.Singleton.GetComponent<UnityTransport>();

                unityTransport.SetRelayServerData(
                    joinAllocation.RelayServer.IpV4,               // IP del Relay
                    (ushort)joinAllocation.RelayServer.Port,       // Puerto
                    joinAllocation.AllocationIdBytes,              // AllocationIdBytes
                    joinAllocation.Key,                            // Key
                    joinAllocation.ConnectionData,                 // Datos del cliente
                    joinAllocation.HostConnectionData,            // Datos del host
                    true                                           // EnableEncryption
                );

                Debug.Log("Transport configurado correctamente para cliente Relay.");

                //  Iniciar cliente
                bool started = NetworkManager.Singleton.StartClient();
                Debug.Log($"StartClient result: {started}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error al unirse vía Relay: {e}");
            }
        }
    }
}