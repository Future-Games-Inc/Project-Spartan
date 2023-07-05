using UnityEngine;
using Photon.Pun;
using TMPro;

public class LoginManager5 : MonoBehaviourPunCallbacks
{
    public TMP_InputField playerNameInput;
    public RoomManager roomManager;
    public GameObject connectButton;

    const string playerNamePrefKey = "PlayerName";

    // Update is called once per frame
    void Update()
    {

    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    [System.Obsolete]
    public void EnterRoom1()
    {
        roomManager.OnEnterButtonClicked_Multiplayer6();
    }
}

