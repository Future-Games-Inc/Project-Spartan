using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class LoginManager4 : MonoBehaviourPunCallbacks
{
    public TMP_InputField playerNameInput;
    public RoomManager roomManager;
    public GameObject connectButton;

    const string playerNamePrefKey = "PlayerName";
    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        //if (!PhotonNetwork.IsConnectedAndReady)
        //{
        //    PhotonNetwork.ConnectUsingSettings();
        //}
    }

    // Update is called once per frame
    void Update()
    {

    }
    #endregion

    #region UI Callback Methods
    public void ConnectToPhotonServer()
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
        PhotonNetwork.NickName = defaultName;
        PhotonNetwork.JoinLobby();
    }

    #endregion

    #region Photon Callback Methods
    public override void OnJoinedLobby()
    {
        connectButton.SetActive(true);
    }

    public void EnterRoom1()
    {
        roomManager.OnEnterButtonClicked_Multiplayer5();
    }


    #endregion
}
