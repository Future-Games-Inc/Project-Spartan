using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerKillUI : MonoBehaviour
{
    public TextMeshProUGUI playerKillUI;
    public PlayerHealth playerHealth;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckEnemiesKilled()
    {
        playerKillUI.text = "Players Killed" + playerHealth.playersKilled.ToString();
    }
}
