using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class LoginManager5 : MonoBehaviourPunCallbacks
{
    public TMP_InputField playerNameInput;
    public RoomManager roomManager;
    public GameObject connectButton;

    const string playerNamePrefKey = "PlayerName";
    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        //string defaultName = string.Empty;
        //if (playerNameInput != null)
        //{
        //    if (PlayerPrefs.HasKey(playerNamePrefKey))
        //    {
        //        defaultName = PlayerPrefs.GetString(playerNamePrefKey);
        //        playerNameInput.text = defaultName;
        //    }
        //}
        //connectButton.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }
    #endregion

    #region UI Callback Methods
    public void ConnectToPhotonServer()
    {
        //string defaultName = string.Empty;
        //if (playerNameInput != null)
        //{
        //    if (PlayerPrefs.HasKey(playerNamePrefKey))
        //    {
        //        defaultName = PlayerPrefs.GetString(playerNamePrefKey);
        //        playerNameInput.text = defaultName;
        //    }
        //}
        //PhotonNetwork.NickName = playerNameInput.text;
        //PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server with player name: " + PhotonNetwork.LocalPlayer.NickName);
        PhotonNetwork.JoinLobby();
    }

    #endregion
    public void EnterRoom1()
    {
        roomManager.OnEnterButtonClicked_Multiplayer6();
    }
}

