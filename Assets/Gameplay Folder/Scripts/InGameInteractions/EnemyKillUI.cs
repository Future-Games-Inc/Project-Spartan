using TMPro;
using UnityEngine;

public class EnemyKillUI : MonoBehaviour
{
    public TextMeshProUGUI killText;
    public PlayerHealth playerHealth;
    public int enemyKills;

    private void Start()
    {
        killText.text = "Enemy Kills: " + playerHealth.enemiesKilled.ToString();
    }
    private void Update()
    {
        if (enemyKills != playerHealth.bulletModifier)
        {
            killText.text = "Enemy Kills: " + playerHealth.enemiesKilled.ToString();
            enemyKills = playerHealth.enemiesKilled;
        }
    }
}


