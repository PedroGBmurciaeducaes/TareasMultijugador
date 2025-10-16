using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class MainMenuLauncher : MonoBehaviourPunCallbacks
{
    [SerializeField] public TMP_InputField usernameInput;
    [SerializeField] public TMP_Text buttonText;
    bool isConnecting;

    public void OnClickConnect()
    {
        Debug.Log($"Player name set to {usernameInput.text}");
        if (usernameInput.text.Length >= 3)
        {
            var name = usernameInput != null ? usernameInput.text : "";
            if (name.Length < 3 || isConnecting) { if (buttonText) buttonText.text = "Name too short!"; return; }

            isConnecting = true;
            if (buttonText) buttonText.text = "Connecting...";
            PhotonNetwork.NickName = name;
            PlayerPrefs.SetString("PlayerName", name);

            PhotonNetwork.GameVersion = "1";

            if (!PhotonNetwork.IsConnected)
                PhotonNetwork.ConnectUsingSettings();  // SOLO aquí
        }
        else
        {
            buttonText.text = "Name too short!";
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        SceneManager.LoadScene("EscenaMultijugador");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        isConnecting = false;
        if (buttonText) buttonText.text = "Conectar";
        Debug.LogWarning($"Disconnected: {cause}");
    }

}

