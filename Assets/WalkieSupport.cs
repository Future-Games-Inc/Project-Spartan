using LootLocker.Requests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class WalkieSupport : MonoBehaviour
{
    public MatchEffects matchEffects;
    public PlayerHealth player;
    public float factionScore;
    public SpawnManager1 spawnManager;

    public int Score;
    public GameObject button1;
    public GameObject button2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponentInParent<PlayerHealth>();
        }

        if (player.faction.ToString() != matchEffects.owner.ToString())
        {
            button1.SetActive(false);
            button2.SetActive(true);
        }
        else
        {
            button1.SetActive(true);
            button2.SetActive(false);
        }
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
                    if(Score >= 300)
                    {
                        spawnManager.spawnReinforcements = true;
                        StartCoroutine(spawnManager.SpawnReinforcements());
                        LootLockerSDKManager.SubmitScore(player.faction.ToString(), Score-300, player.leaderboardID2.ToString(), (response) =>
                        {
                        });
                    }
                }
            });
            Destroy(gameObject);
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
                    }
                }
            });
            Destroy(gameObject);
        }
    }

    public void rescale()
    {
        this.gameObject.transform.localScale = Vector3.one;
    }

}
