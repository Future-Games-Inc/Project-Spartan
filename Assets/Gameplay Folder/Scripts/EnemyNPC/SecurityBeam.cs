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
    void OnEnable()
    {
        beamMaterial.color = beamColor;
        detectedPlayer = null;
        InvokeRepeating("AlarmSound", 5f, 3f);
        StartCoroutine(Lost());
        enemyAI = GameObject.FindGameObjectsWithTag("Enemy");
    }

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
        foreach (GameObject enemy in enemyAI)
        {
            if (enemy.TryGetComponent(out FollowAI followAi))
            {
                followAi.AgroRange = 500;
                followAi.agent.speed = 3;
                followAi.inSight = true;
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
        foreach (GameObject enemy in enemyAI)
        {
            if (enemy.TryGetComponent(out FollowAI followAi))
            {
                followAi.AgroRange = 25f;
                followAi.agent.speed = 1.5f;
                followAi.inSight = false;
            }
        }
    }
}
