using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LootLocker.Requests;
using TMPro;
using UnityEngine.UI;

public class TopReactsLeaderboard : MonoBehaviour
{
    public int leaderboardID = 9820;
    public int leaderboardID2 = 10220;

    public bool updater = true;
    public bool rewardGiven = false;

    public TextMeshProUGUI playerNames;
    public TextMeshProUGUI playerScores;

    public TextMeshProUGUI factionNames;
    public TextMeshProUGUI factionScores;

    public Slider levelSlider;
    public TextMeshProUGUI currentLevel;
    public TextMeshProUGUI nextLevel;
    public TextMeshProUGUI currentXPText;

    public RoomManager roomManager;
    public TimeTracker timeTracker;
    public RawImage rewardIcon;

    public String isLocalPlayer;
    public String firstPlayerID;

    // Start is called before the first frame update
    [Obsolete]
    void Start()
    {
        StartCoroutine(CheckLevel());
        StartCoroutine(GiveRewards());
        StartCoroutine(RewardsGiven());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator RewardsGiven()
    {
        while (true)
        {
            if (timeTracker.rewarded == false)
            {
                rewardIcon.color = Color.black;
                rewardGiven = false;
            }
            yield return null;
        }
    }

    [Obsolete]
    public IEnumerator GiveRewards()
    {
        while (true)
        {
            if (timeTracker.rewarded == true && rewardGiven == false)
            {
                rewardGiven = true;
                bool done = false;
                LootLockerSDKManager.GetScoreListMain(leaderboardID, 1, 0, (response) =>
                {
                    if (response.success)
                    {
                        LootLockerLeaderboardMember[] members = response.items;

                        for (int i = 0; i < members.Length; i++)
                        {
                            firstPlayerID = members[i].player.name.ToString();
                            LootLockerSDKManager.GetPlayerName((response) =>
                            {
                                isLocalPlayer = response.name.ToString();
                                done = true;
                            });
                        }
                    }
                });
                yield return new WaitWhile(() => done == false);
                if (firstPlayerID.ToString() == isLocalPlayer.ToString())
                {
                    rewardIcon.color = Color.white;
                }
            }
            yield return null;
        }
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
        while (updater)
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
            yield return new WaitForSeconds(20);
        }
    }

    [System.Obsolete]
    public IEnumerator FetchFactionScores()
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

    public IEnumerator CheckLevel()
    {
        yield return new WaitForSeconds(2);
        LootLockerSDKManager.GetPlayerInfo((response) =>
        {
            currentLevel.text = response.level.ToString();
            nextLevel.text = (response.level + 1).ToString();
            currentXPText.text = response.xp.ToString() + " / " + response.level_thresholds.next.ToString();

            if (levelSlider.value == levelSlider.maxValue)
            {
                levelSlider.maxValue = (float)(response.level_thresholds.next - response.xp);
                levelSlider.value = 0;
            }
            else
            {
                levelSlider.value = (float)response.xp - response.level_thresholds.current;
            }
            roomManager.mapLevel = (int)response.level;
        });
    }
}
