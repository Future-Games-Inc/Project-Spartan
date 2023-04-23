using Photon.Pun;
using UnityEngine;
using TMPro;

public class ConnectToPhoton : MonoBehaviourPunCallbacks
{
    public TMP_InputField playerNameInput;

    const string playerNamePrefKey = "PlayerName";
    // Start is called before the first frame update
    void Start()
    {
        string defaultName = string.Empty;
        if (playerNameInput != null)
        {
            if (PlayerPrefs.HasKey(playerNamePrefKey))
            {
                defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                playerNameInput.text = defaultName;
            }
        }
    }

    public void ConnectPlayer()
    {
        string defaultName = string.Empty;
        if (playerNameInput != null)
        {
            if (PlayerPrefs.HasKey(playerNamePrefKey))
            {
                defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                playerNameInput.text = defaultName;
            }
        }
        PhotonNetwork.NickName = playerNameInput.text;
        PhotonNetwork.ConnectUsingSettings();
    }

    public void DisconnectPlayer()
    {
        PhotonNetwork.Disconnect();
    }
}
