using UnityEngine;
using Unity.Netcode;

public class joinServer : MonoBehaviour
{
    public void StartAsHost()
    {
        NetworkManager.Singleton.StartHost();
        Debug.Log("Started as Host (Server + Client).");
    }

    public void StartAsClient()
    {
        NetworkManager.Singleton.StartClient();
        Debug.Log("Started as Client.");
    }
}
