using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SecurityBeam : MonoBehaviourPunCallbacks
{
    public GameObject[] enemyAI;

    public AudioSource alarmSource;
    public AudioClip alarmClip;

    public GameObject securityDrone;
    public GameObject detectedPlayer;
    public Material beamMaterial;
    public Color beamColor;

    public bool lost = true;
    public float lostTimer;

    // Start is called before the first frame update
    void Start()
    {
        photonView.RPC("RPC_Start", RpcTarget.AllBuffered);
        StartCoroutine(Lost());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {

    }

    private void OnTriggerExit(Collider other)
    {

    }

    //public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    //{
    //    Vector3 randDirection = Random.insideUnitSphere * dist;

    //    randDirection += origin;

    //    NavMeshHit navHit;

    //    NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

    //    return navHit.position;
    //}

    IEnumerator LostPlayer()
    {
        yield return new WaitForSeconds(0);
        photonView.RPC("RPC_LostPlayer", RpcTarget.AllBuffered);
    }

    public IEnumerator FoundPlayer()
    {
        yield return new WaitForSeconds(0);
        photonView.RPC("RPC_TriggerEnter", RpcTarget.AllBuffered);

    }

    IEnumerator Lost()
    {
        while(true)
        {
            if (lost == false)
            {
                NavMeshAgent droneAgent = securityDrone.GetComponent<NavMeshAgent>();
                droneAgent.SetDestination(detectedPlayer.transform.position);
                beamMaterial.color = Color.red;
            }
            lostTimer += Time.deltaTime;
            if (lostTimer >= 10)
                StartCoroutine(LostPlayer());
            yield return null;
        }
    }

    public void AlarmSound()
    {
        photonView.RPC("RPC_AlarmSound", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void RPC_Start()
    {
        beamMaterial.color = beamColor;
        detectedPlayer = null;
        InvokeRepeating("AlarmSound", 0f, 3f);
    }

    [PunRPC]
    void RPC_TriggerEnter()
    {
        lostTimer = 0;
        lost = false;

        WanderingAI wander = securityDrone.GetComponent<WanderingAI>();
        wander.enabled = false;
        SecuityCamera droneCamera = securityDrone.GetComponent<SecuityCamera>();
        droneCamera.enabled = false;
        NavMeshAgent droneAgent = securityDrone.GetComponent<NavMeshAgent>();
        droneAgent.speed = 2;
        //droneAgent.SetDestination(detectedPlayer.transform.position);
        enemyAI = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemyAI)
        {
            FollowAI followAI = enemy.GetComponent<FollowAI>();
            followAI.targetTransform = detectedPlayer.gameObject.transform;
            followAI.maxFollowDistance = 500;
            followAI.agent.speed = 3;
            followAI.agent.SetDestination(followAI.targetTransform.position);
        }
    }

    [PunRPC]
    void RPC_LostPlayer()
    {
        lost = true;

        beamMaterial.color = beamColor;
        detectedPlayer = null;
        WanderingAI wander = securityDrone.GetComponent<WanderingAI>();
        wander.enabled = true;
        SecuityCamera droneCamera = securityDrone.GetComponent<SecuityCamera>();
        droneCamera.enabled = true;
        NavMeshAgent droneAgent = securityDrone.GetComponent<NavMeshAgent>();
        droneAgent.speed = 0.5f;
        enemyAI = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemyAI)
        {
            FollowAI followAI = enemy.GetComponent<FollowAI>();
            followAI.maxFollowDistance = 5f;
            followAI.agent.speed = 1f;
            followAI.currentWaypoint = (0 + Random.Range(0, 6)) % followAI.waypoints.Length;
            followAI.agent.SetDestination(followAI.waypoints[followAI.currentWaypoint].position);
        }
    }

    [PunRPC]
    void RPC_AlarmSound()
    {
        if (!alarmSource.isPlaying && lost == false)
            alarmSource.PlayOneShot(alarmClip);
    }
}
