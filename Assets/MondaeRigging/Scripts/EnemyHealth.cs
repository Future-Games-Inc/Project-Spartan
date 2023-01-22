using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.Services.Analytics.Internal;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviourPunCallbacks
{
    public FollowAI aiScript;
    public GameObject xpDrop;
    public GameObject xpDropExtra;
    public SpawnManager1 enemyCounter;
    public bool alive;

    public Animator animator;
    public GameObject deathElectric;
    public NavMeshAgent agent;

    public Transform[] lootSpawn;
    public float xpDropRate;

    // Start is called before the first frame update
    void OnEnable()
    {
        enemyCounter = GameObject.FindGameObjectWithTag("spawnManager").GetComponent<SpawnManager1>();
        photonView.RPC("RPC_EnemyHealthEnable", RpcTarget.AllBuffered);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void KillEnemy()
    {
        photonView.RPC("RPC_KillEnemy", RpcTarget.AllBuffered);
    }

    public void DestroyEnemy()
    {
        photonView.RPC("RPC_DestroyEnemy", RpcTarget.AllBuffered);
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (Transform t in lootSpawn)
            {
                if (tag == "Enemy")
                {
                    xpDropRate = 5f;
                }

                else if (tag == "BossEnemy")
                {
                    xpDropRate = 15f;
                }

                if (Random.Range(0, 100f) < xpDropRate)
                {
                    if (PhotonNetwork.IsMasterClient)
                        PhotonNetwork.InstantiateRoomObject(xpDropExtra.name, transform.position, Quaternion.identity);
                }
                else
                    if (PhotonNetwork.IsMasterClient)
                    PhotonNetwork.InstantiateRoomObject(xpDrop.name, transform.position, Quaternion.identity);
            }

            StartCoroutine(Destroy());
        }
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(4f);
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    void RPC_EnemyHealthEnable()
    {
        alive = true;
    }

    [PunRPC]
    void RPC_KillEnemy()
    {
        alive = false;

        photonView.RPC("RPC_UpdateEnemy()", RpcTarget.AllBuffered);
        photonView.RPC("RPC_UpdateEnemyCount()", RpcTarget.AllBuffered);
        DestroyEnemy();
    }

    [PunRPC]
    void RPC_DestroyEnemy()
    {
        this.aiScript.enabled = false;
        animator.SetTrigger("Death");
        deathElectric.SetActive(true);

        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;
    }
}
