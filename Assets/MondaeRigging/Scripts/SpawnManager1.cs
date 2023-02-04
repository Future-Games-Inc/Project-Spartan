using System.Collections;
using UnityEngine;
using Photon.Pun;

public class SpawnManager1 : MonoBehaviourPunCallbacks
{
    public GameObject[] enemyAI;
    public GameObject[] enemyBoss;
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
    public int enemyCountMax;
    public int securityCountMax;
    public int reactorCountMax;
    public int healthCountMax;
    public int enemiesKilled;

    public bool spawnEnemy = true;
    public bool spawnSecurity = true;
    public bool spawnReactor = true;
    public bool spawnHealth = true;
    public bool spawnBoss = true;

    void Start()
    {

    }

    private void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        photonView.RPC("RPC_Instantiate", RpcTarget.All);
    }

    IEnumerator EnemySpawn()
    {
        while (enemyCount < enemyCountMax)
        {
            spawnEnemy = false;
            GameObject enemyCharacter = enemyAI[Random.Range(0, enemyAI.Length)].gameObject;
            PhotonNetwork.Instantiate(enemyCharacter.name, enemyDrop[Random.Range(0, enemyDrop.Length)].position, Quaternion.identity, 0);
            enemyCount += 1;
            yield return new WaitForSeconds(10f);
            spawnEnemy = true;
        }
        //enemySpawnTimer = 20f;
    }

    IEnumerator SecuritySpawn()
    {
        while (securityCount < securityCountMax)
        {
            spawnSecurity = false;
            PhotonNetwork.Instantiate(securityAI.name, enemyDrop[Random.Range(0, enemyDrop.Length)].position, Quaternion.identity, 0);
            securityCount += 1;
            yield return new WaitForSeconds(15f);
            spawnSecurity = true;
        }
        //securitySpawnTimer = 10f;
    }

    IEnumerator ReactorSpawn()
    {
        while (reactorCount < reactorCountMax)
        {
            spawnReactor = false;
            PhotonNetwork.Instantiate(reactor.name, reactorDrop[Random.Range(0, reactorDrop.Length)].position, Quaternion.identity, 0);
            reactorCount += 1;
            yield return new WaitForSeconds(30f);
            spawnReactor = true;
        }
    }

    IEnumerator HealthSpawn()
    {
        while (healthCount < healthCountMax)
        {
            spawnHealth = false;
            PhotonNetwork.Instantiate(health.name, healthDrop[Random.Range(0, healthDrop.Length)].position, Quaternion.identity, 0);
            healthCount += 1;
            yield return new WaitForSeconds(35f);
            spawnHealth = true;
        }
    }
    IEnumerator SpawnBoss()
    {
        while (enemiesKilled > 5)
        {
            spawnBoss = false;
            GameObject enemyCharacterBoss = enemyBoss[Random.Range(0, enemyBoss.Length)].gameObject;
            PhotonNetwork.Instantiate(enemyCharacterBoss.name, enemyDrop[Random.Range(0, enemyDrop.Length)].position, Quaternion.identity, 0);
            enemiesKilled = 2;
            yield return new WaitForSeconds(45f);
            spawnBoss = true;
        }
    }

    [PunRPC]
    public void RPC_UpdateEnemy()
    {
        enemyCount--;
    }

    [PunRPC]
    public void RPC_UpdateSecurity()
    {
        securityCount--;
    }

    [PunRPC]
    public void RPC_UpdateHealth()
    {
        healthCount--;
    }

    [PunRPC]
    public void RPC_UpdateEnemyCount()
    {
        enemiesKilled++;
    }

    [PunRPC]
    void RPC_Instantiate()
    {
        if (spawnEnemy == true && enemyCount < enemyCountMax)
        {
            StartCoroutine(EnemySpawn());
        }
        if (spawnSecurity == true && securityCount < securityCountMax)
        {
            StartCoroutine(SecuritySpawn());
        }
        if (spawnReactor == true && reactorCount < 1)
        {
            StartCoroutine(ReactorSpawn());
        }
        if (spawnHealth == true && healthCount < healthCountMax)
        {
            StartCoroutine(HealthSpawn());
        }

        if (spawnBoss == true && enemiesKilled > 5)
        {
            StartCoroutine(SpawnBoss());
        }
    }
}
