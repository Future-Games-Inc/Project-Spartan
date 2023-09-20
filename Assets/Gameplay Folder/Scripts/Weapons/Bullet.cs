using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject bulletOwner;
    public PlayerHealth playerHealth;
    public bool playerBullet = false;
    public int bulletModifier;
    public AudioSource audioSource;
    public AudioClip clip;


    // Start is called before the first frame update
    void OnEnable()
    {


        StartCoroutine(DestroyBullet());
    }

    // Update is called once per frame
    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("EnemyBullet") && gameObject.activeInHierarchy == true)
            StartCoroutine(DestroyBulletCollision());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playerBullet == true)
        {
            playerHealth = bulletOwner.GetComponentInParent<PlayerHealth>();
            bulletModifier = playerHealth.bulletModifier;
        }
        else
        {
            playerHealth = null;
        }

        if (other.CompareTag("Enemy") && playerHealth != null)
        {
            float criticalChance = 10f;

            //call it at random probability
            if (Random.Range(0, 100f) < criticalChance)
            {
                //critical hit here
                FollowAI enemyDamageCrit = other.GetComponentInParent<FollowAI>();
                if (enemyDamageCrit.Health <= (20 * bulletModifier) && enemyDamageCrit.alive == true && playerHealth != null)
                {
                    playerHealth.EnemyKilled("Normal");
                    enemyDamageCrit.TakeDamage((20 * bulletModifier));
                }

                else if (enemyDamageCrit.Health > (20 * bulletModifier) && enemyDamageCrit.alive == true && playerHealth != null)
                {
                    enemyDamageCrit.TakeDamage(20 * bulletModifier);
                }
            }

            else
            {
                FollowAI enemyDamage = other.GetComponentInParent<FollowAI>();
                if (enemyDamage.Health <= (10 * bulletModifier) && enemyDamage.alive == true && playerHealth != null)
                {
                    playerHealth.EnemyKilled("Normal");
                    enemyDamage.TakeDamage(10 * bulletModifier);
                }

                else if (enemyDamage.Health > (10 * bulletModifier) && enemyDamage.alive == true && playerHealth != null)
                {
                    enemyDamage.TakeDamage(10 * bulletModifier);
                }
            }
        }

        else if (other.CompareTag("BossEnemy") && playerHealth != null)
        {
            float criticalChance = 10f;

            //call it at random probability
            if (Random.Range(0, 100f) < criticalChance)
            {
                //critical hit here
                FollowAI enemyDamageCrit = other.GetComponentInParent<FollowAI>();
                if (enemyDamageCrit.Health <= (15 * bulletModifier) && enemyDamageCrit.alive == true && playerHealth != null)
                {
                    playerHealth.EnemyKilled("Boss");
                    enemyDamageCrit.TakeDamage((15 * bulletModifier));
                }

                else if (enemyDamageCrit.Health > (15 * bulletModifier) && enemyDamageCrit.alive == true && playerHealth != null)
                {
                    enemyDamageCrit.TakeDamage(15 * bulletModifier);
                }
            }

            else
            {
                FollowAI enemyDamage = other.GetComponentInParent<FollowAI>();
                if (enemyDamage.Health <= (5 * bulletModifier) && enemyDamage.alive == true && playerHealth != null)
                {
                    playerHealth.EnemyKilled("Boss");
                    enemyDamage.TakeDamage(5 * bulletModifier);
                }

                else if (enemyDamage.Health > (5 * bulletModifier) && !enemyDamage.alive == true && playerHealth != null)
                {
                    enemyDamage.TakeDamage(5 * bulletModifier);
                }
            }
        }

        else if (other.CompareTag("Security") && playerHealth != null)
        {
            float criticalChance = 30f;

            //cal it at random probability
            if (Random.Range(0, 100f) < criticalChance)
            {
                //critical hit here
                DroneHealth enemyDamageCrit = other.GetComponentInParent<DroneHealth>();
                if (enemyDamageCrit != null)
                {
                    if (enemyDamageCrit.Health <= (20 * bulletModifier) && enemyDamageCrit.alive == true && playerHealth != null)
                    {
                        playerHealth.DroneKilled(other.GetComponentInParent<DroneHealth>().type.ToString(), other.gameObject);
                        enemyDamageCrit.TakeDamage(20 * bulletModifier);
                    }

                    else if (enemyDamageCrit.Health > (20 * bulletModifier) && enemyDamageCrit.alive == true && playerHealth != null)
                        enemyDamageCrit.TakeDamage(20 * bulletModifier);
                }
                else
                {
                    SentryDrone enemyDamageCrit2 = other.GetComponentInParent<SentryDrone>();
                    if (enemyDamageCrit2.Health <= (20 * bulletModifier) && enemyDamageCrit2.alive == true && playerHealth != null)
                    {
                        playerHealth.GuardianKilled();
                        enemyDamageCrit2.TakeDamage(20 * bulletModifier);
                    }

                    else if (enemyDamageCrit2.Health > (20 * bulletModifier) && enemyDamageCrit2.alive == true && playerHealth != null)
                        enemyDamageCrit2.TakeDamage(20 * bulletModifier);
                }
            }

            else
            {
                DroneHealth enemyDamage = other.GetComponentInParent<DroneHealth>();
                if (enemyDamage != null)
                {
                    if (enemyDamage.Health <= (10 * bulletModifier) && enemyDamage.alive == true && playerHealth != null)
                    {
                        playerHealth.DroneKilled(other.GetComponentInParent<DroneHealth>().type.ToString(), other.gameObject);
                        enemyDamage.TakeDamage(10 * bulletModifier);
                    }

                    else if (enemyDamage.Health > (10 * bulletModifier) && enemyDamage.alive == true && playerHealth != null)
                        enemyDamage.TakeDamage(10 * bulletModifier);
                }
                else
                {
                    SentryDrone enemyDamage2 = other.GetComponentInParent<SentryDrone>();
                    if (enemyDamage2.Health <= (10 * bulletModifier) && enemyDamage2.alive == true && playerHealth != null)
                    {
                        playerHealth.GuardianKilled();
                        enemyDamage2.TakeDamage(10 * bulletModifier);
                    }

                    else if (enemyDamage2.Health > (10 * bulletModifier) && enemyDamage2.alive == true && playerHealth != null)
                        enemyDamage2.TakeDamage(10 * bulletModifier);
                }
            }
        }


        else if (other.CompareTag("Player"))
        {
            if (!playerBullet)
            {
                float criticalChance = 20f;
                if (Random.Range(0, 100f) < criticalChance)
                {
                    //critical hit here
                    PlayerHealth playerDamageCrit = other.GetComponentInParent<PlayerHealth>();
                    playerDamageCrit.TakeDamage(5 * bulletModifier);
                }

                else
                {
                    PlayerHealth playerDamage = other.GetComponentInParent<PlayerHealth>();
                    playerDamage.TakeDamage(bulletModifier);
                }
            }
        }

        else if (other.CompareTag("Tower"))
        {
            if (playerBullet)
            {
                //critical hit here
                ReactorCover reactorcover = other.GetComponentInParent<ReactorCover>();
                reactorcover.TakeDamage(5 * bulletModifier);
            }
        }
        Destroy(gameObject);
    }

    IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }
    IEnumerator DestroyBulletCollision()
    {
        yield return new WaitForSeconds(0.05f);
        Destroy(gameObject);
    }
}
