using UnityEngine;
using Unity.Netcode;
public class joinServer : MonoBehaviour
{
    public void Join()
    {
        NetworkManager.Singleton.StartClient();
    }
}
