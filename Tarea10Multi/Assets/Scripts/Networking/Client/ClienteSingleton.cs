using UnityEngine;
using System.Threading.Tasks;
using Networking.Client;

namespace Networking.Client
{
    public class ClienteSingleton : MonoBehaviour
    {
        private static ClienteSingleton instance;

        public static ClienteSingleton Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindAnyObjectByType<ClienteSingleton>();

                    if (instance == null)
                    {
                        Debug.LogError("No se encontró ninguna instancia de ClienteSingleton en la escena.");
                    }
                }

                return instance;
            }
        }

        public ClienteGameManager GameManager { get; private set; }
        private void Awake()
        {
            // Si ya existe una instancia y no somos nosotros -> destruir
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }


        private async void Start()
        {
            await InitAsync();
        }



        public async Task InitAsync()
        {
            Debug.Log("Inicializando ClienteSingleton...");

            GameManager = new ClienteGameManager();
            await GameManager.InitAsync();
            Debug.Log("ClienteSingleton inicializado correctamente.");
        }
    }
}
