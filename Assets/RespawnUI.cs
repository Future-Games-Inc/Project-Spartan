using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RespawnUI : MonoBehaviour
{
    public TextMeshProUGUI respawnText;
    public PlayerHealth playerHealth;

    private void Start()
    {
        respawnText.text = "Respawns Remaining: " + playerHealth.playerLives.ToString();
    }
    private void Update()
    {

    }

    public void UpdateRespawnUI()
    {
        respawnText.text = "Respawns Remaining: " + playerHealth.playerLives.ToString();
    }
}
