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
        StartCoroutine(MapLevel());
    }

    [System.Obsolete]
    IEnumerator MapLevel()
    {
        while (true)
        {
            bool done = false;
            LootLockerSDKManager.GetXpAndLevel((response) =>
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
        joinAsInfo.text = "Joining Room With Tag: " + PhotonNetwork.NickName;
    }

    #region UI Callback Methods
    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    [System.Obsolete]
    public void OnEnterButtonClicked_Multiplayer6()
    {
        LootLockerSDKManager.GetXpAndLevel((response) =>
        {
            mapLevel = (int)response.level;
        });
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
                if ((string)mapType == MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER6)
                {
                    PhotonNetwork.LoadLevel("Playground");
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

    }

    private readonly string[] MAP_VALUES = {MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER6 };

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        {
            object roomLevelSet;
            room.CustomProperties.TryGetValue("PlayerLevelForRoom", out roomLevelSet);
            int roomLevel = (int)roomLevelSet;
            roomName.Add(randomRoomName);
            UpdateOccupancyText(room, roomLevel);
        }
    }

    private void UpdateOccupancyText(RoomInfo room, int roomLevel)
    {
        string occupancyText = room.PlayerCount + "/" + 10 + " Reacts Currently In This Session. World Level: " + roomLevel;

        switch (roomLevel)
        {
            case int n when (n >= 0 && n <= 2):
                for (int i = 0; i < MAP_VALUES.Length; i++)
                {
                    if (room.Name.Contains(MAP_VALUES[i]))
                    {
                        switch (i)
                        {
                            case 5:
                                occupancyMultiplayer6.text = occupancyText;
                                break;
                        }
                    }
                }
                break;

            case int n when (n > 2 && n <= 5):
                for (int i = 0; i < MAP_VALUES.Length; i++)
                {
                    if (room.Name.Contains(MAP_VALUES[i]))
                    {
                        switch (i)
                        {
                            case 5:
                                occupancyMultiplayer6.text = occupancyText;
                                break;
                        }
                    }
                }
                break;

            case int n when (n > 5 && n <= 8):
                for (int i = 0; i < MAP_VALUES.Length; i++)
                {
                    if (room.Name.Contains(MAP_VALUES[i]))
                    {
                        switch (i)
                        {
                            case 5:
                                occupancyMultiplayer6.text = occupancyText;
                                break;
                        }
                    }
                }
                break;
            case int n when (n > 8):
                for (int i = 0; i < MAP_VALUES.Length; i++)
                {
                    if (room.Name.Contains(MAP_VALUES[i]))
                    {
                        switch (i)
                        {
                            case 5:
                                occupancyMultiplayer6.text = occupancyText;
                                break;
                        }
                    }
                }
                break;
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
        if ((string)mapType == MultiplayerVRConstants.MAP_TYPE_VALUE_MULTIPLAYER6)
            roomOptions.MaxPlayers = 10;

        string[] roomPropsInLobby = { MultiplayerVRConstants.MAP_TYPE_KEY, "PlayerLevelForRoom" };

        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.MAP_TYPE_KEY, mapType }, { "PlayerLevelForRoom", mapLevel } };
        roomOptions.CustomRoomPropertiesForLobby = roomPropsInLobby;
        roomOptions.CustomRoomProperties = customRoomProperties;

        PhotonNetwork.CreateRoom(randomRoomName, roomOptions);
    }

    #endregion
}