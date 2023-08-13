using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Threading.Tasks;
using UnityEngine.AI;

public class SpawnManager1 : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject[] enemyAI;
    [SerializeField] private GameObject[] enemyBoss;
    [SerializeField] private GameObject[] securityAI;
    [SerializeField] private GameObject reactor;
    [SerializeField] private GameObject health;
    [SerializeField] private GameObject[] artifacts;
    [SerializeField] private GameObject[] bombs;

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
    private int enemiesKilled;
    private int artifactCount;
    private int bombsCount;

    private bool spawnEnemy = true;
    private bool spawnSecurity = true;
    private bool spawnHealth = true;
    private bool spawnBombs = false;
    private bool spawnArtifacts = true;

    public float spawnRadius = 300.0f;   // Maximum spawn radius

    public NavMeshSurface navMeshSurface;

    private void Start()
    {
        SpawnEnemies();
        SpawnSecurity();
        SpawnReactor();
        SpawnHealth();
        SpawnBoss();
        SpawnArtifacts();
        SpawnBombs();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        // Listen to Photon Events
        PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        // Stop listening to Photon Events
        PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;

    }

    public async void SpawnEnemies()
    {
        while (spawnEnemy && enemyCount < enemyCountMax)
        {
            await Task.Run(() => matchProps.startMatchBool);

            spawnEnemy = false;

            // Create an array to store the valid positions
            Vector3[] spawnPositions = new Vector3[10];
            int validPositionsCount = 0;


            // Generate multiple random positions within the spawn radius
            for (int i = 0; i < spawnPositions.Length; i++)
            {
                Vector3 randomPosition = Random.insideUnitSphere * spawnRadius;
                randomPosition += transform.position;

                // Find the nearest point on the NavMesh to the random position
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPosition, out hit, spawnRadius, NavMesh.AllAreas))
                {
                    // Add the valid position to the array
                    spawnPositions[validPositionsCount] = hit.position;
                    validPositionsCount++;
                }
            }

            // If there are valid positions, choose one randomly for spawning the enemy
            if (validPositionsCount > 0)
            {
                Vector3 spawnPosition = spawnPositions[Random.Range(0, validPositionsCount)];

                GameObject enemyCharacter = enemyAI[Random.Range(0, enemyAI.Length)];
                PhotonNetwork.InstantiateRoomObject(enemyCharacter.name, spawnPosition, Quaternion.identity, 0, null);

                enemyCount++;

                await WaitSecondsConverter(10);

                spawnEnemy = true;

                await WaitSecondsConverter(1);
            }
        }
    }

    public async void SpawnSecurity()
    {
        while (spawnSecurity && securityCount < securityCountMax)
        {
            await Task.Run(() => matchProps.startMatchBool);

            spawnSecurity = false;

            // Create an array to store the valid positions
            Vector3[] spawnPositions = new Vector3[10];
            int validPositionsCount = 0;


            // Generate multiple random positions within the spawn radius
            for (int i = 0; i < spawnPositions.Length; i++)
            {
                Vector3 randomPosition = Random.insideUnitSphere * spawnRadius;
                randomPosition += transform.position;

                // Find the nearest point on the NavMesh to the random position
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPosition, out hit, spawnRadius, NavMesh.AllAreas))
                {
                    // Add the valid position to the array
                    spawnPositions[validPositionsCount] = hit.position;
                    validPositionsCount++;
                }
            }

            // If there are valid positions, choose one randomly for spawning the enemy
            if (validPositionsCount > 0)
            {
                Vector3 spawnPosition = spawnPositions[Random.Range(0, validPositionsCount)];

                GameObject securityDrone = securityAI[Random.Range(0, securityAI.Length)];
                PhotonNetwork.InstantiateRoomObject(securityDrone.name, spawnPosition, Quaternion.identity, 0, null);

                securityCount++;

                await WaitSecondsConverter(15);

                spawnSecurity = true;

                await WaitSecondsConverter(1);
            }
        }
    }

    public async void SpawnReactor()
    {
        while (true)
        {
            await Task.Run(() => matchProps.startMatchBool && matchProps.spawnReactor);
            if (matchProps.spawnReactor && reactorCount < reactorCountMax)
            {
                // Create an array to store the valid positions
                Vector3[] spawnPositions = new Vector3[10];
                int validPositionsCount = 0;


                // Generate multiple random positions within the spawn radius
                for (int i = 0; i < spawnPositions.Length; i++)
                {
                    Vector3 randomPosition = Random.insideUnitSphere * spawnRadius;
                    randomPosition += transform.position;

                    // Find the nearest point on the NavMesh to the random position
                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(randomPosition, out hit, spawnRadius, NavMesh.AllAreas))
                    {
                        // Add the valid position to the array
                        spawnPositions[validPositionsCount] = hit.position;
                        validPositionsCount++;
                    }
                }

                // If there are valid positions, choose one randomly for spawning the enemy
                if (validPositionsCount > 0)
                {
                    Vector3 spawnPosition = spawnPositions[Random.Range(0, validPositionsCount)];

                    PhotonNetwork.InstantiateRoomObject(reactor.name, spawnPosition, Quaternion.identity, 0, null);

                    reactorCount++;

                    await WaitSecondsConverter(30);
                }
            }
            await WaitSecondsConverter(1);
        }
    }

    public async void SpawnBombs()
    {
        while (spawnBombs && bombsCount < bombsCountMax)
        {
            await Task.Run(() => matchProps.startMatchBool);

            spawnBombs = false;

            // Create an array to store the valid positions
            Vector3[] spawnPositions = new Vector3[10];
            int validPositionsCount = 0;


            // Generate multiple random positions within the spawn radius
            for (int i = 0; i < spawnPositions.Length; i++)
            {
                Vector3 randomPosition = Random.insideUnitSphere * spawnRadius;
                randomPosition += transform.position;

                // Find the nearest point on the NavMesh to the random position
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPosition, out hit, spawnRadius, NavMesh.AllAreas))
                {
                    // Add the valid position to the array
                    spawnPositions[validPositionsCount] = hit.position;
                    validPositionsCount++;
                }
            }

            // If there are valid positions, choose one randomly for spawning the enemy
            if (validPositionsCount > 0)
            {
                Vector3 spawnPosition = spawnPositions[Random.Range(0, validPositionsCount)];

                GameObject bombObject = bombs[Random.Range(0, bombs.Length)];
                PhotonNetwork.InstantiateRoomObject(bombObject.name, spawnPosition, Quaternion.identity, 0, null);

                bombsCount++;

                await WaitSecondsConverter(10);

                spawnBombs = true;

                await WaitSecondsConverter(1);
            }

        }
    }

    private async void SpawnArtifacts()
    {
        while (spawnArtifacts && artifactCount < artifactCountMax)
        {
            await Task.Run(() => matchProps.startMatchBool);

            spawnArtifacts = false;

            // Create an array to store the valid positions
            Vector3[] spawnPositions = new Vector3[10];
            int validPositionsCount = 0;


            // Generate multiple random positions within the spawn radius
            for (int i = 0; i < spawnPositions.Length; i++)
            {
                Vector3 randomPosition = Random.insideUnitSphere * spawnRadius;
                randomPosition += transform.position;

                // Find the nearest point on the NavMesh to the random position
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPosition, out hit, spawnRadius, NavMesh.AllAreas))
                {
                    // Add the valid position to the array
                    spawnPositions[validPositionsCount] = hit.position;
                    validPositionsCount++;
                }
            }

            // If there are valid positions, choose one randomly for spawning the enemy
            if (validPositionsCount > 0)
            {
                Vector3 spawnPosition = spawnPositions[Random.Range(0, validPositionsCount)];

                GameObject artifactObject = artifacts[Random.Range(0, artifacts.Length)];
                PhotonNetwork.InstantiateRoomObject(artifactObject.name, spawnPosition, Quaternion.identity, 0, null);

                artifactCount++;

                await WaitSecondsConverter(15);

                spawnArtifacts = true;

                await WaitSecondsConverter(1);
            }
        }
    }

    public async void SpawnHealth()
    {
        while (spawnHealth && healthCount < healthCountMax)
        {
            await Task.Run(() => matchProps.startMatchBool);

            spawnHealth = false;

            // Create an array to store the valid positions
            Vector3[] spawnPositions = new Vector3[10];
            int validPositionsCount = 0;


            // Generate multiple random positions within the spawn radius
            for (int i = 0; i < spawnPositions.Length; i++)
            {
                Vector3 randomPosition = Random.insideUnitSphere * spawnRadius;
                randomPosition += transform.position;

                // Find the nearest point on the NavMesh to the random position
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPosition, out hit, spawnRadius, NavMesh.AllAreas))
                {
                    // Add the valid position to the array
                    spawnPositions[validPositionsCount] = hit.position;
                    validPositionsCount++;
                }
            }

            // If there are valid positions, choose one randomly for spawning the enemy
            if (validPositionsCount > 0)
            {
                Vector3 spawnPosition = spawnPositions[Random.Range(0, validPositionsCount)];

                GameObject Health = PhotonNetwork.InstantiateRoomObject(health.name, spawnPosition, Quaternion.identity, 0, null);

                healthCount++;

                await WaitSecondsConverter(35);

                spawnHealth = true;

                await WaitSecondsConverter(1);
            }
        }
    }

    public async void SpawnBoss()
    {
        while (true)
        {
            await Task.Run(() => matchProps.startMatchBool);

            if (enemiesKilled >= enemiesKilledForBossSpawn)
            {
                // Create an array to store the valid positions
                Vector3[] spawnPositions = new Vector3[10];
                int validPositionsCount = 0;


                // Generate multiple random positions within the spawn radius
                for (int i = 0; i < spawnPositions.Length; i++)
                {
                    Vector3 randomPosition = Random.insideUnitSphere * spawnRadius;
                    randomPosition += transform.position;

                    // Find the nearest point on the NavMesh to the random position
                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(randomPosition, out hit, spawnRadius, NavMesh.AllAreas))
                    {
                        // Add the valid position to the array
                        spawnPositions[validPositionsCount] = hit.position;
                        validPositionsCount++;
                    }
                }

                // If there are valid positions, choose one randomly for spawning the enemy
                if (validPositionsCount > 0)
                {
                    Vector3 spawnPosition = spawnPositions[Random.Range(0, validPositionsCount)];

                    GameObject enemyCharacterBoss = enemyBoss[Random.Range(0, enemyBoss.Length)];
                    PhotonNetwork.InstantiateRoomObject(enemyCharacterBoss.name, spawnPosition, Quaternion.identity, 0, null);

                    enemiesKilled = 0;

                    await WaitSecondsConverter(45);
                }
            }

            await WaitSecondsConverter(1);
        }
    }

    private Task WaitSecondsConverter(int seconds)
    {
        return Task.Delay(seconds * 1000);
    }

    private void NetworkingClient_EventReceived(ExitGames.Client.Photon.EventData obj)
    {
        if (obj.Code == (byte)PUNEventDatabase.SpawnManager1_UpdateEnemyCount)
        {
            UpdateEnemy();
            UpdateEnemyCount();
            return;
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

    [PunRPC]
    public void RPC_UpdateArtifact()
    {
        artifactCount--;
    }

    [PunRPC]
    public void RPC_UpdateBombs()
    {
        bombsCount--;
    }

    [PunRPC]
    public void RPC_UpdateSecurity()
    {
        securityCount--;
    }

    [PunRPC]
    public void RPC_UpdateHealthCount()
    {
        healthCount--;
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        // Check if this is the object's current owner and if the new master client exists
        if (photonView.IsMine && newMasterClient != null)
        {
            // Transfer ownership of the object to the new master client
            photonView.TransferOwnership(newMasterClient.ActorNumber);
        }
    }
}