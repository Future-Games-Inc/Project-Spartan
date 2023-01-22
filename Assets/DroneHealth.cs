using PathologicalGames;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics.Internal;
using UnityEngine;
using UnityEngine.AI;

public class DroneHealth : MonoBehaviourPunCallbacks
{
    public int Health = 100;
    public GameObject xpDrop;
    public GameObject xpDropExtra;
    public SpawnManager1 enemyCounter;
    public bool alive;
    public Transform[] lootSpawn;
    public float xpDropRate;

    public AudioSource audioSource;
    public AudioClip bulletHit;

    public GameObject explosionEffect;
    public EnemyHealthBar healthBar;
    public AudioClip[] audioClip;
    public NavMeshAgent agent;

    public PhotonView photonView;

    // Start is called before the first frame update
    void OnEnable()
    {
        enemyCounter = GameObject.FindGameObjectWithTag("spawnManager").GetComponent<SpawnManager1>();
        photonView = GetComponent<PhotonView>();
        if (PhotonNetwork.IsMasterClient)
        {
            InvokeRepeating("RandomSFX", 15, Random.Range(0, 30));
            photonView.RPC("RPC_OnEnable", RpcTarget.All);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (Health <= 0 && alive == true)
            {
                StartCoroutine(DestroyEnemy());
            }
        }
    }

    public void TakeDamage(int damage)
    {
        photonView.RPC("TakeDamage", RpcTarget.All, damage);
        //audioSource.PlayOneShot(bulletHit);
        //Health -= damage;
        //healthBar.SetCurrentHealth(Health);
    }

    //IEnumerator KillDrone()
    //{
    //    yield return new WaitForSeconds(.25f);
    //    enemyCounter.securityCount -= 1;
    //    StartCoroutine(DestroyEnemy());
    //}

    IEnumerator DestroyEnemy()
    {
        yield return new WaitForSeconds(0);
        //explosionEffect.SetActive(true);
        foreach (Transform t in lootSpawn)
        {
            xpDropRate = 10f;
            if (Random.Range(0, 100f) < xpDropRate)
            {
                PhotonNetwork.InstantiateRoomObject(xpDropExtra.name, transform.position, Quaternion.identity);
            }
            else
                PhotonNetwork.InstantiateRoomObject(xpDrop.name, transform.position, Quaternion.identity);
        }
        PhotonNetwork.InstantiateRoomObject(xpDrop.name, transform.position, Quaternion.identity);
        //agent = GetComponent<NavMeshAgent>();
        //agent.enabled = false;

        yield return new WaitForSeconds(.75f);
        PhotonNetwork.Destroy(gameObject);
    }

    public void RandomSFX()
    {
        //int playAudio = Random.Range(0, 70);
        photonView.RPC("RPC_PlayAudio", RpcTarget.All);
        //if (!audioSource.isPlaying && playAudio <= 70)
        //    audioSource.PlayOneShot(audioClip[Random.Range(0, audioClip.Length)]);
    }

    [PunRPC]

    void RPC_OnEnable()
    {
        explosionEffect.SetActive(false);
        alive = true;
        healthBar.SetMaxHealth(Health);
    }

    [PunRPC]
    void RPC_TakeDamage(int damage)
    {
        audioSource.PlayOneShot(bulletHit);
        Health -= damage;
        healthBar.SetCurrentHealth(Health);

        if (Health <= 0 && alive == true)
        {
            alive = false;
            photonView.RPC("RPC_UpdateSecurity", RpcTarget.AllBuffered);

            explosionEffect.SetActive(true);

            agent = GetComponent<NavMeshAgent>();
            agent.enabled = false;

            StartCoroutine(DestroyEnemy());
        }
    }

    [PunRPC]
    void RPC_PlayAudio()
    {
        int playAudio = Random.Range(0, 70);
        if (!audioSource.isPlaying && playAudio <= 70)
            audioSource.PlayOneShot(audioClip[Random.Range(0, audioClip.Length)]);
    }
}

