using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ReinforcementHealth : MonoBehaviour
{
    public ReinforcementAI aiScript;
    public SpawnManager1 enemyCounter;
    public bool alive;

    public Ragdoll ragDoll;

    public Animator animator;
    public GameObject deathElectric;
    public NavMeshAgent agent;

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
        StartCoroutine(Destroy());
    }

    IEnumerator Destroy()
    {
        enemyCounter.UpdateReinforcements();
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }
}
