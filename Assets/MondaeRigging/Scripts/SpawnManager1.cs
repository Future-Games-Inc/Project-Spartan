using System.Collections;
using UnityEngine;
using Photon.Pun;

public class SpawnManager1 : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject[] enemyAI;
    [SerializeField] private GameObject[] enemyBoss;
    [SerializeField] private GameObject securityAI;
    [SerializeField] private GameObject reactor;
    [SerializeField] private GameObject health;

    [SerializeField] private Transform[] enemyDrop;
    [SerializeField] private Transform[] reactorDrop;
    [SerializeField] private Transform[] healthDrop;

    [SerializeField] private MatchEffects matchProps;

    [SerializeField] private int enemyCountMax = 5;
    [SerializeField] private int securityCountMax = 2;
    [SerializeField] private int reactorCountMax = 1;
    [SerializeField] private int healthCountMax = 1;
    [SerializeField] private int enemiesKilledForBossSpawn = 5;

    private int enemyCount;
    private int securityCount;
    private int reactorCount;
    private int healthCount;
    private int enemiesKilled;

    private bool spawnEnemy = true;
    private bool spawnSecurity = true;
    private bool spawnReactor = true;
    private bool spawnHealth = true;
    private bool spawnBoss = true;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(SpawnEnemies());
            StartCoroutine(SpawnSecurity());
            StartCoroutine(SpawnReactor());
            StartCoroutine(SpawnHealth());
            StartCoroutine(SpawnBoss());
        }
    }

    private IEnumerator SpawnEnemies()
    {
        while (spawnEnemy && enemyCount < enemyCountMax)
        {
            yield return new WaitUntil(() => matchProps.startMatchBool);

            spawnEnemy = false;

            GameObject enemyCharacter = enemyAI[Random.Range(0, enemyAI.Length)];
            PhotonNetwork.Instantiate(enemyCharacter.name, enemyDrop[Random.Range(0, enemyDrop.Length)].position, Quaternion.identity, 0);

            enemyCount++;

            yield return new WaitForSeconds(10f);

            spawnEnemy = true;

            yield return new WaitForSeconds(1);
        }
    }

    private IEnumerator SpawnSecurity()
    {
        while (spawnSecurity && securityCount < securityCountMax)
        {
            yield return new WaitUntil(() => matchProps.startMatchBool);

            spawnSecurity = false;

            PhotonNetwork.Instantiate(securityAI.name, enemyDrop[Random.Range(0, enemyDrop.Length)].position, Quaternion.identity, 0);

            securityCount++;

            yield return new WaitForSeconds(15f);

            spawnSecurity = true;

            yield return new WaitForSeconds(1);
        }
    }

    private IEnumerator SpawnReactor()
    {
        while (spawnReactor && reactorCount < reactorCountMax)
        {
            yield return new WaitUntil(() => matchProps.startMatchBool && matchProps.spawnReactor);

            spawnReactor = false;

            PhotonNetwork.Instantiate(reactor.name, reactorDrop[Random.Range(0, reactorDrop.Length)].position, Quaternion.identity, 0);

            reactorCount++;

            yield return new WaitForSeconds(30f);

            spawnReactor = true;

            yield return new WaitForSeconds(1);
        }
    }

    private IEnumerator SpawnHealth()
    {
        while (spawnHealth && healthCount < healthCountMax)
        {
            yield return new WaitUntil(() => matchProps.startMatchBool);

            spawnHealth = false;

            PhotonNetwork.Instantiate(health.name, healthDrop[Random.Range(0, healthDrop.Length)].position, Quaternion.identity, 0);

            healthCount++;

            yield return new WaitForSeconds(35f);

            spawnHealth = true;

            yield return new WaitForSeconds(1);
        }
    }

    private IEnumerator SpawnBoss()
    {
        while (spawnBoss)
        {
            yield return new WaitUntil(() => matchProps.startMatchBool);

            if (enemiesKilled >= enemiesKilledForBossSpawn)
            {
                spawnBoss = false;

                GameObject enemyCharacterBoss = enemyBoss[Random.Range(0, enemyBoss.Length)];
                PhotonNetwork.Instantiate(enemyCharacterBoss.name, enemyDrop[Random.Range(0, enemyDrop.Length)].position, Quaternion.identity, 0);

                enemiesKilled = 0;

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