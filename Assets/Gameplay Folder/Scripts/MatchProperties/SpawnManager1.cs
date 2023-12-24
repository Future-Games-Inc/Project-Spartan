using System.Collections;
using Umbrace.Unity.PurePool;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

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
    [SerializeField] private GameObject[] defenderEnemies;
    public Transform reactorSpawnLocation;
    public GameObject reactorPlaceholder;


    [SerializeField] private MatchEffects matchProps;

    [SerializeField] public int enemyCountMax = 5;
    [SerializeField] public int defenderCountMax = 5;
    [SerializeField] public int securityCountMax = 2;
    [SerializeField] private int reactorCountMax = 1;
    [SerializeField] private int healthCountMax = 1;
    [SerializeField] public int enemiesKilledForBossSpawn = 5;
    [SerializeField] private int bombsCountMax = 10;
    [SerializeField] private int artifactCountMax = 10;

    private int enemyCount;
    public int defenderCount;
    private int securityCount;
    private int reactorCount;
    private int healthCount;
    public int enemiesKilled;
    private int artifactCount;
    private int bombsCount;

    private bool spawnEnemy = true;
    private bool spawnSecurity = true;
    private bool spawnHealth = true;
    private bool spawnBombs = true;
    private bool spawnArtifacts = true;
    private bool coroutinesStarted = false;
    public bool spawnDefenders = false;


    public float spawnRadius;   // Maximum spawn radius

    public NavMeshSurface navMeshSurface;
    public Transform[] spawnPositions;  // Dynamic list for valid positions
    public float bufferRadius = .15f;
    public bool spawnReinforcements = false;
    public int reinforcementCount;
    public int reinforcementCountMax;
    public GameObject reinforcementTroop;
    public GameObjectPoolManager PoolManager;

    private void Awake()
    {
        // Find the manager if one hasn't been specified.
        if (this.PoolManager == null)
        {
            this.PoolManager = Object.FindObjectOfType<GameObjectPoolManager>();
        }
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name != "WeaponTest")
        {
            if (PlayerPrefs.HasKey("BombQuest") && PlayerPrefs.GetInt("BombQuest") == 1)
            {
                spawnBombs = true;
                StartCoroutine(SpawnBombs());
            }
            else
                spawnBombs = false;

            if (PlayerPrefs.HasKey("ArtifactQuest") && PlayerPrefs.GetInt("ArtifactQuest") == 1)
            {
                spawnArtifacts = true;
                StartCoroutine(SpawnArtifacts());
            }
            else
                spawnArtifacts = false;


            spawnRadius = 5000f;


            if (!coroutinesStarted)
            {
                StartCoroutine(SpawnEnemies());
                StartCoroutine(SpawnSecurity());
                StartCoroutine(SpawnReactor());
                StartCoroutine(SpawnHealth());
                StartCoroutine(SpawnBoss());
                StartCoroutine(SpawnReinforcements());
                StartCoroutine(SpawnDefenders());

                coroutinesStarted = true;
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
            this.PoolManager.Acquire(enemyCharacter, spawnPosition[0].position, Quaternion.identity);

            enemyCount++;

            yield return new WaitForSeconds(2);
            spawnEnemy = true;

            yield return new WaitForSeconds(.25f);
        }
    }

    public IEnumerator SpawnReinforcements()
    {
        while (true)
        {
            while (!matchProps.startMatchBool)
                yield return null;
            if (spawnReinforcements && reinforcementCount < reinforcementCountMax)
            {
                spawnReinforcements = false;

                Transform[] spawnPosition = ShuffleSpawns(spawnPositions);

                this.PoolManager.Acquire(reinforcementTroop, spawnPosition[0].position, Quaternion.identity);

                reinforcementCount++;

                yield return new WaitForSeconds(2);
                spawnReinforcements = true;

                yield return new WaitForSeconds(.25f);
            }
            yield return new WaitForSeconds(.25f);
        }
    }

    public IEnumerator SpawnDefenders()
    {
        while (true)
        {
            while (!matchProps.startMatchBool)
                yield return null;
            if (spawnDefenders && defenderCount < defenderCountMax)
            {
                Transform[] spawnPosition = ShuffleSpawns(spawnPositions);

                spawnDefenders = false;

                GameObject[] enemies = ShuffleArray(defenderEnemies);

                GameObject defenderDrone = enemies[0];
                this.PoolManager.Acquire(defenderDrone, spawnPosition[0].position, Quaternion.identity);

                defenderCount++;

                yield return new WaitForSeconds(2);
                spawnDefenders = true;

                yield return new WaitForSeconds(.25f);
            }
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
            this.PoolManager.Acquire(securityDrone, spawnPosition[0].position, Quaternion.identity);

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

        this.PoolManager.Acquire(supplyDrop, newPosition, Quaternion.identity);
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

                this.PoolManager.Acquire(reactor, reactorSpawnLocation.position, Quaternion.identity);

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
            this.PoolManager.Acquire(securityDrone, spawnPosition[0].position, Quaternion.identity);

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
            this.PoolManager.Acquire(securityDrone, spawnPosition[0].position, Quaternion.identity);

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

            // Modify y value of the spawn position
            Vector3 newPosition = spawnPosition[0].position + new Vector3(0, 10, 0);

            this.PoolManager.Acquire(health, newPosition, Quaternion.identity);

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
                this.PoolManager.Acquire(enemyCharacterBoss, spawnPosition[0].position, Quaternion.identity);

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

    public void UpdateReinforcements()
    {
        reinforcementCount--;
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