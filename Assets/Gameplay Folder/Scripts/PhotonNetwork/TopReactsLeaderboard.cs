using System.Collections;
using UnityEngine;
using LootLocker.Requests;
using TMPro;
using UnityEngine.UI;

public class TopReactsLeaderboard : MonoBehaviour
{
    public string leaderboardID = "react_leaderboard";
    public string leaderboardID2 = "faction_leaders";
    public string leaderboardID3 = "react_kills";
    public string progressionKey = "cent_prog";

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

    public int currentLevelInt;
    public int Score;

    public BlackMarketManager blackMarketManager;
    public ProgressionBadges progressionBadges;
    public SaveData saveData;
    public bool contractBool;

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
        yield return new WaitForSeconds(1f);
        bool done = false;
        LootLockerSDKManager.SubmitScore(WhiteLabelManager.playerID, scoreToUpload, leaderboardID3, (response) =>
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
        bool done = false;
        LootLockerSDKManager.GetScoreList(leaderboardID3, 5, 0, (response) =>
        {
            if (response.success)
            {
                string tempPlayerNames = "Reacts\n";
                string TempPlayerScores = "CUA Eliminated\n";

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
        StartCoroutine(FetchFactionLeaderboard());
    }

    public IEnumerator FetchFactionLeaderboard()
    {
        bool done = false;
        LootLockerSDKManager.GetScoreList(leaderboardID2, 4, 0, (response) =>
        {
            if (response.success)
            {
                string tempFactionNames = "Factions\n";
                string TempFactionScores = "Scores\n";

                LootLockerLeaderboardMember[] members = response.items;

                for (int i = 0; i < members.Length; i++)
                {
                    tempFactionNames += members[i].rank + ". ";
                    if (members[i].member_id != "")
                    {
                        tempFactionNames += members[i].member_id;
                    }
                    else
                    {
                        tempFactionNames += members[i].member_id;
                    }
                    TempFactionScores += members[i].score + "\n";
                    tempFactionNames += "\n";
                }
                done = true;
                factionNames.text = tempFactionNames;
                factionScores.text = TempFactionScores;
            }
            else
            {
                done = true;
            }
        });
        yield return new WaitWhile(() => done == false);
        StartCoroutine(CheckLevel());
    }

    public IEnumerator CheckLevel()
    {
        yield return new WaitForSeconds(.5f);
        LootLockerSDKManager.GetPlayerProgression(progressionKey, (response) =>
        {
            currentLevel.text = response.step.ToString();
            currentLevelInt = (int)response.step;
            nextLevel.text = (response.step + 1).ToString();
            currentXPText.text = response.points + " / " + (response.step * 100);

            if (levelSlider.value == levelSlider.maxValue)
            {
                levelSlider.maxValue = (float)(response.next_threshold);
                levelSlider.value = 0;
            }
            else
            {
                levelSlider.maxValue = (float)(response.next_threshold);
                levelSlider.value = (float)response.points;
            }
        });
        StartCoroutine(SetScore());
    }

    public IEnumerator SetScore()
    {
        if (PlayerPrefs.HasKey("EnemyKills"))
        {
            LootLockerSDKManager.SubmitScore(WhiteLabelManager.playerID, PlayerPrefs.GetInt("EnemyKills"), leaderboardID3, (response) =>
            {
            });
        }
        LootLockerSDKManager.GetMemberRank(leaderboardID3, WhiteLabelManager.playerID, (response) =>
        {
            if (response.success)
            {
                Score = response.score;
            }
        });
        if (!contractBool)
        {
            contractBool = true;
            yield return blackMarketManager.DisplayAvailableContracts();
        }
        yield return saveData.PlayerLevelRoutine();
        yield return progressionBadges.UpdateBadges();
    }

    public void AddProgression(int XP)
    {
        LootLockerSDKManager.AddPointsToPlayerProgression(progressionKey, (ulong)XP, response =>
        {
        });
    }
}
