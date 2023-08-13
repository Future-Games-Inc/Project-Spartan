using Photon.Pun;
using System.Collections;
using System.Threading.Tasks;
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
    void OnEnable()
    {
        beamMaterial.color = beamColor;
        detectedPlayer = null;
        InvokeRepeating("AlarmSound", 5f, 3f);
        Lost();
    }

    private void LostPlayer()
    {
        photonView.RPC("RPC_LostPlayer", RpcTarget.AllBuffered);
    }

    public void FoundPlayer()
    {
        photonView.RPC("RPC_TriggerEnter", RpcTarget.AllBuffered);

    }

    private async void Lost()
    {
        while (true)
        {
            if (lost == false)
            {
                NavMeshAgent droneAgent = securityDrone.GetComponent<NavMeshAgent>();
                if (droneAgent.isOnNavMesh)
                {
                    droneAgent.SetDestination(detectedPlayer.transform.position);
                }
                else
                {
                    // If droneAgent is not on NavMesh, find the nearest point and warp there
                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(droneAgent.transform.position, out hit, 10.0f, NavMesh.AllAreas))
                    {
                        droneAgent.Warp(hit.position);
                        droneAgent.SetDestination(detectedPlayer.transform.position);
                    }
                }
            }
            lostTimer += Time.deltaTime;
            if (lostTimer >= 10 && neverFound == false)
                LostPlayer();
            await Task.Yield();
        }
    }

    public void AlarmSound()
    {
        if (!alarmSource.isPlaying && lost == false)
            alarmSource.PlayOneShot(alarmClip);
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
            if (enemy.TryGetComponent(out FollowAI followAi))
            {
                followAi.AgroRange = 500;
                followAi.agent.speed = 3;
            }
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
        enemyAI = GameObject.FindGameObjectsWithTag("Enemy"); // Look for other options
        foreach (GameObject enemy in enemyAI)
        {
            if (enemy.TryGetComponent(out FollowAI followAi))
            {
                followAi.AgroRange = 25f;
                followAi.agent.speed = 1.5f;;
            }
        }
    }
}
