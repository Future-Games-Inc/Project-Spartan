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
                if (enemyDamageCrit.Health <= (10 * bulletModifier) && enemyDamageCrit.alive == true && playerHealth != null)
                {
                    playerHealth.EnemyKilled("Normal");
                    enemyDamageCrit.TakeDamage((10 * bulletModifier));
                }

                else if (enemyDamageCrit.Health > (10 * bulletModifier) && enemyDamageCrit.alive == true && playerHealth != null)
                {
                    enemyDamageCrit.TakeDamage(10 * bulletModifier);
                }
            }

            else
            {
                FollowAI enemyDamage = other.GetComponentInParent<FollowAI>();
                if (enemyDamage.Health <= (bulletModifier) && enemyDamage.alive == true && playerHealth != null)
                {
                    playerHealth.EnemyKilled("Normal");
                    enemyDamage.TakeDamage(bulletModifier);
                }

                else if (enemyDamage.Health > (bulletModifier) && enemyDamage.alive == true && playerHealth != null)
                {
                    enemyDamage.TakeDamage(bulletModifier);
                }
            }
            Destroy(gameObject);
        }

        else if (other.CompareTag("Enemy") && playerHealth == null)
        {
            float criticalChance = 10f;

            //call it at random probability
            if (Random.Range(0, 100f) < criticalChance)
            {
                //critical hit here
                FollowAI enemyDamageCrit = other.GetComponentInParent<FollowAI>();
                enemyDamageCrit.TakeDamage(10 * bulletModifier);
            }

            else
            {
                FollowAI enemyDamage = other.GetComponentInParent<FollowAI>();
                enemyDamage.TakeDamage(bulletModifier);
            }
            Destroy(gameObject);
        }

        else if (other.CompareTag("Reinforcements") && playerHealth == null)
        {
            float criticalChance = 10f;

            //call it at random probability
            if (Random.Range(0, 100f) < criticalChance)
            {
                //critical hit here
                ReinforcementAI enemyDamageCrit = other.GetComponentInParent<ReinforcementAI>();
                enemyDamageCrit.TakeDamage(10 * bulletModifier);
            }

            else
            {
                ReinforcementAI enemyDamage = other.GetComponentInParent<ReinforcementAI>();
                enemyDamage.TakeDamage(bulletModifier);
            }
            Destroy(gameObject);
        }

        else if (other.CompareTag("Reinforcements") && playerHealth != null)
        {
            float criticalChance = 10f;

            //call it at random probability
            if (Random.Range(0, 100f) < criticalChance)
            {
                //critical hit here
                ReinforcementAI enemyDamageCrit = other.GetComponentInParent<ReinforcementAI>();
                enemyDamageCrit.TakeDamage(10 * bulletModifier);
            }

            else
            {
                ReinforcementAI enemyDamage = other.GetComponentInParent<ReinforcementAI>();
                enemyDamage.TakeDamage(bulletModifier);
            }
            Destroy(gameObject);
        }

        else if (other.CompareTag("BossEnemy") && playerHealth != null)
        {
            float criticalChance = 10f;

            //call it at random probability
            if (Random.Range(0, 100f) < criticalChance)
            {
                //critical hit here
                FollowAI enemyDamageCrit = other.GetComponentInParent<FollowAI>();
                if (enemyDamageCrit.Health <= (5 * bulletModifier) && enemyDamageCrit.alive == true && playerHealth != null)
                {
                    playerHealth.EnemyKilled("Boss");
                    enemyDamageCrit.TakeDamage((5 * bulletModifier));
                }

                else if (enemyDamageCrit.Health > (5 * bulletModifier) && enemyDamageCrit.alive == true && playerHealth != null)
                {
                    enemyDamageCrit.TakeDamage(5 * bulletModifier);
                }
            }

            else
            {
                FollowAI enemyDamage = other.GetComponentInParent<FollowAI>();
                if (enemyDamage.Health <= (bulletModifier) && enemyDamage.alive == true && playerHealth != null)
                {
                    playerHealth.EnemyKilled("Boss");
                    enemyDamage.TakeDamage(bulletModifier);
                }

                else if (enemyDamage.Health > (bulletModifier) && enemyDamage.alive == true && playerHealth != null)
                {
                    enemyDamage.TakeDamage(bulletModifier);
                }
            }
            Destroy(gameObject);
        }

        else if (other.CompareTag("BossEnemy") && playerHealth == null)
        {
            float criticalChance = 10f;

            //call it at random probability
            if (Random.Range(0, 100f) < criticalChance)
            {
                //critical hit here
                FollowAI enemyDamageCrit = other.GetComponentInParent<FollowAI>();
                enemyDamageCrit.TakeDamage((5 * bulletModifier));
            }

            else
            {
                FollowAI enemyDamage = other.GetComponentInParent<FollowAI>();
                enemyDamage.TakeDamage(bulletModifier);
            }
            Destroy(gameObject);
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
                    if (enemyDamageCrit.Health <= (10 * bulletModifier) && enemyDamageCrit.alive == true && playerHealth != null && enemyDamageCrit.gameObject.GetComponent<LootDrone>() != null)
                    {
                        playerHealth.DroneKilled(enemyDamageCrit.gameObject);
                        enemyDamageCrit.TakeDamage(10 * bulletModifier);
                    }
                    else
                        enemyDamageCrit.TakeDamage(10 * bulletModifier);
                }
                else if (enemyDamageCrit == null)
                {
                    SentryDrone enemyDamageCrit2 = other.GetComponentInParent<SentryDrone>();
                    enemyDamageCrit2.TakeDamage(10 * bulletModifier);
                }
                Destroy(gameObject);
            }

            else
            {
                DroneHealth enemyDamage = other.GetComponentInParent<DroneHealth>();
                if (enemyDamage != null)
                {
                    if (enemyDamage.Health <= (bulletModifier) && enemyDamage.alive == true && playerHealth != null && enemyDamage.gameObject.GetComponent<LootDrone>() != null)
                    {
                        playerHealth.DroneKilled(enemyDamage.gameObject);
                        enemyDamage.TakeDamage(bulletModifier);
                    }
                    else
                        enemyDamage.TakeDamage(bulletModifier);
                }
                else if (enemyDamage == null)
                {
                    SentryDrone enemyDamage2 = other.GetComponentInParent<SentryDrone>();
                    enemyDamage2.TakeDamage(bulletModifier);
                }
            }
            Destroy(gameObject);
        }

        else if (other.CompareTag("Security") && playerHealth == null)
        {
            float criticalChance = 30f;

            //cal it at random probability
            if (Random.Range(0, 100f) < criticalChance)
            {
                //critical hit here
                DroneHealth enemyDamageCrit = other.GetComponentInParent<DroneHealth>();
                if (enemyDamageCrit != null)
                {
                    enemyDamageCrit.TakeDamage(10 * bulletModifier);
                }
                else if (enemyDamageCrit == null)
                {
                    SentryDrone enemyDamageCrit2 = other.GetComponentInParent<SentryDrone>();
                    enemyDamageCrit2.TakeDamage(10 * bulletModifier);
                }
            }

            else
            {
                DroneHealth enemyDamage = other.GetComponentInParent<DroneHealth>();
                if (enemyDamage != null)
                {
                    enemyDamage.TakeDamage(bulletModifier);
                }
                else if (enemyDamage == null)
                {
                    SentryDrone enemyDamage2 = other.GetComponentInParent<SentryDrone>();
                    enemyDamage2.TakeDamage(bulletModifier);
                }
            }
            Destroy(gameObject);
        }


        else if (other.CompareTag("Player") || other.CompareTag("ReactorInteractor")
                || other.CompareTag("Bullet") || other.CompareTag("RightHand") || other.CompareTag("LeftHand") || other.CompareTag("RHand") || other.CompareTag("LHand"))
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
                Destroy(gameObject);
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
            Destroy(gameObject);
        }
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
