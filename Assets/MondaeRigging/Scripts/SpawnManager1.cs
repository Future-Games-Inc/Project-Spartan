using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnManager1 : MonoBehaviour
{
    public GameObject enemyAI;
    public GameObject securityAI;
    public GameObject weaponCache;

    public Transform[] enemyDrop;
    public Transform[] weaponDrop;

    public int enemyCount;
    public int securityCount;
    public int weaponCount;

    public bool spawnEnemy = true;
    public bool spawnSecurity = true;
    public bool spawnCache = true;

    //public float enemySpawnTimer = 20f;
    //public float securitySpawnTimer = 10f;
    //public float cacheSpawnTimer = 60f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //enemySpawnTimer -= Time.deltaTime;
        //securitySpawnTimer -= Time.deltaTime;
        //cacheSpawnTimer -= Time.deltaTime;

        //if (enemyCount < 20 && enemySpawnTimer <= 0)
        //{
        //    StartCoroutine(EnemySpawn());
        //}
        //if (securityCount < 10 && securitySpawnTimer <= 0)
        //{
        //    StartCoroutine(SecuritySpawn());
        //}
        //if (weaponCount < 5 && cacheSpawnTimer <= 0)
        //{
        //    StartCoroutine(CacheSpawn());
        //}
        if (spawnEnemy == true)
        {
            StartCoroutine(EnemySpawn());
        }
        if (spawnSecurity == true)
        {
            StartCoroutine(SecuritySpawn());
        }
        if (spawnCache == true)
        {
            StartCoroutine(CacheSpawn());
        }
        //StartCoroutine(CacheSpawn());
        //StartCoroutine(SecuritySpawn());
        //StartCoroutine(EnemySpawn());
    }

    IEnumerator EnemySpawn()
    {
        while (enemyCount <= 20)
        {
            spawnEnemy = false;
            PhotonNetwork.Instantiate("enemyAI", enemyDrop[Random.Range(0, enemyDrop.Length)].position, Quaternion.identity);
            enemyCount += 1;
            yield return new WaitForSeconds(3f);
            spawnEnemy = true;
        }
        //enemySpawnTimer = 20f;
    }

    IEnumerator SecuritySpawn()
    {
        while (securityCount <= 10)
        {
            spawnSecurity = false;
            PhotonNetwork.Instantiate("securityAI", enemyDrop[Random.Range(0, enemyDrop.Length)].position, Quaternion.identity);
            securityCount += 1;
            yield return new WaitForSeconds(5f);
            spawnSecurity = true;
        }
        //securitySpawnTimer = 10f;
    }

    IEnumerator CacheSpawn()
    {
        while (weaponCount <= 5)
        {
            spawnCache = false;
            PhotonNetwork.Instantiate("weaponCache", weaponDrop[Random.Range(0, weaponDrop.Length)].position, Quaternion.identity);
            weaponCount += 1;
            yield return new WaitForSeconds(6f);
            spawnCache = false;
        }
        //cacheSpawnTimer = 60f;
    }
}
