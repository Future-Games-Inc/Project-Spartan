using System.Collections.Generic;
using UnityEngine;
using TMPro;
using LootLocker.Requests;
using System.Collections;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    private string mapType;

    public TextMeshProUGUI joinAsInfo;

    public int mapLevel;

    public string randomRoomName;

    public List<string> roomName = new List<string>();
    const string playerNamePrefKey = "PlayerName";


    // Start is called before the first frame update
    [System.Obsolete]
    void Start()
    {
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
        joinAsInfo.text = "Joining Under REACT Alias: " + PlayerPrefs.GetString(playerNamePrefKey);
    }


    public void OnEnterButtonClicked_Playground()
    {
        LootLockerSDKManager.GetPlayerInfo((response) =>
        {
            mapLevel = (int)response.level;
        });

        SceneManager.LoadScene("Playground");
    }



    public void OnEnterButtonClicked_Bear()
    {
        LootLockerSDKManager.GetPlayerInfo((response) =>
        {
            mapLevel = (int)response.level;
        });

        SceneManager.LoadScene("Bear");
    }

    public void OnEnterButtonClicked_DropZone1()
    {
        LootLockerSDKManager.GetPlayerInfo((response) =>
        {
            mapLevel = (int)response.level;
        });

        SceneManager.LoadScene("DZ1");
    }
    public void OnEnterButtonClicked_DropZone2()
    {
        LootLockerSDKManager.GetPlayerInfo((response) =>
        {
            mapLevel = (int)response.level;
        });

        SceneManager.LoadScene("DZ2");
    }

    public void OnEnterButtonClicked_DropZone3()
    {
        LootLockerSDKManager.GetPlayerInfo((response) =>
        {
            mapLevel = (int)response.level;
        });

        SceneManager.LoadScene("DZ3");
    }

    public void OnEnterButtonClicked_WeaponTest()
    {
        LootLockerSDKManager.GetPlayerInfo((response) =>
        {
            mapLevel = (int)response.level;
        });

        SceneManager.LoadScene("WeaponTest");
    }
}