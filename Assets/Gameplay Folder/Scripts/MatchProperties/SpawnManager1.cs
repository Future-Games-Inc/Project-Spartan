using System.Collections;
using System.Collections.Generic;
using Umbrace.Unity.PurePool;
using UnityEngine;
using UnityEngine.AI;

public class SpawnManager1 : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyAI;
    [SerializeField] private GameObject[] enemyBoss;
    [SerializeField] private GameObject[] securityAI;
    [SerializeField] private GameObject reactor;
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
    List<Vector3> spawnPositions = new List<Vector3>();  // Dynamic list for valid positions
    float bufferRadius = .25f;  // Replace this with the buffer radius you want


    void Start()
    {
        spawnRadius = 1000f;
        

        if (!coroutinesStarted)
        {
            StartCoroutine(SpawnEnemies());
            StartCoroutine(SpawnSecurity());
            StartCoroutine(SpawnReactor());
            StartCoroutine(SpawnHealth());
            StartCoroutine(SpawnBoss());

            coroutinesStarted = true;
        }

        // Generate multiple random positions within the spawn radius
        for (int i = 0; i < 300; i++)  // Increase number of attempts to 100 or more
        {
            Vector3 randomPosition = Random.insideUnitSphere * spawnRadius;
            randomPosition += transform.position;

            NavMeshHit hit;
            int walkableMask = 1 << NavMesh.GetAreaFromName("Walkable");

            if (NavMesh.SamplePosition(randomPosition, out hit, spawnRadius / 2, walkableMask))
            {
                // Check if the hit position is sufficiently far from the closest edge
                NavMeshHit edgeHit;
                if (NavMesh.FindClosestEdge(hit.position, out edgeHit, walkableMask))
                {
                    if (Vector3.Distance(hit.position, edgeHit.position) > bufferRadius)
                    {
                        spawnPositions.Add(hit.position);
                    }
                }
            }
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

    Vector3[] ShuffleSpawns(Vector3[] array)
    {
        Vector3[] shuffledArray = (Vector3[])array.Clone();

        for (int i = shuffledArray.Length - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            Vector3 temp = shuffledArray[i];
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
            if (spawnPositions.Count == 0)
            {
                yield return new WaitForSeconds(1);
                continue;
            }

            spawnEnemy = false;

            Vector3 spawnPosition = spawnPositions[UnityEngine.Random.Range(0, spawnPositions.Count)];

            GameObject enemyCharacter = enemies[0];
            Instantiate(enemyCharacter, spawnPosition, Quaternion.identity);

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
            if (spawnPositions.Count == 0)
            {
                yield return new WaitForSeconds(1);
                continue;
            }

            spawnSecurity = false;

            Vector3 spawnPosition = spawnPositions[UnityEngine.Random.Range(0, spawnPositions.Count)];

            GameObject securityDrone = enemies[0];
            Instantiate(securityDrone, spawnPosition, Quaternion.identity);

            securityCount++;

            yield return new WaitForSeconds(3);
            spawnSecurity = true;

            yield return new WaitForSeconds(.25f);
        }
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

    //public async void SpawnBombs()
    //{
    //    while (spawnBombs && bombsCount < bombsCountMax)
    //    {
    //        await Task.Run(() => matchProps.startMatchBool);

    //        spawnBombs = false;

    //        // Create an array to store the valid positions
    //        Vector3[] spawnPositions = new Vector3[10];
    //        int validPositionsCount = 0;


    //        // Generate multiple random positions within the spawn radius
    //        for (int i = 0; i < spawnPositions.Length; i++)
    //        {
    //            Vector3 randomPosition = Random.insideUnitSphere * spawnRadius;
    //            randomPosition += transform.position;

    //            // Find the nearest point on the NavMesh to the random position
    //            NavMeshHit hit;
    //            if (NavMesh.SamplePosition(randomPosition, out hit, spawnRadius, NavMesh.AllAreas))
    //            {
    //                // Add the valid position to the array
    //                spawnPositions[validPositionsCount] = hit.position;
    //                validPositionsCount++;
    //            }
    //        }

    //        // If there are valid positions, choose one randomly for spawning the enemy
    //        if (validPositionsCount > 0)
    //        {
    //            Vector3 spawnPosition = spawnPositions[Random.Range(0, validPositionsCount)];

    //            GameObject bombObject = bombs[Random.Range(0, bombs.Length)];
    //            PhotonNetwork.InstantiateRoomObject(bombObject.name, spawnPosition, Quaternion.identity, 0, null);

    //            bombsCount++;

    //            await WaitSecondsConverter(10);

    //            spawnBombs = true;

    //            await WaitSecondsConverter(1);
    //        }

    //    }
    //}

    //private async void SpawnArtifacts()
    //{
    //    while (spawnArtifacts && artifactCount < artifactCountMax)
    //    {
    //        await Task.Run(() => matchProps.startMatchBool);

    //        spawnArtifacts = false;

    //        // Create an array to store the valid positions
    //        Vector3[] spawnPositions = new Vector3[10];
    //        int validPositionsCount = 0;


    //        // Generate multiple random positions within the spawn radius
    //        for (int i = 0; i < spawnPositions.Length; i++)
    //        {
    //            Vector3 randomPosition = Random.insideUnitSphere * spawnRadius;
    //            randomPosition += transform.position;

    //            // Find the nearest point on the NavMesh to the random position
    //            NavMeshHit hit;
    //            if (NavMesh.SamplePosition(randomPosition, out hit, spawnRadius, NavMesh.AllAreas))
    //            {
    //                // Add the valid position to the array
    //                spawnPositions[validPositionsCount] = hit.position;
    //                validPositionsCount++;
    //            }
    //        }

    //        // If there are valid positions, choose one randomly for spawning the enemy
    //        if (validPositionsCount > 0)
    //        {
    //            Vector3 spawnPosition = spawnPositions[Random.Range(0, validPositionsCount)];

    //            GameObject artifactObject = artifacts[Random.Range(0, artifacts.Length)];
    //            PhotonNetwork.InstantiateRoomObject(artifactObject.name, spawnPosition, Quaternion.identity, 0, null);

    //            artifactCount++;

    //            await WaitSecondsConverter(15);

    //            spawnArtifacts = true;

    //            await WaitSecondsConverter(1);
    //        }
    //    }
    //}

    IEnumerator SpawnHealth()
    {
        while (spawnHealth && healthCount < healthCountMax)
        {
            while (!matchProps.startMatchBool)
                yield return null;

            if (spawnPositions.Count == 0)
            {
                yield return new WaitForSeconds(1);
                continue;
            }

            spawnHealth = false;

            Vector3 spawnPosition = spawnPositions[UnityEngine.Random.Range(0, spawnPositions.Count)];

            Instantiate(health, spawnPosition, Quaternion.identity);

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
                if (spawnPositions.Count == 0)
                {
                    yield return new WaitForSeconds(1);
                    continue;
                }

                Vector3 spawnPosition = spawnPositions[UnityEngine.Random.Range(0, spawnPositions.Count)];

                GameObject enemyCharacterBoss = bosses[0];
                Instantiate(enemyCharacterBoss, spawnPosition, Quaternion.identity);

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

    public void RPC_UpdateBombs()
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