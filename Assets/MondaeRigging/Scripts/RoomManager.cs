using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using LootLocker.Requests;
using ExitGames.Client.Photon.StructWrapping;

public class RoomManager : MonoBehaviourPunCallbacks
{
    private string mapType;

    public TextMeshProUGUI occupancyMultiplayer1;
    public TextMeshProUGUI occupancyMultiplayer2;
    public TextMeshProUGUI occupancyMultiplayer3;
    public TextMeshProUGUI occupancyMultiplayer4;
    public TextMeshProUGUI occupancyMultiplayer5;

    public int mapLevel;
    public int levelDifference = 2;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region UI Callback Methods
    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnEnterButtonClicked_Multiplayer1()
    {
        //if (!PhotonNetwork.IsConnectedAndReady)
        //{
        //    PhotonNetwork.ConnectUsingSettings();
        //}
        //else
        //{
        //    PhotonNetwork.JoinLobby();
        //}
        mapType = MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER1;
        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.MAP_TYPE_KEY, mapType } };
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
    }

    public void OnEnterButtonClicked_Multiplayer2()
    {
        //if (!PhotonNetwork.IsConnectedAndReady)
        //{
        //    PhotonNetwork.ConnectUsingSettings();
        //}
        //else
        //{
        //    PhotonNetwork.JoinLobby();
        //}
        mapType = MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER2;
        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.MAP_TYPE_KEY, mapType } };
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
    }

    public void OnEnterButtonClicked_Multiplayer3()
    {
        //if (!PhotonNetwork.IsConnectedAndReady)
        //{
        //    PhotonNetwork.ConnectUsingSettings();
        //}
        //else
        //{
        //    PhotonNetwork.JoinLobby();
        //}
        mapType = MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER3;
        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.MAP_TYPE_KEY, mapType } };
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
    }

    public void OnEnterButtonClicked_Multiplayer4()
    {
        //if (!PhotonNetwork.IsConnectedAndReady)
        //{
        //    PhotonNetwork.ConnectUsingSettings();
        //}
        //else
        //{
        //    PhotonNetwork.JoinLobby();
        //}
        mapType = MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER4;
        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.MAP_TYPE_KEY, mapType } };
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
    }

    public void OnEnterButtonClicked_Multiplayer5()
    {
        //if (!PhotonNetwork.IsConnectedAndReady)
        //{
        //    PhotonNetwork.ConnectUsingSettings();
        //}
        //else
        //{
        //    PhotonNetwork.JoinLobby();
        //}
        mapType = MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER5;
        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.MAP_TYPE_KEY, mapType } };
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
    }

    #endregion

    #region Photon Callback Methods
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        CreateAndJoinRoom();
    }
    public override void OnCreatedRoom()
    {
        Debug.Log("A room is created with name: " + PhotonNetwork.CurrentRoom.Name);
        PhotonNetwork.CurrentRoom.IsOpen = true;
        PhotonNetwork.CurrentRoom.IsVisible = true;
    }

    public override void OnJoinedRoom()
    {
        int roomLevel = (int)PhotonNetwork.CurrentRoom.CustomProperties["PlayerLevelForRoom"];
        if (Mathf.Abs(roomLevel - levelDifference) >= mapLevel && Mathf.Abs(roomLevel + levelDifference) <= mapLevel)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            Debug.Log("The local player: " + PhotonNetwork.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name + ". Player Count " + PhotonNetwork.CurrentRoom.PlayerCount);
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(MultiplayerVRConstants.MAP_TYPE_KEY))
            {
                object mapType;
                if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(MultiplayerVRConstants.MAP_TYPE_KEY, out mapType))
                {
                    Debug.Log("Joined room with map: " + (string)mapType);
                    if ((string)mapType == MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER1)
                    {
                        PhotonNetwork.LoadLevel("1Multiplayer");
                    }
                    if ((string)mapType == MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER2)
                    {
                        PhotonNetwork.LoadLevel("2Multiplayer");
                    }
                    if ((string)mapType == MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER3)
                    {
                        PhotonNetwork.LoadLevel("3Multiplayer");
                    }
                    if ((string)mapType == MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER4)
                    {
                        PhotonNetwork.LoadLevel("4Multiplayer");
                    }
                    if ((string)mapType == MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER5)
                    {
                        PhotonNetwork.LoadLevel("5Multiplayer");
                    }
                }
            }
        }
    }

    public override void OnLeftRoom()
    {
        CreateAndJoinRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + "has joined. " + "Player Count" + PhotonNetwork.CurrentRoom.PlayerCount);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (roomList.Count == 0)
        {
            occupancyMultiplayer1.text = 0 + "/" + 20 + " Reacts Currently In This Session.";
            occupancyMultiplayer2.text = 0 + "/" + 20 + " Reacts Currently In This Session.";
            occupancyMultiplayer3.text = 0 + "/" + 20 + " Reacts Currently In This Session.";
            occupancyMultiplayer4.text = 0 + "/" + 20 + " Reacts Currently In This Session.";
            occupancyMultiplayer5.text = 0 + "/" + 20 + " Reacts Currently In This Session.";
        }

        foreach (RoomInfo room in roomList)
        {
            Debug.Log(room.Name);
            object roomLevel;
            if (room.Name.Contains(MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER1))
            {
                occupancyMultiplayer1.text = room.PlayerCount + "/" + 20 + " Reacts Currently In This Session. World level: " + PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("PlayerLevelForRoom", out roomLevel);
            }

            if (room.Name.Contains(MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER2))
            {
                occupancyMultiplayer2.text = room.PlayerCount + "/" + 20 + " Reacts Currently In This Session. World level: " + PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("PlayerLevelForRoom", out roomLevel);
            }

            if (room.Name.Contains(MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER3))
            {
                occupancyMultiplayer3.text = room.PlayerCount + "/" + 20 + " Reacts Currently In This Session. World level: " + PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("PlayerLevelForRoom", out roomLevel);
            }

            if (room.Name.Contains(MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER4))
            {
                occupancyMultiplayer4.text = room.PlayerCount + "/" + 20 + " Reacts Currently In This Session. World level: " + PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("PlayerLevelForRoom", out roomLevel);
            }

            if (room.Name.Contains(MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER5))
            {
                occupancyMultiplayer5.text = room.PlayerCount + "/" + 20 + " Reacts Currently In This Session. World level: " + PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("PlayerLevelForRoom", out roomLevel);
            }
        }
    }

    #endregion

    #region Private Methods
    private void CreateAndJoinRoom()
    {
        string randomRoomName = "Room " + mapType + " " + Random.Range(0, 10000);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.BroadcastPropsChangeToAll = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 20;

        //ExitGames.Client.Photon.Hashtable roomLevel = new ExitGames.Client.Photon.Hashtable() { { "PlayerLevelForRoom", mapLevel } };

        string[] roomPropsInLobby = { MultiplayerVRConstants.MAP_TYPE_KEY };

        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.MAP_TYPE_KEY, mapType }, { "PlayerLevelForRoom", mapLevel } };

        roomOptions.CustomRoomPropertiesForLobby = roomPropsInLobby;
        roomOptions.CustomRoomProperties = customRoomProperties;

        PhotonNetwork.CreateRoom(randomRoomName, roomOptions);
    }

    #endregion
}