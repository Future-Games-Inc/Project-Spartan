using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Threading.Tasks;

public class SpawnManager1 : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject[] enemyAI;
    [SerializeField] private GameObject[] enemyBoss;
    [SerializeField] private GameObject[] securityAI;
    [SerializeField] private GameObject reactor;
    [SerializeField] private GameObject health;
    [SerializeField] private GameObject[] artifacts;
    [SerializeField] private GameObject[] bombs;

    [SerializeField] private Transform[] enemyDrop;

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
    private bool spawnReactor = true;
    private bool spawnHealth = true;
    private bool spawnBoss = true;
    private bool spawnBombs = true;
    private bool spawnArtifacts = true;

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

            GameObject enemyCharacter = enemyAI[Random.Range(0, enemyAI.Length)];
            PhotonNetwork.InstantiateRoomObject(enemyCharacter.name, enemyDrop[Random.Range(0, enemyDrop.Length)].position, Quaternion.identity, 0, null);

            enemyCount++;

            await WaitSecondsConverter(10);

            spawnEnemy = true;

            await WaitSecondsConverter(1);
        }
    }

    public async void SpawnSecurity()
    {
        while (spawnSecurity && securityCount < securityCountMax)
        {
            await Task.Run(() => matchProps.startMatchBool);

            spawnSecurity = false;

            GameObject securityDrone = securityAI[Random.Range(0, securityAI.Length)];
            PhotonNetwork.InstantiateRoomObject(securityDrone.name, enemyDrop[Random.Range(0, enemyDrop.Length)].position, Quaternion.identity, 0, null);

            securityCount++;

            await WaitSecondsConverter(15);

            spawnSecurity = true;

            await WaitSecondsConverter(1);
        }
    }

    public async void SpawnReactor()
    {
        while (spawnReactor && reactorCount < reactorCountMax)
        {
            await Task.Run(() => matchProps.startMatchBool && matchProps.spawnReactor);

            spawnReactor = false;

            Vector3 spawnPosition = enemyDrop[Random.Range(0, enemyDrop.Length)].position;
            spawnPosition += new Vector3(0f, 2f, 0f); // Add 2 to the y transform

            PhotonNetwork.InstantiateRoomObject(reactor.name, spawnPosition, Quaternion.identity, 0, null);

            reactorCount++;

            await WaitSecondsConverter(30);

            spawnReactor = true;

            await WaitSecondsConverter(1);
        }
    }

    public async void SpawnBombs()
    {
        while (spawnBombs && bombsCount < bombsCountMax)
        {
            await Task.Run(() => matchProps.startMatchBool);

            spawnBombs = false;

            GameObject bombObject = bombs[Random.Range(0, bombs.Length)];
            PhotonNetwork.InstantiateRoomObject(bombObject.name, enemyDrop[Random.Range(0, enemyDrop.Length)].position, Quaternion.identity, 0, null);

            bombsCount++;

            await WaitSecondsConverter(10);

            spawnBombs = true;

            await WaitSecondsConverter(1);

        }
    }


    private async void SpawnArtifacts()
    {
        while (spawnArtifacts && artifactCount < artifactCountMax)
        {
            await Task.Run(() => matchProps.startMatchBool);

            spawnArtifacts = false;

            GameObject artifactObject = artifacts[Random.Range(0, artifacts.Length)];
            PhotonNetwork.InstantiateRoomObject(artifactObject.name, enemyDrop[Random.Range(0, enemyDrop.Length)].position, Quaternion.identity, 0, null);

            artifactCount++;

            await WaitSecondsConverter(15);

            spawnArtifacts = true;

            await WaitSecondsConverter(1);
        }
    }

 

    public async void SpawnHealth()
    {
        while (spawnHealth && healthCount < healthCountMax)
        {
            await Task.Run(() => matchProps.startMatchBool);

            spawnHealth = false;

            GameObject Health = PhotonNetwork.InstantiateRoomObject(health.name, enemyDrop[Random.Range(0, enemyDrop.Length)].position, Quaternion.identity, 0, null);

            healthCount++;

            await WaitSecondsConverter(35);

            spawnHealth = true;

            await WaitSecondsConverter(1);
        }
    }

    public async void SpawnBoss()
    {
        while (spawnBoss)
        {
            await Task.Run(() => matchProps.startMatchBool);

            if (enemiesKilled >= enemiesKilledForBossSpawn)
            {
                spawnBoss = false;

                GameObject enemyCharacterBoss = enemyBoss[Random.Range(0, enemyBoss.Length)];
                PhotonNetwork.InstantiateRoomObject(enemyCharacterBoss.name, enemyDrop[Random.Range(0, enemyDrop.Length)].position, Quaternion.identity, 0, null);

                enemiesKilled = 0;

                await WaitSecondsConverter(45);

                spawnBoss = true;
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

    //[PunRPC]
    //public void RPC_UpdateEnemy()
    //{
    //    enemyCount--;
    //}

    //[PunRPC]
    //public void RPC_UpdateEnemyCount()
    //{
    //    enemiesKilled++;
    //}

   

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