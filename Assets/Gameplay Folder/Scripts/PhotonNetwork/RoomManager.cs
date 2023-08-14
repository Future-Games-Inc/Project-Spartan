using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using LootLocker.Requests;
using System.Collections;

public class RoomManager : MonoBehaviourPunCallbacks
{
    private string mapType;

    public TextMeshProUGUI joinAsInfo;

    public int mapLevel;

    public string randomRoomName;

    public List<string> roomName = new List<string>();


    // Start is called before the first frame update
    [System.Obsolete]
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        StartCoroutine(MapLevel());
    }

    IEnumerator MapLevel()
    {
        while (true)
        {
            bool done = false;
            LootLockerSDKManager.GetPlayerInfo((response) =>
                {
                    if (response.success)
                    {
                        mapLevel = (int)response.level;
                        done = true;
                    }
                });
            yield return new WaitWhile(() => done == false);
            StopCoroutine(MapLevel());
        }
    }

    // Update is called once per frame
    void Update()
    {
        joinAsInfo.text = "Joining Under REACT Alias: " + PhotonNetwork.NickName;
    }

    #region UI Callback Methods
    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnEnterButtonClicked_Playground()
    {
        LootLockerSDKManager.GetPlayerInfo((response) =>
        {
            mapLevel = (int)response.level;
        });
        mapType = MultiplayerVRConstants.MAP_TYPE_VALUE_PLAYGROUND;
        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.MAP_TYPE_KEY, mapType } };
        if (mapLevel >= 0 && mapLevel <= 10 && roomName.Contains("Low"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else if (mapLevel > 10 && mapLevel <= 30 && roomName.Contains("Normal"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else if (mapLevel > 30 && mapLevel <= 50 && roomName.Contains("Medium"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else if (mapLevel > 50 && mapLevel <= 70 && roomName.Contains("High"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else if (mapLevel > 70 && mapLevel <= 90 && roomName.Contains("Reinforced"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else if (mapLevel > 90 && mapLevel <= 110 && roomName.Contains("Chokehold"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
    }

    public void OnEnterButtonClicked_DropZone1()
    {
        LootLockerSDKManager.GetPlayerInfo((response) =>
        {
            mapLevel = (int)response.level;
        });
        mapType = MultiplayerVRConstants.MAP_TYPE_VALUE_DROPZONE1;
        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.MAP_TYPE_KEY, mapType } };
        if (mapLevel >= 0 && mapLevel <= 10 && roomName.Contains("Low"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else if (mapLevel > 10 && mapLevel <= 30 && roomName.Contains("Normal"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else if (mapLevel > 30 && mapLevel <= 50 && roomName.Contains("Medium"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else if (mapLevel > 50 && mapLevel <= 70 && roomName.Contains("High"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else if (mapLevel > 70 && mapLevel <= 90 && roomName.Contains("Reinforced"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else if (mapLevel > 90 && mapLevel <= 110 && roomName.Contains("Chokehold"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
    }
    public void OnEnterButtonClicked_DropZone2()
    {
        LootLockerSDKManager.GetPlayerInfo((response) =>
        {
            mapLevel = (int)response.level;
        });
        mapType = MultiplayerVRConstants.MAP_TYPE_VALUE_DROPZONE2;
        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.MAP_TYPE_KEY, mapType } };
        if (mapLevel >= 0 && mapLevel <= 10 && roomName.Contains("Low"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else if (mapLevel > 10 && mapLevel <= 30 && roomName.Contains("Normal"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else if (mapLevel > 30 && mapLevel <= 50 && roomName.Contains("Medium"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else if (mapLevel > 50 && mapLevel <= 70 && roomName.Contains("High"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else if (mapLevel > 70 && mapLevel <= 90 && roomName.Contains("Reinforced"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else if (mapLevel > 90 && mapLevel <= 110 && roomName.Contains("Chokehold"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
    }

    public void OnEnterButtonClicked_DropZone3()
    {
        LootLockerSDKManager.GetPlayerInfo((response) =>
        {
            mapLevel = (int)response.level;
        });
        mapType = MultiplayerVRConstants.MAP_TYPE_VALUE_DROPZONE3;
        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.MAP_TYPE_KEY, mapType } };
        if (mapLevel >= 0 && mapLevel <= 10 && roomName.Contains("Low"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else if (mapLevel > 10 && mapLevel <= 30 && roomName.Contains("Normal"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else if (mapLevel > 30 && mapLevel <= 50 && roomName.Contains("Medium"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else if (mapLevel > 50 && mapLevel <= 70 && roomName.Contains("High"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else if (mapLevel > 70 && mapLevel <= 90 && roomName.Contains("Reinforced"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else if (mapLevel > 90 && mapLevel <= 110 && roomName.Contains("Chokehold"))
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        else
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
    }


    #endregion

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CreateAndJoinRoom();
    }
    public override void OnCreatedRoom()
    {
        object roomLevel;
        PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("PlayerLevelForRoom", out roomLevel);
        PhotonNetwork.CurrentRoom.IsOpen = true;
        PhotonNetwork.CurrentRoom.IsVisible = true;
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(MultiplayerVRConstants.MAP_TYPE_KEY))
        {
            object mapType;
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(MultiplayerVRConstants.MAP_TYPE_KEY, out mapType))
            {
                if ((string)mapType == MultiplayerVRConstants.MAP_TYPE_VALUE_PLAYGROUND)
                {
                    PhotonNetwork.LoadLevel("WeaponTest");
                }
                else if ((string)mapType == MultiplayerVRConstants.MAP_TYPE_VALUE_DROPZONE1)
                {
                    PhotonNetwork.LoadLevel("DZ1");
                }
                else if ((string)mapType == MultiplayerVRConstants.MAP_TYPE_VALUE_DROPZONE2)
                {
                    PhotonNetwork.LoadLevel("DZ2");
                }
                else if ((string)mapType == MultiplayerVRConstants.MAP_TYPE_VALUE_DROPZONE3)
                {
                    PhotonNetwork.LoadLevel("DZ3");
                }
            }
        }
    }

    public override void OnLeftRoom()
    {
        CreateAndJoinRoom();
    }

    private readonly string[] MAP_VALUES = { MultiplayerVRConstants.MAP_TYPE_VALUE_PLAYGROUND, MultiplayerVRConstants.MAP_TYPE_VALUE_DROPZONE1, MultiplayerVRConstants.MAP_TYPE_VALUE_DROPZONE2, MultiplayerVRConstants.MAP_TYPE_VALUE_DROPZONE3 };

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        {
            object roomLevelSet;
            room.CustomProperties.TryGetValue("PlayerLevelForRoom", out roomLevelSet);
            int roomLevel = (int)roomLevelSet;
            roomName.Add(randomRoomName);
        }
    }

    private void CreateAndJoinRoom()
    {
        if (mapLevel >= 0 && mapLevel <= 10)
        {
            randomRoomName = "Low " + mapType + " " + Random.Range(0, 10000) + " " + mapLevel;
        }
        else if ((mapLevel > 10 && mapLevel <= 30))
        {
            randomRoomName = "Normal " + mapType + " " + Random.Range(0, 10000) + " " + mapLevel;
        }
        else if ((mapLevel > 30 && mapLevel <= 50))
        {
            randomRoomName = "Medium " + mapType + " " + Random.Range(0, 10000) + " " + mapLevel;
        }
        else if ((mapLevel > 50 && mapLevel <= 70))
        {
            randomRoomName = "High " + mapType + " " + Random.Range(0, 10000) + " " + mapLevel;
        }
        else if ((mapLevel > 70 && mapLevel <= 90))
        {
            randomRoomName = "Reinforced " + mapType + " " + Random.Range(0, 10000) + " " + mapLevel;
        }
        else if ((mapLevel > 90 && mapLevel <= 110))
        {
            randomRoomName = "Chockehold " + mapType + " " + Random.Range(0, 10000) + " " + mapLevel;
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.BroadcastPropsChangeToAll = true;
        roomOptions.IsVisible = true;
        if ((string)mapType == MultiplayerVRConstants.MAP_TYPE_VALUE_PLAYGROUND)
            roomOptions.MaxPlayers = 10;
        else if ((string)mapType == MultiplayerVRConstants.MAP_TYPE_VALUE_DROPZONE1)
            roomOptions.MaxPlayers = 10;
        else if ((string)mapType == MultiplayerVRConstants.MAP_TYPE_VALUE_DROPZONE2)
            roomOptions.MaxPlayers = 10;
        else if ((string)mapType == MultiplayerVRConstants.MAP_TYPE_VALUE_DROPZONE3)
            roomOptions.MaxPlayers = 5;

        string[] roomPropsInLobby = { MultiplayerVRConstants.MAP_TYPE_KEY, "PlayerLevelForRoom" };

        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.MAP_TYPE_KEY, mapType }, { "PlayerLevelForRoom", mapLevel } };
        roomOptions.CustomRoomPropertiesForLobby = roomPropsInLobby;
        roomOptions.CustomRoomProperties = customRoomProperties;

        PhotonNetwork.CreateRoom(randomRoomName, roomOptions);
    }
}