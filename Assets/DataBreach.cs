using BNG;
using LootLocker.Requests;
using TMPro;
using UnityEngine;

public class DataBreach : MonoBehaviour
{
    public SnapZone snapZone;
    public string faction;
    public PlayerHealth player;
    public int Score;
    public TextMeshProUGUI breachText;
    public MatchEffects matchEffects;

    // Start is called before the first frame update
    void Start()
    {
        matchEffects = FindObjectOfType<MatchEffects>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponentInParent<PlayerHealth>();
        }

        if (snapZone.HeldItem != null)
        {
            faction = snapZone.HeldItem.GetComponent<FactionCard>().faction;
            if(faction == "Decryption")
                breachText.text = "Decrypt Rael's Datacard and upload to faction servers?";
            else if (faction != player.faction && faction != "Decryption")
                breachText.text = "Breach " + faction + "? 100 Cs will be transferred from " + faction + " to " + player.faction + ".";
        }

        if (snapZone.HeldItem == null)
        {
            faction = "";
            breachText.text = "";
        }
    }


    public void Breach()
    {
        if (faction != "Decryption")
        {
            LootLockerSDKManager.GetMemberRank(player.leaderboardID2.ToString(), faction.ToString(), (response) =>
            {
                if (response.success)
                {
                    Score = response.score;
                    if (faction != player.faction)
                    {
                        LootLockerSDKManager.SubmitScore(faction.ToString(), Score - 100, player.leaderboardID2.ToString(), (response) =>
                        {
                            LootLockerSDKManager.GetMemberRank(player.leaderboardID2.ToString(), player.faction.ToString(), (response) =>
                            {
                                if (response.success)
                                {
                                    int playerScore = response.score;
                                    LootLockerSDKManager.SubmitScore(player.faction.ToString(), playerScore + 100, player.leaderboardID2.ToString(), (response) =>
                                    {
                                        PlayerVoiceover voice = GameObject.FindGameObjectWithTag("Player").GetComponentInParent<PlayerVoiceover>();

                                        StartCoroutine(voice.VoiceOvers(player.faction, 2));
                                    });
                                }
                            });
                        });
                    }
                    else
                    {
                        LootLockerSDKManager.SubmitScore(faction.ToString(), Score + 100, player.leaderboardID2.ToString(), (response) =>
                        {
                            PlayerVoiceover voice = GameObject.FindGameObjectWithTag("Player").GetComponentInParent<PlayerVoiceover>();

                            StartCoroutine(voice.VoiceOvers(player.faction, 2));
                        });
                    }
                }
            });
        }
        else if (faction == "Decryption")
        {
            matchEffects.MissionEnd();
            matchEffects.AddTime(60);
            player.UpdateSkills(250);
            player.GetXP(100);
            matchEffects.Rael.SetActive(false);
            matchEffects.MissionStart.SetActive(false);
        }
        Destroy(snapZone.lastHeldItem.gameObject);
    }
}
