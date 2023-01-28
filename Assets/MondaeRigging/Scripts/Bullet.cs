using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UIElements;

public class Bullet : MonoBehaviourPunCallbacks
{
    public GameObject bulletOwner;
    public PlayerHealth playerHealth;
    public bool playerBullet = false;
    public int bulletModifier;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyBullet());
    }

    // Update is called once per frame
    void OnCollisionEnter(Collision collision)
    {
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

        if (other.CompareTag("Enemy") || other.CompareTag("BossEnemy"))
        {
            float criticalChance = 10f;

            //call it at random probability
            if (Random.Range(0, 100f) < criticalChance)
            {
                //critical hit here
                FollowAI enemyDamageCrit = other.GetComponent<FollowAI>();
                if (enemyDamageCrit.Health <= (40 * bulletModifier) && enemyDamageCrit.alive == true && playerHealth != null)
                {
                    playerHealth.EnemyKilled();
                    enemyDamageCrit.TakeDamage((40 * bulletModifier));
                }
                else if (enemyDamageCrit.Health > (40 * bulletModifier) && enemyDamageCrit.alive == true && playerHealth != null)
                {
                    enemyDamageCrit.TakeDamage((40 * bulletModifier));
                }
                    PhotonNetwork.Destroy(gameObject);
            }

            else
            {
                FollowAI enemyDamage = other.GetComponent<FollowAI>();
                if (enemyDamage.Health <= (20 * bulletModifier) && enemyDamage.alive == true && playerHealth != null)
                {
                    playerHealth.EnemyKilled();
                    enemyDamage.TakeDamage((20 * bulletModifier));
                }
                else if (enemyDamage.Health > (20 * bulletModifier) && enemyDamage.alive == true && playerHealth != null)
                {
                    enemyDamage.TakeDamage((20 * bulletModifier));
                }
                    PhotonNetwork.Destroy(gameObject);
            }
        }

        if (other.CompareTag("Security"))
        {
            float criticalChance = 30f;

            //cal it at random probability
            if (Random.Range(0, 100f) < criticalChance)
            {
                //critical hit here
                DroneHealth enemyDamageCrit = other.GetComponent<DroneHealth>();
                enemyDamageCrit.TakeDamage((50 * bulletModifier));
                    PhotonNetwork.Destroy(gameObject);
            }

            else
            {
                DroneHealth enemyDamage = other.GetComponent<DroneHealth>();
                enemyDamage.TakeDamage((5 * bulletModifier));
                    PhotonNetwork.Destroy(gameObject);
            }
        }


        if (other.CompareTag("Player"))
        {
            float criticalChance = 10f;

            if (Random.Range(0, 100f) < criticalChance)
            {
                //critical hit here
                PlayerHealth playerDamageCrit = other.GetComponent<PlayerHealth>();
                if (playerDamageCrit.Health <= (2 * bulletModifier) && playerDamageCrit.alive == true && playerHealth != null)
                {
                    playerHealth.PlayersKilled();
                }
                playerDamageCrit.TakeDamage((2 * bulletModifier));
                    PhotonNetwork.Destroy(gameObject);
            }

            else
            {
                PlayerHealth playerDamage = other.GetComponent<PlayerHealth>();
                if (playerDamage.Health <= (1 * bulletModifier) && playerDamage.alive == true && playerHealth != null)
                {
                    playerHealth.PlayersKilled();
                }
                playerDamage.TakeDamage((1 * bulletModifier));
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
        yield return new WaitForSeconds(0.75f);
        PhotonNetwork.Destroy(gameObject);
    }
}
