using UnityEngine;
using UnityEngine.Rendering;
using Networking.Client;
using Networking.Host;

namespace Networking
{
    public class ApplicationController : MonoBehaviour
    {
        [SerializeField] private ClienteSingleton clientPrefab;
        [SerializeField] private HostSingleton hostPrefab;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            InitializeApplication();
        }

        private void InitializeApplication()
        {
            if (IsDedicatedServer())
            {
                Debug.Log("Modo Servidor Dedicado detectado.");
                // Aquí instanciaríamos ServerSingleton (más adelante)
            }
            else
            {
                Debug.Log("Modo Cliente/Host detectado.");

                // Instanciamos ambos prefabs
                Instantiate(clientPrefab);
                Instantiate(hostPrefab);
            }
        }

        private bool IsDedicatedServer()
        {
            return SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;
        }
    }
}
