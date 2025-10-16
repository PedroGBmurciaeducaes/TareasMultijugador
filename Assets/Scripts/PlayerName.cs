//using UnityEngine;
//using TMPro;
//using Photon.Pun;

//[RequireComponent(typeof(PhotonView))]
//public class PlayerName:MonoBehaviourPunCallbacks
//{
//    //Tenemos que tener referencia de ese texto
//    [SerializeField] TMP_Text playerName;

//    void Awake()
//    {
//        if (playerName == null)
//            playerName = GetComponentInChildren<TMP_Text>(true);
//    }

//    [PunRPC]
//    public void SetNameText(string name)
//    {
//        if (playerName == null || playerName.Equals(null)) return;
//        playerName.text = name;
//    }

//}

using UnityEngine;
using TMPro;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class PlayerName : MonoBehaviourPun
{
    [SerializeField] TMP_Text playerName;

    void Awake()
    {
        if (playerName == null)
            playerName = GetComponentInChildren<TMP_Text>(true);
    }

    void Start()
    {
        // SIEMPRE pisa el texto con el Nick del dueño (evita quedarse con el default)
        var nick = (photonView.Owner != null && !string.IsNullOrEmpty(photonView.Owner.NickName))
            ? photonView.Owner.NickName
            : PlayerPrefs.GetString("PlayerName", PhotonNetwork.NickName ?? "Player");

        if (playerName == null || playerName.Equals(null))
            playerName = GetComponentInChildren<TMP_Text>(true);

        if (playerName != null && !playerName.Equals(null))
            playerName.text = nick;
    }

    [PunRPC]
    public void SetNameText(string name)   // firma simple; compatible con tu PUN
    {
        if (playerName == null || playerName.Equals(null))
            playerName = GetComponentInChildren<TMP_Text>(true);

        if (playerName != null && !playerName.Equals(null))
            playerName.text = name;
    }
}

