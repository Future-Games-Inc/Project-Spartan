using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UIElements;

public class Bullet : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyBullet());
    }

    // Update is called once per frame
    void OnCollisionEnter(Collision collision)
    {
        PhotonNetwork.Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            float criticalChance = 30f;

            //cal it at random probability
            if (Random.Range(0, 100f) < criticalChance)
            {
                //critical hit here
                EnemyHealth enemyDamageCrit = other.GetComponent<EnemyHealth>();
                enemyDamageCrit.TakeDamage(40);
                PhotonNetwork.Destroy(gameObject);
            }
            EnemyHealth enemyDamage = other.GetComponent<EnemyHealth>();
            enemyDamage.TakeDamage(20);
            PhotonNetwork.Destroy(gameObject);
        }

        if(other.CompareTag("Player"))
        {
            float criticalChance = 20f;

            if (Random.Range(0, 100f) < criticalChance)
            {
                //critical hit here
                EnemyHealth enemyDamageCrit = other.GetComponent<EnemyHealth>();
                enemyDamageCrit.TakeDamage(30);
                PhotonNetwork.Destroy(gameObject);
            }
            PlayerHealth playerDamage = other.GetComponent<PlayerHealth>();
            playerDamage.TakeDamage(10);
            PhotonNetwork.Destroy(gameObject);
        }
    }

    IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(5);
        PhotonNetwork.Destroy(gameObject);
    }
}
