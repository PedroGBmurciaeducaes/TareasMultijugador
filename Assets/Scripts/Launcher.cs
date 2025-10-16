using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;



public class Launcher : MonoBehaviourPunCallbacks

{
    [SerializeField] PhotonView playerPrefab;
    [SerializeField] PhotonView playerPrefab2;

    [SerializeField] Transform spawnPoint;
    [SerializeField] Transform spawnPoint2;

    //public TMP_InputField PlayerName1;
    //public TMP_InputField PlayerName2;

    bool joinAfterConnect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ////PhotonNetwork.ConnectUsingSettings();
        //if (!PhotonNetwork.IsConnected)
        //{
        //    SceneManager.LoadScene("MainMenu");
        //    return;
        //}

        //PhotonNetwork.JoinRandomOrCreateRoom(roomOptions: new RoomOptions { MaxPlayers = 4 });

        // Nick por si abres esta escena directamente
        if (string.IsNullOrEmpty(PhotonNetwork.NickName))
            PhotonNetwork.NickName = PlayerPrefs.GetString("PlayerName", $"Player{Random.Range(1000, 9999)}");

        // 1) Si ya estás conectado → entrar/crear sala
        if (PhotonNetwork.IsConnected)
        {
            JoinOrCreate();
            return;
        }

        // 2) Si estás totalmente desconectado → conecta ahora
        if (PhotonNetwork.NetworkClientState == ClientState.Disconnected)
        {
            PhotonNetwork.GameVersion = "1";
            joinAfterConnect = true;
            PhotonNetwork.ConnectUsingSettings();
            return;
        }

        // 3) Si estás "conectando" (viniendo del menú) → espera callback
        joinAfterConnect = true; // OnConnectedToMaster llamará a JoinOrCreate()
    }

    public override void OnConnectedToMaster()
    {
        if (joinAfterConnect)
            JoinOrCreate();
    }

    void JoinOrCreate()
    {
        PhotonNetwork.JoinRandomOrCreateRoom(roomOptions: new RoomOptions { MaxPlayers = 10 });
    }



    public override void OnJoinedRoom()
    {
        //Debug.Log("Joined a room.");

        //int actor = PhotonNetwork.LocalPlayer.ActorNumber;

        //if (actor == 1)
        //{
        //    GameObject player=PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
        //    player.GetComponent<PhotonView>().RPC("SetNameText", RpcTarget.AllBuffered, PlayerPrefs.GetString("PlayerName"));
        //    //player.GetComponent<PhotonView>().RPC("SetNameText", RpcTarget.AllBuffered, PlayerName1);
        //}


        //else if (actor != 1)
        //{
        //    GameObject playern = PhotonNetwork.Instantiate(playerPrefab2.name, spawnPoint2.position, spawnPoint2.rotation);
        //    playern.GetComponent<PhotonView>().RPC("SetNameText", RpcTarget.AllBuffered, PlayerPrefs.GetString("PlayerName"));
        //    //playern.GetComponent<PhotonView>().RPC("SetNameText", RpcTarget.AllBuffered, PlayerName2);
        //}

        Debug.Log("Joined a room.");

        int actor = PhotonNetwork.LocalPlayer.ActorNumber;

        string prefabName = (actor == 1) ? playerPrefab.gameObject.name : playerPrefab2.gameObject.name;
        var pos = (actor == 1) ? spawnPoint.position : spawnPoint2.position;
        var rot = (actor == 1) ? spawnPoint.rotation : spawnPoint2.rotation;

        var go = PhotonNetwork.Instantiate(prefabName, pos, rot);
        var pv = go.GetComponent<PhotonView>();
        var nick = string.IsNullOrEmpty(PhotonNetwork.NickName)
                   ? PlayerPrefs.GetString("PlayerName", "Player")
                   : PhotonNetwork.NickName;

        if (pv != null && pv.IsMine)
        {
            // actualiza inmediatamente local
            go.GetComponent<PlayerName>()?.SetNameText(nick);

            // y sincroniza para todos / late-joiners
            pv.RPC("SetNameText", RpcTarget.AllBufferedViaServer, nick);
        }

    }

    void OnDestroy()
    {
        if (photonView != null && photonView.IsMine)
            PhotonNetwork.RemoveRPCs(photonView);
    }


}













