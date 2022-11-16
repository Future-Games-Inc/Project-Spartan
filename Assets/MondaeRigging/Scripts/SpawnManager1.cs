using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using PathologicalGames;

public class SpawnManager1 : MonoBehaviour
{
    public GameObject enemyAI;
    public GameObject securityAI;
    public GameObject reactor;
    public GameObject health;

    public Transform[] enemyDrop;
    public Transform[] reactorDrop;
    public Transform[] healthDrop;

    public int enemyCount;
    public int securityCount;
    public int reactorCount;
    public int healthCount;

    public bool spawnEnemy = true;
    public bool spawnSecurity = true;
    public bool spawnReactor = true;
    public bool spawnHealth = true;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (spawnEnemy == true && enemyCount < 10)
        {
            StartCoroutine(EnemySpawn());
        }
        if (spawnSecurity == true && securityCount < 5)
        {
            StartCoroutine(SecuritySpawn());
        }
        if (spawnReactor == true && reactorCount < 1)
        {
            StartCoroutine(ReactorSpawn());
        }
        if (spawnHealth == true && healthCount < 4)
        {
            StartCoroutine(HealthSpawn());
        }
    }

    IEnumerator EnemySpawn()
    {
        while (enemyCount < 10)
        {
            spawnEnemy = false;
            PhotonNetwork.Instantiate(enemyAI.name, enemyDrop[Random.Range(0, enemyDrop.Length)].position, Quaternion.identity);
            enemyCount += 1;
            yield return new WaitForSeconds(10f);
            spawnEnemy = true;
        }
        //enemySpawnTimer = 20f;
    }

    IEnumerator SecuritySpawn()
    {
        while (securityCount < 5)
        {
            spawnSecurity = false;
            PhotonNetwork.Instantiate(securityAI.name, enemyDrop[Random.Range(0, enemyDrop.Length)].position, Quaternion.identity);
            securityCount += 1;
            yield return new WaitForSeconds(15f);
            spawnSecurity = true;
        }
        //securitySpawnTimer = 10f;
    }

    IEnumerator ReactorSpawn()
    {
        while (reactorCount < 1)
        {
            spawnReactor = false;
            PhotonNetwork.Instantiate(reactor.name, reactorDrop[Random.Range(0, reactorDrop.Length)].position, Quaternion.identity);
            reactorCount += 1;
            yield return new WaitForSeconds(30f);
            spawnReactor = true;
        }
    }

    IEnumerator HealthSpawn()
    {
        while (healthCount < 4)
        {
            spawnHealth = false;
            PhotonNetwork.Instantiate(health.name, healthDrop[Random.Range(0, healthDrop.Length)].position, Quaternion.identity);
            healthCount += 1;
            yield return new WaitForSeconds(35f);
            spawnHealth = true;
        }
    }
}
