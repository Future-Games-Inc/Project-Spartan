using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyKillUI : MonoBehaviour
{
    public TextMeshProUGUI enemyKillUI;
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
        enemyKillUI.text = "Enemies Killed: " + playerHealth.enemiesKilled.ToString();
    }
}
