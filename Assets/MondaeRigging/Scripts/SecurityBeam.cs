using Photon.Pun;
using System.Collections;
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
    public bool neverFound = true;
    public float lostTimer;

    // Start is called before the first frame update
    void Start()
    {
        photonView.RPC("RPC_Start", RpcTarget.All);
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
        while (true)
        {
            if (lost == false)
            {
                NavMeshAgent droneAgent = securityDrone.GetComponent<NavMeshAgent>();
                droneAgent.SetDestination(detectedPlayer.transform.position);
            }
            lostTimer += Time.deltaTime;
            if (lostTimer >= 10 && neverFound == false)
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
        neverFound = false;

        beamMaterial.color = Color.red;
        securityDrone.GetComponent<WanderingAI>().enabled = false;
        securityDrone.GetComponent<SecuityCamera>().enabled = false;
        securityDrone.GetComponent<NavMeshAgent>().speed = 2;
        //droneAgent.SetDestination(detectedPlayer.transform.position);
        enemyAI = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemyAI)
        {
            enemy.TryGetComponent<FollowAI>(out var followAI);
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
        neverFound = true;

        beamMaterial.color = beamColor;
        detectedPlayer = null;
        securityDrone.GetComponent<WanderingAI>().enabled = true;
        securityDrone.GetComponent<SecuityCamera>().enabled = true;
        securityDrone.GetComponent<NavMeshAgent>().speed = 0.5f;
        enemyAI = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemyAI)
        {
            enemy.TryGetComponent<FollowAI>(out var followAI);
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
