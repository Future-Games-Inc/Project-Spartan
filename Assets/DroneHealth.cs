using PathologicalGames;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneHealth : MonoBehaviour
{
    public int Health = 100;
    public GameObject xpDrop;
    public GameObject xpDropExtra;
    public SpawnManager1 enemyCounter;
    public bool alive;
    public Transform[] lootSpawn;
    public float xpDropRate;

    public AudioSource audioSource;
    public AudioClip bulletHit;

    public GameObject explosionEffect;
    public EnemyHealthBar healthBar;

    // Start is called before the first frame update
    void OnEnable()
    {
        explosionEffect.SetActive(false);
        enemyCounter = GameObject.FindGameObjectWithTag("spawnManager").GetComponent<SpawnManager1>();
        alive = true;
        healthBar.SetMaxHealth(Health);
    }

    // Update is called once per frame
    void Update()
    {
        if (Health <= 0 && alive == true)
        {
            alive = false;
            StartCoroutine(KillDrone());
        }

    }

    public void TakeDamage(int damage)
    {
        audioSource.PlayOneShot(bulletHit);
        Health -= damage;
        healthBar.SetCurrentHealth(Health);
    }

    IEnumerator KillDrone()
    {
        yield return new WaitForSeconds(0);
        enemyCounter.securityCount -= 1;
        StartCoroutine(DestroyEnemy());
    }

    IEnumerator DestroyEnemy()
    {
        yield return new WaitForSeconds(0);
        explosionEffect.SetActive(true);

        foreach (Transform t in lootSpawn)
        {
            xpDropRate = 10f;
            if (Random.Range(0, 100f) < xpDropRate)
            {
                PhotonNetwork.Instantiate(xpDropExtra.name, transform.position, Quaternion.identity);
            }
            else
                PhotonNetwork.Instantiate(xpDrop.name, transform.position, Quaternion.identity);
        }
        PhotonNetwork.Instantiate(xpDrop.name, transform.position, Quaternion.identity);

        yield return new WaitForSeconds(.75f);
        PhotonNetwork.Destroy(gameObject);
    }
}

