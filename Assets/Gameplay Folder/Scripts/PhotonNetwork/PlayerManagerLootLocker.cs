using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;
using Photon.Realtime;

public class PlayerManagerLootLocker : MonoBehaviour
{
    public TopReactsLeaderboard leaderboard;
    public WhiteLabelManager whiteLabelManager;
    const string playerNamePrefKey = "PlayerName";

    // Start is called before the first frame update
    void Start()
    {

    }

    public void LoginPlayer()
    {
        StartCoroutine(LoginRoutine());
    }

    public IEnumerator LoginRoutine()
    {
        bool done = false;
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (response.success)
            {
                PlayerPrefs.SetString("PlayerID", response.player_id.ToString());
                whiteLabelManager.playerID = response.player_id.ToString();
                done = true;
            }
            else
            {
                done = true;
            }
        });
        yield return new WaitWhile(() => done == false);

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
                }
                else
                {
                }
            });
        }
        yield return leaderboard.FetchTopHighScores();
    }
}
