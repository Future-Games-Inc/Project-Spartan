using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;
using TMPro;

public class TopReactsLeaderboard : MonoBehaviour
{
    public int leaderboardID = 9820;
    public int leaderboardID2 = 10220;
    public int cyberCints;
    public int muerteCints;
    public int chaosCints;
    public int cintCints;
    public int fedCints;

    public bool updater = true;

    public TextMeshProUGUI playerNames;
    public TextMeshProUGUI playerScores;

    public TextMeshProUGUI factionNames;
    public TextMeshProUGUI factionScores;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    [System.Obsolete]
    public IEnumerator SubmitScoreRoutine(int scoreToUpload)
    {
        bool done = false;
        string playerID = PlayerPrefs.GetString("PlayerID");
        LootLockerSDKManager.SubmitScore(playerID, scoreToUpload, leaderboardID, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Successfully uploaded score");
                done = true;
            }
            else
            {
                Debug.Log("Failed" + response.Error);
                done = true;
            }
        });
        yield return new WaitWhile(() => done == false);
    }

    [System.Obsolete]
    public IEnumerator FetchTopHighScores()
    {
        bool done = false;
        LootLockerSDKManager.GetScoreListMain(leaderboardID, 5, 0, (response) =>
        {
            if (response.success)
            {
                string tempPlayerNames = "Names\n";
                string TempPlayerScores = "Scores\n";

                LootLockerLeaderboardMember[] members = response.items;

                for (int i = 0; i < members.Length; i++)
                {
                    tempPlayerNames += members[i].rank + ". ";
                    if (members[i].player.name != "")
                    {
                        tempPlayerNames += members[i].player.name;
                    }
                    else
                    {
                        tempPlayerNames += members[i].player.id;
                    }
                    TempPlayerScores += members[i].score + "\n";
                    tempPlayerNames += "\n";
                }
                done = true;
                playerNames.text = tempPlayerNames;
                playerScores.text = TempPlayerScores;
            }
            else
            {
                Debug.Log("Failed" + response.Error);
                done = true;
            }
        });
        yield return new WaitWhile(() => done == false);
        StartCoroutine(FetchFactionScores());
    }

    [System.Obsolete]
    public IEnumerator FetchFactionScores()
    {
        while (updater)
        {
            bool done = false;
            LootLockerSDKManager.GetScoreListMain(leaderboardID2, 5, 0, (response) =>
            {
                if (response.success)
                {
                    string tempPlayerNames = "Names\n";
                    string TempPlayerScores = "Scores\n";

                    LootLockerLeaderboardMember[] members = response.items;

                    for (int i = 0; i < members.Length; i++)
                    {
                        tempPlayerNames += members[i].rank + ". ";
                        if (members[i].member_id != "")
                        {
                            tempPlayerNames += members[i].member_id;
                        }
                        else
                        {
                            tempPlayerNames += "";
                        }
                        TempPlayerScores += members[i].score + "\n";
                        tempPlayerNames += "\n";
                    }
                    done = true;
                    factionNames.text = tempPlayerNames;
                    factionScores.text = TempPlayerScores;
                }
                else
                {
                    Debug.Log("Failed" + response.Error);
                    done = true;
                }
            });
            yield return new WaitWhile(() => done == false);
            yield return new WaitForSeconds(20);
        }
    }
}
