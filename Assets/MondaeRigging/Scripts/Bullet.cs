using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UIElements;

public class Bullet : MonoBehaviour
{
    public GameObject bulletOwner;
    public PlayerHealth playerHealth;
    public bool alive;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyBullet());
        bulletOwner = this.transform.parent.gameObject;
        if (bulletOwner.CompareTag("Player"))
        {
            playerHealth = bulletOwner.GetComponentInParent<PlayerHealth>();
        }
        else
        {
            playerHealth = null;
        }
        alive = true;
    }

    // Update is called once per frame
    void OnCollisionEnter(Collision collision)
    {
        PhotonNetwork.Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            float criticalChance = 10f;

            //cal it at random probability
            if (Random.Range(0, 100f) < criticalChance)
            {
                //critical hit here
                FollowAI enemyDamageCrit = other.GetComponent<FollowAI>();
                EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
                if (enemyDamageCrit.Health <= 40 && enemyHealth.alive == true && playerHealth != null)
                {
                    playerHealth.EnemyKilled();
                }
                else
                enemyDamageCrit.TakeDamage(40);
                PhotonNetwork.Destroy(gameObject);
            }
            FollowAI enemyDamage = other.GetComponent<FollowAI>();
            EnemyHealth enemyHealth2 = other.GetComponent<EnemyHealth>();
            if (enemyDamage.Health <= 20 && enemyHealth2.alive == true && playerHealth != null)
            {
                playerHealth.EnemyKilled();
            }
            enemyDamage.TakeDamage(20);
            PhotonNetwork.Destroy(gameObject);
        }

        if (other.CompareTag("Security"))
        {
            float criticalChance = 30f;

            //cal it at random probability
            if (Random.Range(0, 100f) < criticalChance)
            {
                //critical hit here
                DroneHealth enemyDamageCrit = other.GetComponent<DroneHealth>();
                if (enemyDamageCrit.Health <= 50 && enemyDamageCrit.alive == true && playerHealth != null)
                {
                    playerHealth.EnemyKilled();
                }
                enemyDamageCrit.TakeDamage(50);
                PhotonNetwork.Destroy(gameObject);
            }
            DroneHealth enemyDamage = other.GetComponent<DroneHealth>();
            if (enemyDamage.Health <= 25 && enemyDamage.alive == true && playerHealth != null)
            {
                playerHealth.EnemyKilled();
            }
            enemyDamage.TakeDamage(25);
            PhotonNetwork.Destroy(gameObject);
        }

        if (other.CompareTag("Player"))
        {
            float criticalChance = 15f;

            if (Random.Range(0, 100f) < criticalChance)
            {
                //critical hit here
                PlayerHealth playerDamageCrit = other.GetComponent<PlayerHealth>();
                if (playerDamageCrit.Health <= 3 && playerDamageCrit.alive == true && playerHealth != null)
                {
                    playerHealth.PlayersKilled();
                }
                playerDamageCrit.TakeDamage(3);
                PhotonNetwork.Destroy(gameObject);
            }
            PlayerHealth playerDamage = other.GetComponent<PlayerHealth>();
            if (playerDamage.Health <= 1 && playerDamage.alive == true && playerHealth != null)
            {
                playerHealth.PlayersKilled();
            }
            playerDamage.TakeDamage(1);
            PhotonNetwork.Destroy(gameObject);
        }
    }

    IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(5);
        PhotonNetwork.Destroy(gameObject);
    }
}
