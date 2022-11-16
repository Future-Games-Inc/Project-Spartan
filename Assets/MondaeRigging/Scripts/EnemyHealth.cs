using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;
using static RootMotion.FinalIK.GenericPoser;
using Unity.XR.CoreUtils;

public class EnemyHealth : MonoBehaviour
{
    public FollowAI aiScript;
    public GameObject xpDrop;
    public SpawnManager1 enemyCounter;
    public bool alive;

    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        enemyCounter = GameObject.FindGameObjectWithTag("spawnManager").GetComponent<SpawnManager1>();
        alive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (aiScript.Health <= 0 && alive == true)
        {
            alive = false;
            StartCoroutine(KillEnemy());
        }
    }

    IEnumerator KillEnemy()
    {
        yield return new WaitForSeconds(0);
        enemyCounter.enemyCount -= 1;
        StartCoroutine(DestroyEnemy());
    }

    IEnumerator DestroyEnemy()
    {
        yield return new WaitForSeconds(0);
        this.aiScript.enabled = false;
        animator.SetTrigger("Death");
        PhotonNetwork.Instantiate(xpDrop.name, transform.position, Quaternion.identity); 
        yield return new WaitForSeconds(4f);
        PhotonNetwork.Destroy(gameObject);
    }
}
