using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Collider))]
public class EnemyBurstSpawnArea : MonoBehaviour
{

    [SerializeField][Tooltip("Leave Empty if you want the enemies to spawn inside this objects Collider," +
        " else add a different game object with collider that you wish to spawn enemies within")]
    private Collider spawnCollider;
    [SerializeField][Tooltip("The enemy that you attach here will need to have the same enemy in the enemy spawner you wish to be spawned within it")]
    private EnemigoSpawner enemySpawner;
    [SerializeField]
    private List<EnemyScriptableObject> Enemies = new List<EnemyScriptableObject>();
    [SerializeField]
    private EnemigoSpawner.SpawnMethod SpawnMethod = EnemigoSpawner.SpawnMethod.Random;
    [SerializeField]
    private int spawnCount = 10;
    [SerializeField]
    private float spawnDelay = 0.5f;




    private Coroutine spawnEnemiesCoroutine;
    private Bounds bounds;

    private void Awake()
    {

        if(spawnCollider == null)
        {
             spawnCollider = GetComponent<Collider>();

        }


        bounds = spawnCollider.bounds;

    }


    private void OnTriggerEnter(Collider other)
    {
        if(spawnEnemiesCoroutine == null)
        {
            spawnEnemiesCoroutine = StartCoroutine(SpawnEnemies());

        }
    }

    private Vector3 GetRandomPositionInBounds()
    {

      return  new Vector3(UnityEngine.Random.Range(bounds.min.x, bounds.max.x), bounds.min.y, UnityEngine.Random.Range(bounds.min.z, bounds.max.z));

    }


    private IEnumerator SpawnEnemies()
    {
        WaitForSeconds wait = new WaitForSeconds(spawnDelay);

        for(int i=0; i < spawnCount; i ++)
        {
            if(SpawnMethod == EnemigoSpawner.SpawnMethod.RoundRobin)
            {
                enemySpawner.DoSpawnEnemy(enemySpawner.weightedEnemies.FindIndex((enemy) => enemy.enemy.Equals(Enemies[i % Enemies.Count])), GetRandomPositionInBounds() );
            }
            else if( SpawnMethod == EnemigoSpawner.SpawnMethod.Random)
            {
                int index = UnityEngine.Random.Range(0, Enemies.Count);
                enemySpawner.DoSpawnEnemy(enemySpawner.weightedEnemies.FindIndex((enemy) => enemy.enemy.Equals(Enemies[index])), GetRandomPositionInBounds() );

            }

            else if(SpawnMethod == EnemigoSpawner.SpawnMethod.SetSpawnLocation)
            {
                //todo: put in the set position to spawn
            }

            yield return wait;

        }

        Destroy(gameObject);


    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
