using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

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
        StartCoroutine(SpawnEnemies());
        StartCoroutine(SpawnSecurity());
        StartCoroutine(SpawnReactor());
        StartCoroutine(SpawnHealth());
        StartCoroutine(SpawnBoss());
        StartCoroutine(SpawnArtifacts());
        StartCoroutine(SpawnBombs());
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

    

    private IEnumerator SpawnEnemies()
    {
        while (spawnEnemy && enemyCount < enemyCountMax)
        {
            yield return new WaitUntil(() => matchProps.startMatchBool);

            spawnEnemy = false;

            GameObject enemyCharacter = enemyAI[Random.Range(0, enemyAI.Length)];
            PhotonNetwork.InstantiateRoomObject(enemyCharacter.name, enemyDrop[Random.Range(0, enemyDrop.Length)].position, Quaternion.identity, 0, null);

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

            GameObject securityDrone = securityAI[Random.Range(0, securityAI.Length)];
            PhotonNetwork.InstantiateRoomObject(securityDrone.name, enemyDrop[Random.Range(0, enemyDrop.Length)].position, Quaternion.identity, 0, null);

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

            Vector3 spawnPosition = enemyDrop[Random.Range(0, enemyDrop.Length)].position;
            spawnPosition += new Vector3(0f, 2f, 0f); // Add 2 to the y transform

            PhotonNetwork.InstantiateRoomObject(reactor.name, spawnPosition, Quaternion.identity, 0, null);

            reactorCount++;

            yield return new WaitForSeconds(30f);

            spawnReactor = true;

            yield return new WaitForSeconds(1);
        }
    }

    private IEnumerator SpawnBombs()
    {
        while (spawnBombs && bombsCount < bombsCountMax)
        {
            yield return new WaitUntil(() => matchProps.startMatchBool);

            spawnBombs = false;

            GameObject bombObject = bombs[Random.Range(0, bombs.Length)];
            PhotonNetwork.InstantiateRoomObject(bombObject.name, enemyDrop[Random.Range(0, enemyDrop.Length)].position, Quaternion.identity, 0, null);

            bombsCount++;

            yield return new WaitForSeconds(10f);

            spawnBombs = true;

            yield return new WaitForSeconds(1);
        }
    }

    private IEnumerator SpawnArtifacts()
    {
        while (spawnArtifacts && artifactCount < artifactCountMax)
        {
            yield return new WaitUntil(() => matchProps.startMatchBool);

            spawnArtifacts = false;

            GameObject artifactObject = artifacts[Random.Range(0, artifacts.Length)];
            PhotonNetwork.InstantiateRoomObject(artifactObject.name, enemyDrop[Random.Range(0, enemyDrop.Length)].position, Quaternion.identity, 0, null);

            artifactCount++;

            yield return new WaitForSeconds(15f);

            spawnArtifacts = true;

            yield return new WaitForSeconds(1);
        }
    }

    private IEnumerator SpawnHealth()
    {
        while (spawnHealth && healthCount < healthCountMax)
        {
            yield return new WaitUntil(() => matchProps.startMatchBool);

            spawnHealth = false;

            GameObject Health = PhotonNetwork.InstantiateRoomObject(health.name, enemyDrop[Random.Range(0, enemyDrop.Length)].position, Quaternion.identity, 0, null);
            Health.GetComponent<Rigidbody>().isKinematic = false;

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
                PhotonNetwork.InstantiateRoomObject(enemyCharacterBoss.name, enemyDrop[Random.Range(0, enemyDrop.Length)].position, Quaternion.identity, 0, null);

                enemiesKilled = 0;

                yield return new WaitForSeconds(45f);

                spawnBoss = true;
            }

            yield return new WaitForSeconds(1);
        }
    }

    private void NetworkingClient_EventReceived(ExitGames.Client.Photon.EventData obj)
    {
        if (obj.Code == (byte)PUNEventDatabase.SpawnManager1UpdateEnemyCount)
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