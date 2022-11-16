using PathologicalGames;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneHealth : MonoBehaviour, IPunPrefabPool
{
    public float Health = 100f;
    public GameObject xpDrop;
    public SpawnManager1 enemyCounter;
    public bool alive;

    public AudioSource audioSource;
    public AudioClip bulletHit;

    public GameObject explosionEffect;

    // Start is called before the first frame update
    void Start()
    {
        explosionEffect.SetActive(false);
        enemyCounter = GameObject.FindGameObjectWithTag("spawnManager").GetComponent<SpawnManager1>();
        alive = true;
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
        PhotonNetwork.Instantiate(xpDrop.name, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(.75f);
        PhotonNetwork.Destroy(gameObject);
    }

    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        throw new System.NotImplementedException();
    }

    public void Destroy(GameObject gameObject)
    {
        throw new System.NotImplementedException();
    }
}
