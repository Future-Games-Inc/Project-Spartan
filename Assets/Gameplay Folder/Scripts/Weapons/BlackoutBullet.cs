using UnityEngine;

public class BlackoutBullet : MonoBehaviour
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

    public AudioSource audioSource;
    public AudioClip clip;

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

            // Apply hit effect
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            Instantiate(smoke,transform.position, Quaternion.identity);

            // Destroy bullet
            Destroy(gameObject);
        }
    }
}
