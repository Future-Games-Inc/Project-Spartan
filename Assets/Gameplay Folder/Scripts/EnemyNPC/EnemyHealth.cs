using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    public FollowAI aiScript;
    public GameObject xpDrop;
    public GameObject xpDropExtra;
    public SpawnManager1 enemyCounter;
    public bool alive;

    public Ragdoll ragDoll;

    public Animator animator;
    public GameObject deathElectric;
    public NavMeshAgent agent;

    public Transform[] lootSpawn;
    public float xpDropRate;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void OnEnable()
    {
            alive = true;
            enemyCounter = GameObject.FindGameObjectWithTag("spawnManager").GetComponent<SpawnManager1>();
    }

    public void KillEnemy()
    {
        alive = false;
        aiScript.attackWeapon.fireWeaponBool = false;
        aiScript.alive = false;
        DestroyEnemy();
    }

    public void DestroyEnemy()
    {
        //photonView.RPC("RPC_DestroyEnemy", RpcTarget.All);
        this.aiScript.enabled = false;
        ragDoll.SetActive(true);
        agent.isStopped = true;
        deathElectric.SetActive(true);
        foreach (Transform t in lootSpawn)
        {
            if (gameObject.CompareTag("Enemy"))
            {
                xpDropRate = 5f;
            }

            else if (gameObject.CompareTag("BossEnemy"))
            {
                xpDropRate = 15f;
            }

            if (Random.Range(0, 100f) < xpDropRate)
            {
                GameObject DropExtra = Instantiate(xpDropExtra, t.position, Quaternion.identity);
                DropExtra.GetComponent<Rigidbody>().isKinematic = false;
            }
            else
            {
                GameObject DropNormal = Instantiate(xpDrop, t.position, Quaternion.identity);
                DropNormal.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
        StartCoroutine(Destroy());
    }

    IEnumerator Destroy()
    {
        enemyCounter.UpdateEnemy();
        enemyCounter.UpdateEnemyCount();
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }
}
