using LootLocker.Requests;
using System.Collections;
using TMPro;
using UnityEngine;

public class WalkieSupport : MonoBehaviour
{
    public MatchEffects matchEffects;
    public PlayerHealth player;
    public float factionScore;
    public SpawnManager1 spawnManager;

    public int Score;
    public GameObject button1;
    public GameObject button2;
    public GameObject button3;

    public bool check;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponentInParent<PlayerHealth>();
        }

        if (matchEffects == null)
        {
            matchEffects = GameObject.FindGameObjectWithTag("Props").GetComponentInParent<MatchEffects>();
        }

        if (spawnManager == null)
        {
            spawnManager = GameObject.FindGameObjectWithTag("spawnManager").GetComponentInParent<SpawnManager1>();
        }

        if (player.faction.ToString() != matchEffects.owner.ToString() && !check)
        {
            check = true;
            StartCoroutine(CheckFaction());
        }
        else if (player.faction.ToString() == matchEffects.owner.ToString() && !check)
        {
            check = true;
            StartCoroutine(CheckFaction2());
        }
    }

    IEnumerator CheckFaction()
    {
        LootLockerSDKManager.GetMemberRank(player.leaderboardID2.ToString(), player.faction.ToString(), (response) =>
        {
            if (response.success)
            {
                Score = response.score;
                if (Score >= 100)
                {
                    button1.SetActive(false);
                    button3.SetActive(false);
                    button2.SetActive(true);
                }
            }
            else
                button3.SetActive(true);
        });
        yield return new WaitForSeconds(15);
        check = false;
    }


    IEnumerator CheckFaction2()
    {
        LootLockerSDKManager.GetMemberRank(player.leaderboardID2.ToString(), player.faction.ToString(), (response) =>
        {
            if (response.success)
            {
                Score = response.score;
                if (Score >= 300)
                {
                    button1.SetActive(true);
                    button3.SetActive(false);
                    button2.SetActive(false);
                }
            }
            else
                button3.SetActive(true);
        });
        yield return new WaitForSeconds(20);
        check = false;
    }

    public void Reinforcements()
    {
        if (player.faction.ToString() == matchEffects.owner.ToString())
        {
            LootLockerSDKManager.GetMemberRank(player.leaderboardID2.ToString(), player.faction.ToString(), (response) =>
            {
                if (response.success)
                {
                    Score = response.score;
                    if (Score >= 300)
                    {
                        spawnManager.spawnReinforcements = true;
                        LootLockerSDKManager.SubmitScore(player.faction.ToString(), Score - 300, player.leaderboardID2.ToString(), (response) =>
                        {
                            if (response.success)
                            {
                                PlayerVoiceover voice = GameObject.FindGameObjectWithTag("Player").GetComponentInParent<PlayerVoiceover>();

                                StartCoroutine(voice.VoiceOvers(player.faction, 1));
                            }
                        });
                        Destroy(gameObject);
                    }
                }
            });
        }
    }

    public void Breach()
    {
        if (player.faction.ToString() != matchEffects.owner.ToString())
        {
            LootLockerSDKManager.GetMemberRank(player.leaderboardID2.ToString(), player.faction.ToString(), (response) =>
            {
                if (response.success)
                {
                    Score = response.score;
                    if (Score >= 100)
                    {
                        LootLockerSDKManager.SubmitScore(player.faction.ToString(), Score - 100, player.leaderboardID2.ToString(), (response) =>
                        {
                            if (response.success)
                            {
                                LootLockerSDKManager.GetMemberRank(player.leaderboardID2.ToString(), matchEffects.owner.ToString(), (response) =>
                                {
                                    if (response.success)
                                    {
                                        Score = response.score;
                                        {
                                            LootLockerSDKManager.SubmitScore(matchEffects.owner.ToString(), Score - 150, player.leaderboardID2.ToString(), (response) =>
                                            {

                                            });
                                        }
                                    }
                                });
                            }
                        });
                        Destroy(gameObject);
                    }
                }
            });
        }
    }

    public void rescale()
    {
        this.gameObject.transform.localScale = Vector3.one;
    }

}
