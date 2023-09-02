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
    public int enemiesKilled;
    private int artifactCount;
    private int bombsCount;

    private bool spawnEnemy = true;
    private bool spawnSecurity = true;
    private bool spawnHealth = true;
    private bool spawnBombs = false;
    private bool spawnArtifacts = true;
    private bool coroutinesStarted = false;


    public float spawnRadius = 300.0f;   // Maximum spawn radius

    public NavMeshSurface navMeshSurface;
    public Vector3[] spawnPositions;
    public int validPositionsCount;

    public override void OnEnable()
    {
        if (!coroutinesStarted)
        {
            StartCoroutine(SpawnEnemies());
            StartCoroutine(SpawnSecurity());
            StartCoroutine(SpawnReactor());
            StartCoroutine(SpawnHealth());
            StartCoroutine(SpawnBoss());

            coroutinesStarted = true;
        }
        base.OnEnable();
        // Listen to Photon Events
        PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;


        // Create an array to store the valid positions
        spawnPositions = new Vector3[10];
        validPositionsCount = 0;


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

    public override void OnDisable()
    {
        StopAllCoroutines();
        coroutinesStarted = false;
        base.OnDisable();
        // Stop listening to Photon Events
        PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;

    }

    IEnumerator SpawnEnemies()
    {
        while (spawnEnemy && enemyCount < enemyCountMax)
        {
            while (!matchProps.startMatchBool)
                yield return null;

            ShuffleArray(enemyAI);
            ShuffleSpawns(spawnPositions);

            spawnEnemy = false;

            Vector3 spawnPosition = spawnPositions[0];

            GameObject enemyCharacter = enemyAI[0];
            PhotonNetwork.InstantiateRoomObject(enemyCharacter.name, spawnPosition, Quaternion.identity, 0, null);

            enemyCount++;

            yield return new WaitForSeconds(2); // Replaces await WaitSecondsConverter(10);
            spawnEnemy = true;

            yield return new WaitForSeconds(1); // Replaces await WaitSecondsConverter(1);
        }
    }


    IEnumerator SpawnSecurity()
    {
        while (spawnSecurity && securityCount < securityCountMax)
        {
            while (!matchProps.startMatchBool)
                yield return null;

            yield return new WaitForSeconds(1);

            ShuffleArray(securityAI);
            ShuffleSpawns(spawnPositions);

            spawnSecurity = false;

            Vector3 spawnPosition = spawnPositions[0];

            GameObject securityDrone = securityAI[0];
            PhotonNetwork.InstantiateRoomObject(securityDrone.name, spawnPosition, Quaternion.identity, 0, null);

            securityCount++;

            yield return new WaitForSeconds(4);
            spawnSecurity = true;

            yield return new WaitForSeconds(1);
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
                ShuffleSpawns(spawnPositions);
                Vector3 spawnPosition = spawnPositions[0];

                PhotonNetwork.InstantiateRoomObject(reactor.name, spawnPosition, Quaternion.identity, 0, null);

                reactorCount++;

                yield return new WaitForSeconds(25);
            }
            yield return new WaitForSeconds(1);
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

            ShuffleSpawns(spawnPositions);

            spawnHealth = false;

            Vector3 spawnPosition = spawnPositions[0];

            GameObject Health = PhotonNetwork.InstantiateRoomObject(health.name, spawnPosition, Quaternion.identity, 0, null);

            healthCount++;

            yield return new WaitForSeconds(25);
            spawnHealth = true;

            yield return new WaitForSeconds(1);
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
                ShuffleArray(enemyBoss);
                ShuffleSpawns(spawnPositions);

                Vector3 spawnPosition = spawnPositions[0];

                GameObject enemyCharacterBoss = enemyBoss[0];
                PhotonNetwork.InstantiateRoomObject(enemyCharacterBoss.name, spawnPosition, Quaternion.identity, 0, null);

                enemiesKilled = 0;
                yield return new WaitForSeconds(30);
            }

            yield return new WaitForSeconds(1);
        }
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