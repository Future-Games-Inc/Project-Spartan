using System.Collections;
using UnityEngine;
using System;
using LootLocker.Requests;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class TopReactsLeaderboard : MonoBehaviour
{
    public string leaderboardID = "react_leaderboard";
    public string leaderboardID2 = "faction_leaderboard";

    public bool updater = true;
    public bool rewardGiven = false;

    public TextMeshProUGUI playerNames;
    public TextMeshProUGUI playerScores;

    public Slider levelSlider;
    public TextMeshProUGUI currentLevel;
    public TextMeshProUGUI nextLevel;
    public TextMeshProUGUI currentXPText;

    public RoomManager roomManager;
    public TimeTracker timeTracker;
    public RawImage rewardIcon;

    public String isLocalPlayer;
    public String firstPlayerID;
    string playerID;

    public int currentLevelInt;
    public int Score;

    public BlackMarketManager blackMarketManager;
    public ProgressionBadges progressionBadges;
    public SaveData saveData;
    public WhiteLabelManager whiteLabelManager;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(CheckLevel());
        //StartCoroutine(GiveRewards());
        //StartCoroutine(RewardsGiven());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SubmitScore(int Score)
    {
        StartCoroutine(SubmitScoreRoutine(Score));
    }

    //public IEnumerator RewardsGiven()
    //{
    //    while (true)
    //    {
    //        if (timeTracker.rewarded == false)
    //        {
    //            rewardIcon.color = Color.black;
    //            rewardGiven = false;
    //        }
    //        yield return null;
    //    }
    //}

    //public IEnumerator GiveRewards()
    //{
    //    while (true)
    //    {
    //        if (timeTracker.rewarded == true && rewardGiven == false)
    //        {
    //            rewardGiven = true;
    //            bool done = false;
    //            LootLockerSDKManager.GetScoreList(leaderboardID, 1, 0, (response) =>
    //            {
    //                if (response.success)
    //                {
    //                    LootLockerLeaderboardMember[] members = response.items;

    //                    for (int i = 0; i < members.Length; i++)
    //                    {
    //                        firstPlayerID = members[i].player.name.ToString();
    //                        LootLockerSDKManager.GetPlayerName((response) =>
    //                        {
    //                            isLocalPlayer = response.name.ToString();
    //                            done = true;
    //                        });
    //                    }
    //                }
    //            });
    //            yield return new WaitWhile(() => done == false);
    //            if (firstPlayerID.ToString() == isLocalPlayer.ToString())
    //            {
    //                rewardIcon.color = Color.white;
    //            }
    //        }
    //        yield return null;
    //    }
    //}

    public IEnumerator SubmitScoreRoutine(int scoreToUpload)
    {
        yield return new WaitForSeconds(3);
        bool done = false;
        playerID = whiteLabelManager.playerID;
        LootLockerSDKManager.SubmitScore(playerID, scoreToUpload, leaderboardID, (response) =>
        {
            if (response.success)
            {
                done = true;
            }
            else
            {
                done = true;
            }
        });
        yield return new WaitWhile(() => done == false);
    }

    public IEnumerator FetchTopHighScores()
    {
        while (updater)
        {
            bool done = false;
            LootLockerSDKManager.GetScoreList(leaderboardID, 5, 0, (response) =>
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
                    done = true;
                }
            });
            yield return new WaitWhile(() => done == false);
            StartCoroutine(CheckLevel());
            yield return new WaitForSeconds(20);
        }
    }

    public IEnumerator CheckLevel()
    {
        yield return new WaitForSeconds(.75f);
        LootLockerSDKManager.GetPlayerInfo((response) =>
        {
            currentLevelInt = (int)response.level;
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
        StartCoroutine(SetScore());
    }

    public IEnumerator SetScore()
    {
        playerID = whiteLabelManager.playerID;
        LootLockerSDKManager.GetMemberRank(leaderboardID, playerID, (response) =>
        {
            if (response.success)
            {
                Score = response.score;
            }
            else
            {

            }
        });
        yield return blackMarketManager.DisplayAvailableContracts();
        yield return saveData.PlayerLevelRoutine();
        yield return progressionBadges.UpdateBadges();
    }
}
