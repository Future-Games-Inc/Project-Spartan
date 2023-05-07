using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DroneHealth : MonoBehaviourPunCallbacks
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

    // Start is called before the first frame update
    void OnEnable()
    {
        enemyCounter = GameObject.FindGameObjectWithTag("spawnManager").GetComponent<SpawnManager1>();
        InvokeRepeating("RandomSFX", 15, 20f);
        photonView.RPC("RPC_OnEnable", RpcTarget.All, 0, null);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(int damage)
    {
        photonView.RPC("RPC_TakeDamage", RpcTarget.All, 0, null, damage);
    }

    IEnumerator DestroyEnemy()
    {
        yield return new WaitForSeconds(0);
        foreach (Transform t in lootSpawn)
        {
            xpDropRate = 10f;
            if (Random.Range(0, 100f) < xpDropRate)
            {
                PhotonNetwork.Instantiate(xpDropExtra.name, t.position, Quaternion.identity, 0);
            }
            else
                PhotonNetwork.Instantiate(xpDrop.name, t.position, Quaternion.identity, 0);
        }

        yield return new WaitForSeconds(.75f);
        PhotonNetwork.Destroy(gameObject);
    }

    public void RandomSFX()
    {
        photonView.RPC("RPC_PlayAudio", RpcTarget.All, 0, null);
    }

    [PunRPC]
    void RPC_OnEnable()
    {
        explosionEffect.SetActive(false);
        //healthBar.SetMaxHealth(Health);
        alive = true;
    }

    [PunRPC]
    void RPC_TakeDamage(int damage)
    {
        audioSource.PlayOneShot(bulletHit);
        Health -= damage;
        //healthBar.SetCurrentHealth(Health);

        if (Health <= 0 && alive == true)
        {
            alive = false;
            enemyCounter.photonView.RPC("RPC_UpdateSecurity", RpcTarget.All, 0, null);

            explosionEffect.SetActive(true);

            agent.enabled = false;

            StartCoroutine(DestroyEnemy());
        }
    }

    [PunRPC]
    void RPC_PlayAudio()
    {
        if (!audioSource.isPlaying)
            audioSource.PlayOneShot(audioClip[Random.Range(0, audioClip.Length)]);
    }
}

