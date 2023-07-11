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

    public void EnterRoom1()
    {
        roomManager.OnEnterButtonClicked_Playground();
    }

    public void EnterRoom2()
    {
        roomManager.OnEnterButtonClicked_DropZone1();
    }

    public void EnterRoom3()
    {
        roomManager.OnEnterButtonClicked_DropZone2();
    }

    public void EnterRoom4()
    {
        roomManager.OnEnterButtonClicked_DropZone3();
    }
}

