using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;
using Photon.Realtime;

public class PlayerManagerLootLocker : MonoBehaviour
{
    public TopReactsLeaderboard leaderboard;
    const string playerNamePrefKey = "PlayerName";

    // Start is called before the first frame update
    [System.Obsolete]
    void Start()
    {
        StartCoroutine(SetupRoutine());
    }

    [System.Obsolete]
    IEnumerator SetupRoutine()
    {
        yield return LoginRoutine();
        yield return leaderboard.FetchTopHighScores();
    }
    IEnumerator LoginRoutine()
    {
        bool done = false;
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (response.success)
            {
                Debug.Log("Player was logged in");
                PlayerPrefs.SetString("PlayerID", response.player_id.ToString());
                done = true;
            }
            else
            {
                Debug.Log("Could not start session");
                done = true;
            }
        });
        yield return new WaitWhile (() => done == false);

        string playerID = PlayerPrefs.GetString("PlayerID");
        AuthenticationValues authValues = new AuthenticationValues(playerID);

        string defaultName = string.Empty;
        if (PlayerPrefs.HasKey(playerNamePrefKey))
        {
            defaultName = PlayerPrefs.GetString(playerNamePrefKey);
            LootLockerSDKManager.SetPlayerName(defaultName, (response) =>
            {
                if (response.success)
                {
                    Debug.Log("Successfully set player name");
                }
                else
                {
                    Debug.Log("Could not set player name" + response.Error);
                }
            });
        }
        StartCoroutine(GetFriendsList());
    }
    // Update is called once per frame
    public IEnumerator GetFriendsList()
    {
        yield return new WaitForSeconds(2f);
        string playerID = PlayerPrefs.GetString("PlayerID");
        AuthenticationValues authValues = new AuthenticationValues(playerID);

    }
}
