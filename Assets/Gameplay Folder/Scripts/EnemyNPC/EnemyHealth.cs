using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;
using ExitGames.Client.Photon;
using Photon.Realtime;

public class EnemyHealth : MonoBehaviourPunCallbacks, IOnEventCallback
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
        enemyCounter = GameObject.FindGameObjectWithTag("spawnManager").GetComponent<SpawnManager1>();
        RaiseEventOptions options = new RaiseEventOptions() { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent((byte)PUNEventDatabase.EnemyHealth_EnemyHealthEnable, null, options, SendOptions.SendUnreliable);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.AddCallbackTarget(this);
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }
    }

    public void KillEnemy()
    {
        photonView.RPC("RPC_KillEnemy", RpcTarget.All);
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
                GameObject DropExtra = PhotonNetwork.InstantiateRoomObject(xpDropExtra.name, t.position, Quaternion.identity, 0, null);
                DropExtra.GetComponent<Rigidbody>().isKinematic = false;
            }
            else
            {
                GameObject DropNormal = PhotonNetwork.InstantiateRoomObject(xpDrop.name, t.position, Quaternion.identity, 0, null);
                DropNormal.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
        StartCoroutine(Destroy());
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(5f);
        PhotonNetwork.Destroy(gameObject);
    }

    void EnemyHealthEnable()
    {
        alive = true;
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (byte)PUNEventDatabase.EnemyHealth_EnemyHealthEnable)
        {
            EnemyHealthEnable();
        }
    }


    [PunRPC]
    void RPC_KillEnemy()
    {
        alive = false;
        aiScript.attackWeapon.fireWeaponBool = false;
        aiScript.alive = false;

        //enemyCounter.photonView.RPC("RPC_UpdateEnemy", RpcTarget.All);
        //enemyCounter.photonView.RPC("RPC_UpdateEnemyCount", RpcTarget.All);

        DestroyEnemy();
    }


    //[PunRPC]
    //void RPC_EnemyHealthEnable()
    //{
    //    alive = true;
    //}

    //[PunRPC]
    //void RPC_DestroyEnemy()
    //{
    //    this.aiScript.enabled = false;
    //    ragDoll.SetActive(true);
    //    agent.isStopped = true;
    //    deathElectric.SetActive(true);
    //}
}
