using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using LootLocker.Requests;

public class RoomManager : MonoBehaviourPunCallbacks
{
    private string mapType;

    public TextMeshProUGUI occupancyMultiplayer1;
    public TextMeshProUGUI occupancyMultiplayer2;
    public TextMeshProUGUI occupancyMultiplayer3;
    public TextMeshProUGUI occupancyMultiplayer4;
    public TextMeshProUGUI occupancyMultiplayer5;
    public TextMeshProUGUI occupancyMultiplayer6;

    public TextMeshProUGUI joinAsInfo;

    public int mapLevel;

    public string randomRoomName;

    public List<string> roomName = new List<string>();

    // Start is called before the first frame update
    [System.Obsolete]
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        LootLockerSDKManager.GetXpAndLevel((response) =>
        {
            mapLevel = (int)response.level;
        });
    }

    // Update is called once per frame
    void Update()
    {
        joinAsInfo.text = "Joining Room With Alias: " + PhotonNetwork.NickName;
    }

    #region UI Callback Methods
    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnEnterButtonClicked_Multiplayer1()
    {
        mapType = MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER1;
        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.MAP_TYPE_KEY, mapType } };
        if (mapLevel >= 0 && mapLevel <= 2 && roomName.Contains("beginnerRoom"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else if (mapLevel > 2 && mapLevel <= 5 && roomName.Contains("easyRoom"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else if (mapLevel > 5 && mapLevel <= 8 && roomName.Contains("mediumRoom"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else if (mapLevel > 8 && mapLevel <= 10 && roomName.Contains("hardRoom"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
    }

    public void OnEnterButtonClicked_Multiplayer2()
    {
        mapType = MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER2;
        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.MAP_TYPE_KEY, mapType } };
        if (mapLevel >= 0 && mapLevel <= 2 && roomName.Contains("beginnerRoom"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else if (mapLevel > 2 && mapLevel <= 5 && roomName.Contains("easyRoom"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else if (mapLevel > 5 && mapLevel <= 8 && roomName.Contains("mediumRoom"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else if (mapLevel > 8 && mapLevel <= 10 && roomName.Contains("hardRoom"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
    }

    public void OnEnterButtonClicked_Multiplayer3()
    {
        mapType = MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER3;
        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.MAP_TYPE_KEY, mapType } };
        if (mapLevel >= 0 && mapLevel <= 2 && roomName.Contains("beginnerRoom"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else if (mapLevel > 2 && mapLevel <= 5 && roomName.Contains("easyRoom"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else if (mapLevel > 5 && mapLevel <= 8 && roomName.Contains("mediumRoom"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else if (mapLevel > 8 && mapLevel <= 10 && roomName.Contains("hardRoom"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
    }

    public void OnEnterButtonClicked_Multiplayer4()
    {
        mapType = MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER4;
        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.MAP_TYPE_KEY, mapType } };
        if (mapLevel >= 0 && mapLevel <= 2 && roomName.Contains("beginnerRoom"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else if (mapLevel > 2 && mapLevel <= 5 && roomName.Contains("easyRoom"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else if (mapLevel > 5 && mapLevel <= 8 && roomName.Contains("mediumRoom"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else if (mapLevel > 8 && mapLevel <= 10 && roomName.Contains("hardRoom"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
    }

    public void OnEnterButtonClicked_Multiplayer5()
    {
        mapType = MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER5;
        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.MAP_TYPE_KEY, mapType } };
        if (mapLevel >= 0 && mapLevel <= 2 && roomName.Contains("beginnerRoom"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else if (mapLevel > 2 && mapLevel <= 5 && roomName.Contains("easyRoom"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else if (mapLevel > 5 && mapLevel <= 8 && roomName.Contains("mediumRoom"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else if (mapLevel > 8 && mapLevel <= 10 && roomName.Contains("hardRoom"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
    }
    public void OnEnterButtonClicked_Multiplayer6()
    {
        mapType = MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER6;
        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.MAP_TYPE_KEY, mapType } };
        if (mapLevel >= 0 && mapLevel <= 2 && roomName.Contains("beginnerRoom"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else if (mapLevel > 2 && mapLevel <= 5 && roomName.Contains("easyRoom"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else if (mapLevel > 5 && mapLevel <= 8 && roomName.Contains("mediumRoom"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else if (mapLevel > 8 && mapLevel <= 10 && roomName.Contains("hardRoom"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else
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
        object roomLevel;
        PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("PlayerLevelForRoom", out roomLevel);
        Debug.Log("A room is created with name: " + PhotonNetwork.CurrentRoom.Name + " Room level: " + (int)roomLevel);
        PhotonNetwork.CurrentRoom.IsOpen = true;
        PhotonNetwork.CurrentRoom.IsVisible = true;
    }

    public override void OnJoinedRoom()
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
                else if ((string)mapType == MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER2)
                {
                    PhotonNetwork.LoadLevel("2Multiplayer");
                }
                else if ((string)mapType == MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER3)
                {
                    PhotonNetwork.LoadLevel("3Multiplayer");
                }
                else if ((string)mapType == MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER4)
                {
                    PhotonNetwork.LoadLevel("4Multiplayer");
                }
                else if ((string)mapType == MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER5)
                {
                    PhotonNetwork.LoadLevel("5Multiplayer");
                }
                else if ((string)mapType == MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER6)
                {
                    PhotonNetwork.LoadLevel("6Multiplayer");
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
        foreach (RoomInfo room in roomList)
        {
            object roomLevelSet;
            room.CustomProperties.TryGetValue("PlayerLevelForRoom", out roomLevelSet);
            int roomLevel = (int)roomLevelSet;
            roomName.Add(randomRoomName);
            if (roomLevel >= 0 && roomLevel <= 2)
            {
                Debug.Log(room.Name);
                if (room.Name.Contains(MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER1))
                {
                    occupancyMultiplayer1.text = room.PlayerCount + "/" + 10 + " Reacts Currently In This Session. World Level: " + roomLevel;
                }

                else if (room.Name.Contains(MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER2))
                {
                    occupancyMultiplayer2.text = room.PlayerCount + "/" + 10 + " Reacts Currently In This Session. World Level: " + roomLevel;
                }

                else if (room.Name.Contains(MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER3))
                {
                    occupancyMultiplayer3.text = room.PlayerCount + "/" + 10 + " Reacts Currently In This Session. World Level: " + roomLevel;
                }

                else if (room.Name.Contains(MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER4))
                {
                    occupancyMultiplayer4.text = room.PlayerCount + "/" + 10 + " Reacts Currently In This Session. World Level: " + roomLevel;
                }

                else if (room.Name.Contains(MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER5))
                {
                    occupancyMultiplayer5.text = room.PlayerCount + "/" + 10 + " Reacts Currently In This Session. World Level: " + roomLevel;
                }

                else if (room.Name.Contains(MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER6))
                {
                    occupancyMultiplayer6.text = room.PlayerCount + "/" + 10 + " Reacts Currently In This Session. World Level: " + roomLevel;
                }

            }

            else if (roomLevel > 2 && roomLevel <= 5)
            {
                Debug.Log(room.Name);
                if (room.Name.Contains(MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER1))
                {
                    occupancyMultiplayer1.text = room.PlayerCount + "/" + 10 + " Reacts Currently In This Session. World Level: " + roomLevel;
                }

                else if (room.Name.Contains(MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER2))
                {
                    occupancyMultiplayer2.text = room.PlayerCount + "/" + 10 + " Reacts Currently In This Session. World Level: " + roomLevel;
                }

                else if (room.Name.Contains(MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER3))
                {
                    occupancyMultiplayer3.text = room.PlayerCount + "/" + 10 + " Reacts Currently In This Session. World Level: " + roomLevel;
                }

                else if (room.Name.Contains(MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER4))
                {
                    occupancyMultiplayer4.text = room.PlayerCount + "/" + 10 + " Reacts Currently In This Session. World Level: " + roomLevel;
                }

                else if (room.Name.Contains(MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER5))
                {
                    occupancyMultiplayer5.text = room.PlayerCount + "/" + 10 + " Reacts Currently In This Session. World Level: " + roomLevel;
                }

                else if (room.Name.Contains(MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER6))
                {
                    occupancyMultiplayer6.text = room.PlayerCount + "/" + 10 + " Reacts Currently In This Session. World Level: " + roomLevel;
                }

            }

            else if (roomLevel > 5 && roomLevel <= 8)
            {
                Debug.Log(room.Name);
                if (room.Name.Contains(MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER1))
                {
                    occupancyMultiplayer1.text = room.PlayerCount + "/" + 10 + " Reacts Currently In This Session. World Level: " + roomLevel;
                }

                else if (room.Name.Contains(MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER2))
                {
                    occupancyMultiplayer2.text = room.PlayerCount + "/" + 10 + " Reacts Currently In This Session. World Level: " + roomLevel;
                }

                else if (room.Name.Contains(MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER3))
                {
                    occupancyMultiplayer3.text = room.PlayerCount + "/" + 10 + " Reacts Currently In This Session. World Level: " + roomLevel;
                }

                else if (room.Name.Contains(MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER4))
                {
                    occupancyMultiplayer4.text = room.PlayerCount + "/" + 10 + " Reacts Currently In This Session. World Level: " + roomLevel;
                }

                else if (room.Name.Contains(MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER5))
                {
                    occupancyMultiplayer5.text = room.PlayerCount + "/" + 10 + " Reacts Currently In This Session. World Level: " + roomLevel;
                }

                else if (room.Name.Contains(MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER6))
                {
                    occupancyMultiplayer6.text = room.PlayerCount + "/" + 10 + " Reacts Currently In This Session. World Level: " + roomLevel;
                }

            }

            else if (roomLevel > 8 && roomLevel <= 10)
            {
                Debug.Log(room.Name);
                if (room.Name.Contains(MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER1))
                {
                    occupancyMultiplayer1.text = room.PlayerCount + "/" + 10 + " Reacts Currently In This Session. World Level: " + roomLevel;
                }

                else if (room.Name.Contains(MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER2))
                {
                    occupancyMultiplayer2.text = room.PlayerCount + "/" + 10 + " Reacts Currently In This Session. World Level: " + roomLevel;
                }

                else if (room.Name.Contains(MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER3))
                {
                    occupancyMultiplayer3.text = room.PlayerCount + "/" + 10 + " Reacts Currently In This Session. World Level: " + roomLevel;
                }

                else if (room.Name.Contains(MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER4))
                {
                    occupancyMultiplayer4.text = room.PlayerCount + "/" + 10 + " Reacts Currently In This Session. World Level: " + roomLevel;
                }

                else if (room.Name.Contains(MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER5))
                {
                    occupancyMultiplayer5.text = room.PlayerCount + "/" + 10 + " Reacts Currently In This Session. World Level: " + roomLevel;
                }

                else if (room.Name.Contains(MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER6))
                {
                    occupancyMultiplayer6.text = room.PlayerCount + "/" + 10 + " Reacts Currently In This Session. World Level: " + roomLevel;
                }
            }

        }
    }

    #endregion

    #region Private Methods
    private void CreateAndJoinRoom()
    {
        if (mapLevel >= 0 && mapLevel <= 2)
        {
            randomRoomName = "beginnerRoom " + mapType + " " + Random.Range(0, 10000) + " " + mapLevel;
        }
        else if ((mapLevel > 2 && mapLevel <= 5))
        {
            randomRoomName = "easyRoom " + mapType + " " + Random.Range(0, 10000) + " " + mapLevel;
        }
        else if ((mapLevel > 5 && mapLevel <= 8))
        {
            randomRoomName = "mediumRoom " + mapType + " " + Random.Range(0, 10000) + " " + mapLevel;
        }
        else if ((mapLevel > 8 && mapLevel <= 10))
        {
            randomRoomName = "hardRoom " + mapType + " " + Random.Range(0, 10000) + " " + mapLevel;
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.BroadcastPropsChangeToAll = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 10;

        string[] roomPropsInLobby = { MultiplayerVRConstants.MAP_TYPE_KEY, "PlayerLevelForRoom" };

        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.MAP_TYPE_KEY, mapType }, { "PlayerLevelForRoom", mapLevel } };
        roomOptions.CustomRoomPropertiesForLobby = roomPropsInLobby;
        roomOptions.CustomRoomProperties = customRoomProperties;

        PhotonNetwork.CreateRoom(randomRoomName, roomOptions);
    }

    #endregion
}