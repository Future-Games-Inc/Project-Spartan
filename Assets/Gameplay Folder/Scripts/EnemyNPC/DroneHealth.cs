using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class DroneHealth : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public int Health = 100;
    public GameObject xpDrop;
    public GameObject xpDropExtra;
    public SpawnManager1 enemyCounter;
    public bool alive = true;
    public Transform[] lootSpawn;
    public float xpDropRate = 10f;

    public AudioSource audioSource;
    public AudioClip bulletHit;
    public AudioClip[] audioClip;
    public GameObject explosionEffect;
    public NavMeshAgent agent;

    public string type;

    void OnEnable()
    {
        enemyCounter = GameObject.FindGameObjectWithTag("spawnManager").GetComponent<SpawnManager1>();
        InvokeRepeating("RandomSFX", 15, 20f);
        explosionEffect.SetActive(false);
        alive = true;
    }

    void Update()
    {
        if (agent != null && !agent.isOnNavMesh)
        {
            TakeDamage(300);
        }
    }

    public void TakeDamage(int damage)
    {
        photonView.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(int damage)
    {
        audioSource.PlayOneShot(bulletHit);
        Health -= damage;

        if (Health <= 0 && alive)
        {
            alive = false;
            TriggerDeathEffects();
        }
    }

    private void TriggerDeathEffects()
    {
        explosionEffect.SetActive(true);
        agent.enabled = false;
        GetComponent<Rigidbody>().isKinematic = false;
        SpawnLoot();
        enemyCounter.photonView.RPC("RPC_UpdateSecurity", RpcTarget.All);
        PhotonNetwork.Destroy(gameObject);
    }

    private void SpawnLoot()
    {
        foreach (Transform t in lootSpawn)
        {
            GameObject drop = Random.Range(0, 100f) < xpDropRate ? xpDropExtra : xpDrop;
            GameObject loot = PhotonNetwork.InstantiateRoomObject(drop.name, t.position, Quaternion.identity, 0);
            loot.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    public void RandomSFX()
    {
        if (alive && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(audioClip[Random.Range(0, audioClip.Length)]);
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        // Handle any photon events here if needed
    }
}
