using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Networking.Host
{
    public class HostGameManager
    {
        private const int MAX_CONNECTIONS = 20;
        private const string GAME_SCENE_NAME = "Game";

        private string joinCode;
        private Allocation allocation;

        public async Task StartHostAsync()
        {
            try
            {
                Debug.Log("Iniciando host vía Relay...");

                // ---------------- INICIALIZAR UNITY SERVICES ----------------
                await Unity.Services.Core.UnityServices.InitializeAsync();
                Debug.Log("Unity Services inicializados.");

                // ---------------- CREAR ALLOCATION ----------------
                allocation = await RelayService.Instance.CreateAllocationAsync(MAX_CONNECTIONS);
                Debug.Log($"Allocation creada. ID: {allocation.AllocationId}");
                Debug.Log($"Relay IP: {allocation.RelayServer.IpV4}, Port: {allocation.RelayServer.Port}");

                // ---------------- OBTENER JOIN CODE ----------------
                joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                Debug.Log($"Join Code: {joinCode}");

                // ---------------- CONFIGURAR UNITY TRANSPORT ----------------
                var unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();

                Debug.Log("Unity transport" + unityTransport);
                // Usamos la versión que acepta parámetros explícitos
               

                unityTransport.SetRelayServerData(
                    allocation.RelayServer.IpV4,
                    (ushort)allocation.RelayServer.Port,
                    allocation.AllocationIdBytes,
                    allocation.Key,
                    allocation.ConnectionData,
                    allocation.ConnectionData,  // host connection data (mismo que ConnectionData para host)
                    true // indica que es host
                );
                Debug.Log("UnityTransport configurado para Relay.");





                // ---------------- INICIAR HOST ----------------
                bool started = NetworkManager.Singleton.StartHost();
                Debug.Log($"StartHost result: {started}");

                if (!started)
                {
                    Debug.LogError("StartHost falló. Considera recrear la allocation y reiniciar NetworkManager.");
                    return;
                }


                // ---------------- CARGAR ESCENA DE JUEGO ----------------
                NetworkManager.Singleton.SceneManager.LoadScene(
                    GAME_SCENE_NAME,
                    LoadSceneMode.Single
                );
                Debug.Log("Escena de juego cargada correctamente.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error en StartHostAsync: {e}");
            }
        }

        // Getter del join code
        public string GetJoinCode()
        {
            return joinCode;
        }

        // Getter de allocation para debug o recreación
        public Allocation GetAllocation()
        {
            return allocation;
        }
    }
}