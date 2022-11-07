using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemyHealth : MonoBehaviour
{
    public FollowAI aiScript;
    public float Health;
    public GameObject xpDrop;
    public SpawnManager1 enemyCounter;
    public bool alive;

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
            StartCoroutine(KillEnemy());
        }

    }

    public void TakeDamage(int damage)
    {


    }

    IEnumerator KillEnemy()
    {
        yield return new WaitForSeconds(0);
        alive = false;
        PhotonNetwork.Instantiate(xpDrop.name, transform.position, Quaternion.identity);
        if (gameObject.tag == "Enemy")
        {
            enemyCounter.enemyCount -= 1;
        }
        if (gameObject.tag == "Security")
        {
            enemyCounter.securityCount -= 1;
        }
        Invoke(nameof(DestroyEnemy),0f);
    }

    private void DestroyEnemy()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
