using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UIElements;

public class Bullet2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyBullet());
    }

    // Update is called once per frame
    void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            float criticalChance = 20f;

            if (Random.Range(0, 100f) < criticalChance)
            {
                //critical hit here
                PlayerStats playerDamageCrit = other.GetComponent<PlayerStats>();
                playerDamageCrit.TakeDamage(30);
                Destroy(gameObject);
            }
            PlayerStats playerDamage = other.GetComponent<PlayerStats>();
            playerDamage.TakeDamage(10);
            Destroy(gameObject);
        }
    }

    IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }
}
