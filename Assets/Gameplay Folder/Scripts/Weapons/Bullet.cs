using System.Collections;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPunCallbacks
{
    public GameObject bulletOwner;
    public PlayerHealth playerHealth;
    public bool playerBullet = false;
    public int bulletModifier;

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

    [System.Obsolete]
    private void OnTriggerEnter(Collider other)
    {
        if (playerBullet == true)
        {
            playerHealth = bulletOwner.GetComponent<PlayerHealth>();
        }
        else
        {
            playerHealth = null;
        }

        if (other.CompareTag("Enemy"))
        {
            float criticalChance = 10f;

            //call it at random probability
            if (Random.Range(0, 100f) < criticalChance)
            {
                //critical hit here
                FollowAI enemyDamageCrit = other.GetComponent<FollowAI>();
                if (enemyDamageCrit.Health <= (20 * bulletModifier) && enemyDamageCrit.alive == true && playerHealth != null)
                {
                    playerHealth.EnemyKilled("Normal");
                    enemyDamageCrit.TakeDamage((20 * bulletModifier));
                }
                else if (enemyDamageCrit.Health > (20 * bulletModifier) && enemyDamageCrit.alive == true && playerHealth != null)
                {
                    enemyDamageCrit.TakeDamage(20 * bulletModifier);
                }
                PhotonNetwork.Destroy(gameObject);
            }

            else
            {
                FollowAI enemyDamage = other.GetComponent<FollowAI>();
                if (enemyDamage.Health <= (10 * bulletModifier) && enemyDamage.alive == true && playerHealth != null)
                {
                    playerHealth.EnemyKilled("Normal");
                    enemyDamage.TakeDamage(10 * bulletModifier);
                }
                else if (enemyDamage.Health > (10 * bulletModifier) && enemyDamage.alive == true && playerHealth != null)
                {
                    enemyDamage.TakeDamage(10 * bulletModifier);
                }
                PhotonNetwork.Destroy(gameObject);
            }
        }

        if (other.CompareTag("BossEnemy"))
        {
            float criticalChance = 10f;

            //call it at random probability
            if (Random.Range(0, 100f) < criticalChance)
            {
                //critical hit here
                FollowAI enemyDamageCrit = other.GetComponent<FollowAI>();
                if (enemyDamageCrit.Health <= (20 * bulletModifier) && enemyDamageCrit.alive == true && playerHealth != null)
                {
                    playerHealth.EnemyKilled("Boss");
                    enemyDamageCrit.TakeDamage((20 * bulletModifier));
                }
                else if (enemyDamageCrit.Health > (20 * bulletModifier) && enemyDamageCrit.alive == true && playerHealth != null)
                {
                    enemyDamageCrit.TakeDamage(20 * bulletModifier);
                }
                PhotonNetwork.Destroy(gameObject);
            }

            else
            {
                FollowAI enemyDamage = other.GetComponent<FollowAI>();
                if (enemyDamage.Health <= (10 * bulletModifier) && enemyDamage.alive == true && playerHealth != null)
                {
                    playerHealth.EnemyKilled("Boss");
                    enemyDamage.TakeDamage(10 * bulletModifier);
                }
                else if (enemyDamage.Health > (10 * bulletModifier) && enemyDamage.alive == true && playerHealth != null)
                {
                    enemyDamage.TakeDamage(10 * bulletModifier);
                }
                PhotonNetwork.Destroy(gameObject);
            }
        }

        else if (other.CompareTag("Security"))
        {
            float criticalChance = 30f;

            //cal it at random probability
            if (Random.Range(0, 100f) < criticalChance)
            {
                //critical hit here
                DroneHealth enemyDamageCrit = other.GetComponent<DroneHealth>();
                if (enemyDamageCrit != null)
                {
                    if (enemyDamageCrit.Health <= (30 * bulletModifier) && enemyDamageCrit.alive == true && playerHealth != null)
                    {
                        playerHealth.DroneKilled(other.GetComponent<DroneHealth>().type.ToString(), other.gameObject);
                        enemyDamageCrit.TakeDamage(30 * bulletModifier);
                    }
                    else if (enemyDamageCrit.Health > (30 * bulletModifier) && enemyDamageCrit.alive == true && playerHealth != null)
                        enemyDamageCrit.TakeDamage(30 * bulletModifier);
                }
                else
                {
                    SentryDrone enemyDamageCrit2 = other.GetComponent<SentryDrone>();
                    if (enemyDamageCrit2.Health <= (30 * bulletModifier) && enemyDamageCrit.alive == true && playerHealth != null)
                    {
                        playerHealth.GuardianKilled();
                        enemyDamageCrit.TakeDamage(30 * bulletModifier);
                    }
                    else if (enemyDamageCrit.Health > (30 * bulletModifier) && enemyDamageCrit.alive == true && playerHealth != null)
                        enemyDamageCrit.TakeDamage(30 * bulletModifier);
                }
                PhotonNetwork.Destroy(gameObject);
            }

            else
            {
                DroneHealth enemyDamage = other.GetComponent<DroneHealth>();
                if (enemyDamage != null)
                {
                    if (enemyDamage.Health <= (30 * bulletModifier) && enemyDamage.alive == true && playerHealth != null)
                    {
                        playerHealth.DroneKilled(other.GetComponent<DroneHealth>().type.ToString(), other.gameObject);
                        enemyDamage.TakeDamage(30 * bulletModifier);
                    }
                    else if (enemyDamage.Health > (30 * bulletModifier) && enemyDamage.alive == true && playerHealth != null)
                        enemyDamage.TakeDamage(30 * bulletModifier);
                }
                else
                {
                    SentryDrone enemyDamage2 = other.GetComponent<SentryDrone>();
                    if (enemyDamage.Health <= (30 * bulletModifier) && enemyDamage.alive == true && playerHealth != null)
                    {
                        playerHealth.GuardianKilled();
                        enemyDamage2.TakeDamage(30 * bulletModifier);
                    }
                    else if (enemyDamage2.Health > (30 * bulletModifier) && enemyDamage2.alive == true && playerHealth != null)
                        enemyDamage2.TakeDamage(30 * bulletModifier);
                }
                PhotonNetwork.Destroy(gameObject);
            }
        }


        else if (other.CompareTag("Player"))
        {
            float criticalChance = 10f;

            if (Random.Range(0, 100f) < criticalChance)
            {
                //critical hit here
                PlayerHealth playerDamageCrit = other.GetComponent<PlayerHealth>();
                if (playerDamageCrit.Health <= (10 * bulletModifier) && playerDamageCrit.alive == true && playerHealth != null)
                {
                    playerHealth.PlayersKilled();
                }
                playerDamageCrit.TakeDamage(10 * bulletModifier);
                PhotonNetwork.Destroy(gameObject);
            }

            else
            {
                PlayerHealth playerDamage = other.GetComponent<PlayerHealth>();
                if (playerDamage.Health <= (5 * bulletModifier) && playerDamage.alive == true && playerHealth != null)
                {
                    playerHealth.PlayersKilled();
                }
                playerDamage.TakeDamage(5 * bulletModifier);
                PhotonNetwork.Destroy(gameObject);
            }
        }

    }

    IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(5);
        PhotonNetwork.Destroy(gameObject);
    }
    IEnumerator DestroyBulletCollision()
    {
        yield return new WaitForSeconds(0.15f);
        PhotonNetwork.Destroy(gameObject);
    }
}
