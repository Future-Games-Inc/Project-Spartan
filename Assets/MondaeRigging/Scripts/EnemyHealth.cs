using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemyHealth : MonoBehaviour
{
    public FollowAI aiScript;
    public GameObject xpDrop;
    public GameObject xpDropExtra;
    public SpawnManager1 enemyCounter;
    public bool alive;

    public Animator animator;
    public GameObject deathElectric;

    public Transform[] lootSpawn;
    public float xpDropRate;

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
            alive = false;
            KillEnemy();
        }
    }

    public void KillEnemy()
    {
        enemyCounter.enemyCount -= 1;
        enemyCounter.enemiesKilled += 1;
        DestroyEnemy();
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
                PhotonNetwork.Instantiate(xpDropExtra.name, transform.position, Quaternion.identity);
            }
            else
                PhotonNetwork.Instantiate(xpDrop.name, transform.position, Quaternion.identity);
        }

        StartCoroutine(Destroy());
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(4f); 
        PhotonNetwork.Destroy(gameObject);
    }
}
