using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DroneHealth : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public int Health = 100;
    public GameObject xpDrop;
    public GameObject xpDropExtra;
    public SpawnManager1 enemyCounter;
    public bool alive = true;
    public Transform[] lootSpawn;
    public float xpDropRate;

    public AudioSource audioSource;
    public AudioClip bulletHit;

    public GameObject explosionEffect;
    //public EnemyHealthBar healthBar;
    public AudioClip[] audioClip;
    public NavMeshAgent agent;
    public string type;

    // Start is called before the first frame update
    void OnEnable()
    {
        enemyCounter = GameObject.FindGameObjectWithTag("spawnManager").GetComponent<SpawnManager1>();
        InvokeRepeating("RandomSFX", 15, 20f);
        photonView.RPC("RPC_OnEnable", RpcTarget.All);
    }

    // Update is called once per frame
    void Update()
    {
        if (agent != null && !agent.isOnNavMesh)
        {
            TakeDamage(300);
        }
    }

    public void TakeDamage(int damage)
    {
        //photonView.RPC("RPC_TakeDamage", RpcTarget.All, damage);
        audioSource.PlayOneShot(bulletHit);
        Health -= damage;
        //healthBar.SetCurrentHealth(Health);

        if (Health <= 0 && alive == true)
        {
            alive = false;
            enemyCounter.photonView.RPC("RPC_UpdateSecurity", RpcTarget.All);

            explosionEffect.SetActive(true);
            explosionEffect.GetComponentInChildren<ParticleSystem>().Play();

            agent.enabled = false;
            GetComponent<Rigidbody>().isKinematic = false;

            DestroyEnemy();
        }
    }

    private void DestroyEnemy()
    {
        foreach (Transform t in lootSpawn)
        {
            xpDropRate = 10f;
            if (Random.Range(0, 100f) < xpDropRate)
            {
                GameObject DropExtra = PhotonNetwork.InstantiateRoomObject(xpDropExtra.name, t.position, Quaternion.identity, 0);
                DropExtra.GetComponent<Rigidbody>().isKinematic = false;
            }
            else
            {
                GameObject DropNormal = PhotonNetwork.InstantiateRoomObject(xpDrop.name, t.position, Quaternion.identity, 0);
                DropNormal.GetComponent<Rigidbody>().isKinematic = false;
            }
        }

        //yield return new WaitForSeconds(.75f);
        PhotonNetwork.Destroy(gameObject);
    }

    public void RandomSFX()
    {
        photonView.RPC("RPC_PlayAudio", RpcTarget.All);
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (byte)PUNEventDatabase.DroneHealth_TakeDamage)
        {
            object[] data = (object[])photonEvent.CustomData;
            int damage = (int)data[0];
            TakeDamage(damage);
        }
    }

    [PunRPC]
    void RPC_OnEnable()
    {
        explosionEffect.SetActive(false);
        //healthBar.SetMaxHealth(Health);
        alive = true;
    }

    [PunRPC]
    void RPC_PlayAudio()
    {
        if (!audioSource.isPlaying)
            audioSource.PlayOneShot(audioClip[Random.Range(0, audioClip.Length)]);
    }

    

    //[PunRPC]
    //void RPC_TakeDamage(int damage)
    //{
    //    audioSource.PlayOneShot(bulletHit);
    //    Health -= damage;
    //    //healthBar.SetCurrentHealth(Health);

    //    if (Health <= 0 && alive == true)
    //    {
    //        alive = false;
    //        enemyCounter.photonView.RPC("RPC_UpdateSecurity", RpcTarget.All);

    //        explosionEffect.SetActive(true);

    //        agent.enabled = false;
    //        GetComponent<Rigidbody>().isKinematic = false;

    //        DestroyEnemy();
    //    }
    //}
}

