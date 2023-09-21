using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnManager1 : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyAI;
    [SerializeField] private GameObject[] enemyBoss;
    [SerializeField] private GameObject[] securityAI;
    [SerializeField] private GameObject reactor;
    [SerializeField] private GameObject supplyDrop;
    [SerializeField] private GameObject health;
    [SerializeField] private GameObject[] artifacts;
    [SerializeField] private GameObject[] bombs;
    public Transform reactorSpawnLocation;
    public GameObject reactorPlaceholder;


    [SerializeField] private MatchEffects matchProps;

    [SerializeField] private int enemyCountMax = 5;
    [SerializeField] private int securityCountMax = 2;
    [SerializeField] private int reactorCountMax = 1;
    [SerializeField] private int healthCountMax = 1;
    [SerializeField] private int enemiesKilledForBossSpawn = 5;
    [SerializeField] private int bombsCountMax = 10;
    [SerializeField] private int artifactCountMax = 10;

    private int enemyCount;
    private int securityCount;
    private int reactorCount;
    private int healthCount;
    public int enemiesKilled;
    private int artifactCount;
    private int bombsCount;

    private bool spawnEnemy = true;
    private bool spawnSecurity = true;
    private bool spawnHealth = true;
    private bool spawnBombs = false;
    private bool spawnArtifacts = true;
    private bool coroutinesStarted = false;


    public float spawnRadius;   // Maximum spawn radius

    public NavMeshSurface navMeshSurface;
    public Transform[] spawnPositions;  // Dynamic list for valid positions
    public float bufferRadius = .15f;


    void Start()
    {
        spawnRadius = 5000f;


        if (!coroutinesStarted)
        {
            StartCoroutine(SpawnEnemies());
            StartCoroutine(SpawnSecurity());
            StartCoroutine(SpawnReactor());
            StartCoroutine(SpawnHealth());
            StartCoroutine(SpawnBoss());
            StartCoroutine(SpawnBombs());
            StartCoroutine(SpawnArtifacts());

            coroutinesStarted = true;
        }
    }

    GameObject[] ShuffleArray(GameObject[] array)
    {
        GameObject[] shuffledArray = (GameObject[])array.Clone();

        for (int i = shuffledArray.Length - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            GameObject temp = shuffledArray[i];
            shuffledArray[i] = shuffledArray[randomIndex];
            shuffledArray[randomIndex] = temp;
        }

        return shuffledArray;
    }

    Transform[] ShuffleSpawns(Transform[] array)
    {
        Transform[] shuffledArray = (Transform[])array.Clone();

        for (int i = shuffledArray.Length - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            Transform temp = shuffledArray[i];
            shuffledArray[i] = shuffledArray[randomIndex];
            shuffledArray[randomIndex] = temp;
        }

        return shuffledArray;
    }


    IEnumerator SpawnEnemies()
    {
        while (spawnEnemy && enemyCount < enemyCountMax)
        {
            while (!matchProps.startMatchBool)
                yield return null;

            GameObject[] enemies = ShuffleArray(enemyAI);

            spawnEnemy = false;

            Transform[] spawnPosition = ShuffleSpawns(spawnPositions);

            GameObject enemyCharacter = enemies[0];
            Instantiate(enemyCharacter, spawnPosition[0].position, Quaternion.identity);

            enemyCount++;

            yield return new WaitForSeconds(2);
            spawnEnemy = true;

            yield return new WaitForSeconds(.25f);
        }
    }


    IEnumerator SpawnSecurity()
    {
        while (spawnSecurity && securityCount < securityCountMax)
        {
            while (!matchProps.startMatchBool)
                yield return null;

            yield return new WaitForSeconds(1);

            GameObject[] enemies = ShuffleArray(securityAI);

            spawnSecurity = false;

            Transform[] spawnPosition = ShuffleSpawns(spawnPositions);

            GameObject securityDrone = enemies[0];
            Instantiate(securityDrone, spawnPosition[0].position, Quaternion.identity);

            securityCount++;

            yield return new WaitForSeconds(3);
            spawnSecurity = true;

            yield return new WaitForSeconds(.25f);
        }
    }


    public IEnumerator SpawnSupplyDrop()
    {
        yield return new WaitForSeconds(1);

        Transform[] spawnPosition = ShuffleSpawns(spawnPositions);

        Vector3 newPosition = new Vector3(spawnPosition[0].position.x, spawnPosition[0].position.y + 20, spawnPosition[0].position.z);

        Instantiate(supplyDrop, newPosition, Quaternion.identity);
    }

    IEnumerator SpawnReactor()
    {
        while (true)
        {
            while (!matchProps.startMatchBool && !matchProps.spawnReactor)
                yield return null;

            if (matchProps.spawnReactor && reactorCount < reactorCountMax)
            {
                reactorSpawnLocation = reactorPlaceholder.transform;
                Destroy(reactorPlaceholder);

                Instantiate(reactor, reactorSpawnLocation.position, Quaternion.identity);

                reactorCount++;

                yield return new WaitForSeconds(25);
            }
            yield return new WaitForSeconds(.25f);
        }
    }

    IEnumerator SpawnBombs()
    {
        while (spawnBombs && bombsCount < bombsCountMax)
        {
            while (!matchProps.startMatchBool)
                yield return null;

            yield return new WaitForSeconds(1);

            GameObject[] bomb = ShuffleArray(bombs);

            spawnBombs = false;

            Transform[] spawnPosition = ShuffleSpawns(spawnPositions);

            GameObject securityDrone = bomb[0];
            Instantiate(securityDrone, spawnPosition[0].position, Quaternion.identity);

            bombsCount++;

            yield return new WaitForSeconds(12);
            spawnBombs = true;

            yield return new WaitForSeconds(.25f);
        }
    }

    IEnumerator SpawnArtifacts()
    {
        while (spawnArtifacts && artifactCount < artifactCountMax)
        {
            while (!matchProps.startMatchBool)
                yield return null;

            yield return new WaitForSeconds(1);

            GameObject[] artifact = ShuffleArray(artifacts);

            spawnArtifacts = false;

            Transform[] spawnPosition = ShuffleSpawns(spawnPositions);

            GameObject securityDrone = artifact[0];
            Instantiate(securityDrone, spawnPosition[0].position, Quaternion.identity);

            artifactCount++;

            yield return new WaitForSeconds(20);
            spawnArtifacts = true;

            yield return new WaitForSeconds(.25f);
        }
    }

    IEnumerator SpawnHealth()
    {
        while (spawnHealth && healthCount < healthCountMax)
        {
            while (!matchProps.startMatchBool)
                yield return null;

            spawnHealth = false;

            Transform[] spawnPosition = ShuffleSpawns(spawnPositions);

            Instantiate(health, spawnPosition[0].position, Quaternion.identity);

            healthCount++;

            yield return new WaitForSeconds(25);
            spawnHealth = true;

            yield return new WaitForSeconds(.25f);
        }
    }

    IEnumerator SpawnBoss()
    {
        while (true)
        {
            while (!matchProps.startMatchBool)
                yield return null;

            if (enemiesKilled >= enemiesKilledForBossSpawn)
            {
                GameObject[] bosses = ShuffleArray(enemyBoss);
                Transform[] spawnPosition = ShuffleSpawns(spawnPositions);

                GameObject enemyCharacterBoss = bosses[0];
                Instantiate(enemyCharacterBoss, spawnPosition[0].position, Quaternion.identity);

                enemiesKilled = 0;
                yield return new WaitForSeconds(10);
            }

            yield return new WaitForSeconds(.25f);
        }
    }

    public void UpdateEnemy()
    {
        enemyCount--;
    }

    public void UpdateEnemyCount()
    {
        enemiesKilled++;
    }

    public void UpdateArtifact()
    {
        artifactCount--;
    }

    public void UpdateBombs()
    {
        bombsCount--;
    }

    public void UpdateSecurity()
    {
        securityCount--;
    }

    public void UpdateHealthCount()
    {
        healthCount--;
    }
}