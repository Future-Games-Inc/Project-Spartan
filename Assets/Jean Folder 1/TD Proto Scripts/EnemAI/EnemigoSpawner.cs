using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemigoSpawner : MonoBehaviour
{
    // Chosen state spawn method
    public enum SpawnMethod { RoundRobin, Random, SetSpawnLocation, WeightedRandom, SetSpawnLocationWeightedRandom }
    public SpawnMethod EnemySpawnMethod = SpawnMethod.RoundRobin;

    public Transform player; // assigning this to the enemy that is spawned. 
    [Tooltip("How many enemies do you wish to spawn. Note that this will be affected by the scaling scriptable object")]
    public int NumberOfEnemiesToSpawn = 4;
    public float SpawnDelay = 1f;
    public List<WeightedSpawnScriptableObject> weightedEnemies = new List<WeightedSpawnScriptableObject>();
    [Space]
    [Tooltip("If true, it will spawn Enemies Continuosly, wave after wave with out the need to clear the wave. If False" +
        "you will need to clear the wave first before any more enemies spawn")]
    public bool continuousSpawning;
    [Space]
    public ScalingScriptableObject scaling;
    [Header("Set Spawn location and GameObject Tag")] [Tooltip("Enemies will spawn at random at said location")]
    public GameObject[] spawnlocations;
    public string spawnPointTagName = "spawnpoint";
    [Space]

    private Dictionary<int, ObjectPool> EnemyObjectPools = new Dictionary<int, ObjectPool>();


    private NavMeshTriangulation triangulation;
    [Space]
    [Header("Current Level to Max level")] [Tooltip("Do not change or input value into the level")]
    [SerializeField] private int level = 0;
    [Tooltip("Do not change or input a value into the Max level, this item is reflected" +
        " on the quantity of keys present in the spawn count curve of the scaling scritpable object")]
    [SerializeField] private int MaxLevel;
    [Space] [Header(" Game Play View of Scaled & Weighted Enemy")]
    [Tooltip("Do Not Add or Change it will auto populate when running")]
    [SerializeField] private List<EnemyScriptableObject> scaledEnemies = new List<EnemyScriptableObject>();
    [Tooltip("Do Not Add or Change it will auto populate when running")]
    [SerializeField] private float[] weights;

    [SerializeField] private int enemiesAlive = 0;

    [Space] [Header("Wave Settings")]
    private int spawnedEnemies = 0;
    private int intialEnemiesToSpawn;
    private float initialSpawnDelay;


    private float searchCountDown = 1f;
    public enum SpawnState { Counting, Spawning, Waiting, }
    public enum PlayState { Play, Paused, Finished }
    [Header("Wave Spawner, wave configuration")]
    public float timeBetweenWaves = 5f;
    public float waveCountDown;
    [SerializeField] private SpawnState spawn_State = SpawnState.Counting;
    [SerializeField] private PlayState play_State = PlayState.Play;

    [Space][Header("Connected Scripts")]
    public Wrist_UI_Manager HandUIState;

    private void Awake()
    {
        MaxLevel = scaling.spawnCountCurve.length;
        //spawnlocations = GameObject.FindGameObjectsWithTag(spawnPointTagName);
        GameObject _player = GameObject.FindGameObjectWithTag("Player");
        player = _player.GetComponent<Transform>();

        for (int i = 0; i < weightedEnemies.Count; i++)
        {
            EnemyObjectPools.Add(i, ObjectPool.CreateInstance(weightedEnemies[i].enemy.Prefab, NumberOfEnemiesToSpawn));
        }


        weights = new float[weightedEnemies.Count];

        intialEnemiesToSpawn = NumberOfEnemiesToSpawn;
        initialSpawnDelay = SpawnDelay;

    }

   


    private void Start()
    {
        //Connecting to the UI Hand Manager Script 

        triangulation = NavMesh.CalculateTriangulation();
        spawnlocations = GameObject.FindGameObjectsWithTag(spawnPointTagName);

        for (int i = 0; i < weightedEnemies.Count; i++)
        {


            scaledEnemies.Add(weightedEnemies[i].enemy.ScaleUpForLevel(scaling, 0));

        }

        //Calling to start spawning enemies. 
        //StartCoroutine(SpawnEnemies());


    }

    private void Update()
    {
        if(play_State == PlayState.Play)
        {


            if(spawn_State == SpawnState.Waiting)
            {
                // and if enemy alive is false
                if(!EnemyIsAlive())
                {
                    // run the method Wave completed. 
                    WaveCompleted();
                
                }
                else
                {
                    //return
                    return;
                }
           

            }
            //if wavecountdown is less than or equal to zero. 
            if(waveCountDown <= 0)
            {
                // and if the sate is not equal to spawning
                if(spawn_State != SpawnState.Spawning)
                {
                    //scale up the spawns and start the coroutine spawn enemies method. 
                    ScaledUpSpawns();
                    StartCoroutine(SpawnEnemies());
                }
            }
            // if not continue counting down the waveCountDown Timer
            else
            {
                    waveCountDown -= Time.deltaTime;
            }

        }
        else
        {
            return;
        }
        
    }

    public void Pause_Play_State()
    {
        play_State = PlayState.Paused;

    }

    public void Activate_Play_State()
    {
        play_State = PlayState.Play;

    }
    

  
    public bool EnemyIsAlive()
    {
        searchCountDown -= Time.deltaTime;
        if(searchCountDown <= 0f)
        {
            searchCountDown = 2f;
            //if we try and find a game object with tag enemy and nothing comes up
            // GameObject.FindGameObjectWithTag("Enemy") == null ||
            if (GameObject.FindGameObjectWithTag("Enemy") == null && enemiesAlive <= 0 && spawnedEnemies == NumberOfEnemiesToSpawn)
            {
                // EnemyIs Alive will be set as false
                return false;
            }
        }

        //else EnemyIsAlive will be set as true
        return true;

    }

    void WaveCompleted()
    {
        Debug.Log("Wave Completed");
        spawn_State = SpawnState.Counting;
        waveCountDown = timeBetweenWaves;

        if (level == MaxLevel )
        {
            Debug.Log("All Waves are complete, Play state is set to Finished");
            play_State = PlayState.Finished;
            Debug.Log(PlayState.Finished);
            // TODO: Connect to complete state
            HandUIState.SetHandUIStateTo_GameWonState();
        }
        else
        {
            level++;
            //Connecting the levels passed to the static round variable that is connected to the HandUI;
            PlayerStats.Rounds = level;
        }


    }



    private IEnumerator SpawnEnemies()
    {
        spawn_State = SpawnState.Spawning;
        //level++;
        spawnedEnemies = 0;
        enemiesAlive = 0;

        for(int i = 0; i < weightedEnemies.Count; i++)
        {
            scaledEnemies[i] = weightedEnemies[i].enemy.ScaleUpForLevel(scaling, level);

        }

        ResetSpawnWheights();

        WaitForSeconds wait = new WaitForSeconds(SpawnDelay);


        while (spawnedEnemies < NumberOfEnemiesToSpawn)
        {
            if(EnemySpawnMethod == SpawnMethod.RoundRobin)
            {
                SpawnRoundRobinEnemy(spawnedEnemies);
            }
            else if(EnemySpawnMethod == SpawnMethod.Random)
            {
                SpawnRandomEnemy();
            }
            else if(EnemySpawnMethod == SpawnMethod.SetSpawnLocation)
            {
                int SpawnIndex = spawnedEnemies % weightedEnemies.Count;
                SpawnEnemyAtSetLocation(SpawnIndex);
            }
            else if(EnemySpawnMethod == SpawnMethod.WeightedRandom)
            {
                SpawnWeightedRandomEnemy();

            }
            else if (EnemySpawnMethod == SpawnMethod.SetSpawnLocationWeightedRandom)
            {
                SpawnWeightedEnemy_AtSetLocation();
            }

            spawnedEnemies++;

            spawn_State = SpawnState.Waiting;
            yield return wait;
        }

        if(continuousSpawning)
        {
            ScaledUpSpawns();
            StartCoroutine(SpawnEnemies());
        }


    }

    private void SpawnWeightedEnemy_AtSetLocation()
    {
        float value = UnityEngine.Random.value;

        for (int i = 0; i < weights.Length; i++)
        {
            if (value < weights[i])
            {
                //DoSpawnEnemy(i, ChooseRandomPositionOnNavMesh());
                SpawnEnemyAtSetLocation(i);
                return;
            }

            value -= weights[i];

        }
        Debug.Log("Invalid configuratio! could not spawn a weighted random enemy. Did you forget to call resetSpawnWeights()?");

    }

    private void SpawnWeightedRandomEnemy()
    {
        float value = UnityEngine.Random.value;

        for (int i = 0; i < weights.Length; i++)
        {
            if(value < weights[i])
            {
                DoSpawnEnemy(i, ChooseRandomPositionOnNavMesh());
                return;
            }

            value -= weights[i];

        }
        Debug.Log("Invalid configuratio! could not spawn a weighted random enemy. Did you forget to call resetSpawnWeights()?");

    }

    private void ResetSpawnWheights()
    {
        float totalWeight = 0;
        for (int i = 0; i < weightedEnemies.Count; i++)
        {
            weights[i] = weightedEnemies[i].GetWeight();
            totalWeight += weights[i];


        }

        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] = weights[i] / totalWeight;
        }  


    }



    private void SpawnEnemyAtSetLocation(int spawnIndex)
    {

       
        PoolableObject poolableObject = EnemyObjectPools[spawnIndex].GetObject();

        //int spawn = UnityEngine.Random.Range(0, spawnlocations.Length - 1);

        if (poolableObject != null)
        {
            Enemigo enemy = poolableObject.GetComponent<Enemigo>();
            weightedEnemies[spawnIndex].enemy.Setupenemy(enemy);

          

            enemy.agent.Warp(spawnlocations[UnityEngine.Random.Range(0, spawnlocations.Length)].transform.position);
            enemy.movement.player = player;
            enemy.movement.triangulation = triangulation;
            enemy.agent.enabled = true;
            enemy.movement.Spawn();
            enemy.OnDie += HandleEnemyDeath;


            enemiesAlive++;

        }
        else
        {
            Debug.LogError($"unable to fetch enemy of type {spawnIndex} from object pool. ou tof objects;");

        }




    }

    private void SpawnRoundRobinEnemy(int spawnedEnemies)
    {
        int SpawnIndex = spawnedEnemies % weightedEnemies.Count; // if zero spawned enemies and two enemy prefag you get a zero result. 0 % 2 = 0 
        DoSpawnEnemy(SpawnIndex , ChooseRandomPositionOnNavMesh());                                              //
    }

    private void SpawnRandomEnemy( )
    {
        DoSpawnEnemy(UnityEngine.Random.Range(0, weightedEnemies.Count), ChooseRandomPositionOnNavMesh());
    }

    private Vector3 ChooseRandomPositionOnNavMesh()
    {
        int VertexIndex = UnityEngine.Random.Range(0, triangulation.vertices.Length);
        return triangulation.vertices[VertexIndex];

    }

    public void DoSpawnEnemy(int spawnIndex, Vector3 spawnPosition)
    {
        PoolableObject poolableObject = EnemyObjectPools[spawnIndex].GetObject();
        

        if(poolableObject != null)
        {
            Enemigo enemy = poolableObject.GetComponent<Enemigo>();
            scaledEnemies[spawnIndex].Setupenemy(enemy);



            NavMeshHit Hit;

            if(NavMesh.SamplePosition(spawnPosition, out Hit, 2f, -1))
            {
                enemy.agent.Warp(Hit.position);
                //Enemy needs to get enabled and start chasing now. 
                enemy.movement.player = player;
                enemy.movement.triangulation = triangulation;
                enemy.agent.enabled = true;
                enemy.movement.Spawn();
                enemy.OnDie += HandleEnemyDeath;

                enemiesAlive++;

            }
            else
            {
                Debug.LogError($"Unable to place NavMeshAgeent on Navement. Tried to use {spawnPosition} ");
            }
        }
        else
        {
            Debug.LogError($"unable to fetch enemy of type {spawnIndex} from object pool. ou tof objects;" );

        }


    }


    private void ScaledUpSpawns()
    {
        NumberOfEnemiesToSpawn = Mathf.FloorToInt(intialEnemiesToSpawn * scaling.spawnCountCurve.Evaluate(level + 1));
        SpawnDelay = initialSpawnDelay * scaling.SpawnRateCurve.Evaluate(level + 1);

    }
   


    private void HandleEnemyDeath(Enemigo enemy)
    {
        

        enemiesAlive--;
        //Debug.Log("HandleEnemyDeath is being passed trhough");
        //if(enemiesAlive == 0 && spawnedEnemies == NumberOfEnemiesToSpawn)
        //{
        //    if(!EnemyIsAlive())
        //    {
        //        ScaledUpSpawns();
        //        StartCoroutine(SpawnEnemies());

        //    }
        //    else
        //    {
        //        return;
        //    }
        //}
      

    }


    



}
