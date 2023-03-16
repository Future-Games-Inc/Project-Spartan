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

    public MatchEffects matchProps;

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
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(EnemySpawn());
            StartCoroutine(SecuritySpawn());
            StartCoroutine(ReactorSpawn());
            StartCoroutine(HealthSpawn());
            StartCoroutine(SpawnBoss());
        }
    }

    private void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator EnemySpawn()
    {
        while (spawnEnemy == true && enemyCount < enemyCountMax)
        {
            if (matchProps.startMatchBool == true)
            {
                spawnEnemy = false;
                GameObject enemyCharacter = enemyAI[Random.Range(0, enemyAI.Length)].gameObject;
                PhotonNetwork.Instantiate(enemyCharacter.name, enemyDrop[Random.Range(0, enemyDrop.Length)].position, Quaternion.identity, 0);
                enemyCount += 1;
                yield return new WaitForSeconds(10f);
                spawnEnemy = true;
            }
            yield return new WaitForSeconds(1);
        }
    }

    IEnumerator SecuritySpawn()
    {
        while (spawnSecurity == true && securityCount < securityCountMax)
        {
            if (matchProps.startMatchBool == true)
            {
                spawnSecurity = false;
                PhotonNetwork.Instantiate(securityAI.name, enemyDrop[Random.Range(0, enemyDrop.Length)].position, Quaternion.identity, 0);
                securityCount += 1;
                yield return new WaitForSeconds(15f);
                spawnSecurity = true;
            }
            yield return new WaitForSeconds(1);
        }
    }

    IEnumerator ReactorSpawn()
    {
        while (spawnReactor == true && reactorCount < 1)
        {
            if (matchProps.startMatchBool == true)
            {
                spawnReactor = false;
                PhotonNetwork.Instantiate(reactor.name, reactorDrop[Random.Range(0, reactorDrop.Length)].position, Quaternion.identity, 0);
                reactorCount += 1;
                yield return new WaitForSeconds(30f);
                spawnReactor = true;
            }
            yield return new WaitForSeconds(1);
        }
    }

    IEnumerator HealthSpawn()
    {
        while (spawnHealth == true && healthCount < healthCountMax)
        {
            if (matchProps.startMatchBool == true)
            {
                spawnHealth = false;
                PhotonNetwork.Instantiate(health.name, healthDrop[Random.Range(0, healthDrop.Length)].position, Quaternion.identity, 0);
                healthCount += 1;
                yield return new WaitForSeconds(35f);
                spawnHealth = true;
            }
            yield return new WaitForSeconds(1);
        }
    }
    IEnumerator SpawnBoss()
    {
        while (spawnBoss == true && enemiesKilled > 5)
        {
            if (matchProps.startMatchBool == true)
            {
                spawnBoss = false;
                GameObject enemyCharacterBoss = enemyBoss[Random.Range(0, enemyBoss.Length)].gameObject;
                PhotonNetwork.Instantiate(enemyCharacterBoss.name, enemyDrop[Random.Range(0, enemyDrop.Length)].position, Quaternion.identity, 0);
                enemiesKilled = 2;
                yield return new WaitForSeconds(45f);
                spawnBoss = true;
            }
            yield return new WaitForSeconds(1);
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
}
