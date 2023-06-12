using Photon.Pun;
using UnityEngine;

public class BlackoutBullet : MonoBehaviourPunCallbacks
{
    [Header("Bullet Behavior ---------------------------------------------------")]
    public GameObject hitEffectPrefab;
    public GameObject smoke;

    private bool hasHit = false;
    public GameObject bulletOwner;
    public PlayerHealth playerHealth;
    public bool playerBullet = false;

    [Header("Bullet Effects ---------------------------------------------------")]
    public int damage = 10;
    public float blastRadius = 5.0f;
    public float maxBlindDuration = 6.0f;
    public float minBlindDuration = 2.0f;

    [System.Obsolete]
    private void OnTriggerEnter(Collider other)
    {
        if (!hasHit)
        {
            hasHit = true;

            if (playerBullet == true)
            {
                playerHealth = bulletOwner.GetComponent<PlayerHealth>();
            }
            else
            {
                playerHealth = null;
            }

            if (other.gameObject.CompareTag("Enemy"))
            {
                FollowAI enemyDamageCrit = other.GetComponent<FollowAI>();
                if (enemyDamageCrit.Health <= (10) && enemyDamageCrit.alive == true && playerHealth != null)
                {
                    playerHealth.EnemyKilled("Normal");
                    enemyDamageCrit.TakeDamage(10);
                }
                else if (enemyDamageCrit.Health > (10) && enemyDamageCrit.alive == true && playerHealth != null)
                {
                    enemyDamageCrit.TakeDamage(10);
                }
            }

            else if (other.gameObject.CompareTag("BossEnemy"))
            {
                FollowAI enemyDamageCrit = other.GetComponent<FollowAI>();
                if (enemyDamageCrit.Health <= (10) && enemyDamageCrit.alive == true && playerHealth != null)
                {
                    playerHealth.EnemyKilled("Boss");
                    enemyDamageCrit.TakeDamage(10);
                }
                else if (enemyDamageCrit.Health > (10) && enemyDamageCrit.alive == true && playerHealth != null)
                {
                    enemyDamageCrit.TakeDamage(10);
                }
            }

            else if (other.CompareTag("Security"))
            {
                //critical hit here
                DroneHealth enemyDamageCrit = other.GetComponent<DroneHealth>();
                enemyDamageCrit.TakeDamage(15);
            }

            else if (other.CompareTag("Player"))
            {
                //critical hit here
                PlayerHealth playerDamageCrit = other.GetComponent<PlayerHealth>();
                if (playerDamageCrit.Health <= (5) && playerDamageCrit.alive == true && playerHealth != null)
                {
                    playerHealth.PlayersKilled();
                }
                playerDamageCrit.TakeDamage(5);
            }

            // Apply hit effect
            PhotonNetwork.InstantiateRoomObject(hitEffectPrefab.name, transform.position, Quaternion.identity, 0, null);
            PhotonNetwork.InstantiateRoomObject(smoke.name,transform.position, Quaternion.identity, 0, null);

            // Destroy bullet
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
