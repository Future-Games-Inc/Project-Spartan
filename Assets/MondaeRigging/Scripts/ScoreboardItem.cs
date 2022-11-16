using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ScoreboardItem : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI usernametext;
    public TextMeshProUGUI killstext;
    public TextMeshProUGUI enemyKillsText;
    public TextMeshProUGUI extractionText;

    Player player;

    // Start is called before the first frame update
    public void Initialize(Player player)
    {
        this.player = player;
        usernametext.text = player.NickName;
        killstext.text = "Players Killed: " + 0.ToString();
        enemyKillsText.text = "Enemies Killed: " + 0.ToString();
        extractionText.text = "Extraction: " + 0.ToString();
        UpdateStats();
    }

    void UpdateStats()
    {
        if (player.CustomProperties.TryGetValue("playerKills", out object playersKilled))
        {
            killstext.text = "Players Killed: " + playersKilled.ToString();
        }

        if (player.CustomProperties.TryGetValue("enemyKills", out object enemiesKilled))
        {
            enemyKillsText.text = "Enemies Killed: " + enemiesKilled.ToString();
        }

        if (player.CustomProperties.TryGetValue("reactorExtraction", out object reactorExtraction))
        {
            extractionText.text = "Extraction: " + reactorExtraction.ToString() + "%";
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (targetPlayer == player)
        {
            if (changedProps.ContainsKey("playerKills") || changedProps.ContainsKey("enemyKills") || changedProps.ContainsKey("reactorExtraction"))
            {
                UpdateStats();
            }
        }
    }
}
