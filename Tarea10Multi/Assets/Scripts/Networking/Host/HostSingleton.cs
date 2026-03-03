using UnityEngine;
using System.Threading.Tasks;
using Networking.Host;

namespace Networking.Host
{
    public class HostSingleton : MonoBehaviour
    {
        private static HostSingleton instance;

        public static HostSingleton Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindAnyObjectByType<HostSingleton>();

                    if (instance == null)
                    {
                        Debug.LogError("No se encontró ninguna instancia de HostSingleton en la escena.");
                    }
                }

                return instance;
            }
        }

        public HostGameManager GameManager { get; private set; }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            GameManager = new HostGameManager();

        }


    }
}
