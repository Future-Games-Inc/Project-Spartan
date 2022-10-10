using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class LoginManager2 : MonoBehaviourPunCallbacks
{
    public TMP_InputField playerNameInput;
    public RoomManager roomManager;
    public GameObject connectButton;

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
    public void ConnectAnonymously1()
    {
        roomManager.OnEnterButtonClicked_Multiplayer1();
    }

    public void ConnectToPhotonServer()
    {
        if (playerNameInput != null)
        {
            PhotonNetwork.NickName = playerNameInput.text;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    #endregion

    #region Photon Callback Methods
    public override void OnConnected()
    {
        Debug.Log("OnConnectred called. Server available.");
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Conencted to Master Server with player name: " + PhotonNetwork.NickName);
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        connectButton.SetActive(true);
    }

    public void EnterRoom1()
    {
        roomManager.OnEnterButtonClicked_Multiplayer3();
    }


    #endregion
}
