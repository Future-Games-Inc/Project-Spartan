using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.Services.Analytics.Internal;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
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
    public PhotonView photonView;

    // Start is called before the first frame update
    void OnEnable()
    {
        enemyCounter = GameObject.FindGameObjectWithTag("spawnManager").GetComponent<SpawnManager1>();
        alive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (aiScript.Health <= 0 && alive == true)
        {
            KillEnemy();
        }
    }

    public void KillEnemy()
    {
        photonView.RPC("RPC_KillEnemy", RpcTarget.All);
        //DestroyEnemy();
    }

    public void DestroyEnemy()
    {
        this.aiScript.enabled = false;
        animator.SetTrigger("Death");
        deathElectric.SetActive(true);

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
                PhotonNetwork.InstantiateRoomObject(xpDropExtra.name, transform.position, Quaternion.identity);
            }
            else
                PhotonNetwork.InstantiateRoomObject(xpDrop.name, transform.position, Quaternion.identity);
        }

        StartCoroutine(Destroy());
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(4f);
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    void RPC_KillEnemy()
    {
        if (!photonView.IsMine)
        { return; }

        alive = false;

        enemyCounter.enemyCount -= 1;
        enemyCounter.enemiesKilled += 1;
        DestroyEnemy();
    }
}
