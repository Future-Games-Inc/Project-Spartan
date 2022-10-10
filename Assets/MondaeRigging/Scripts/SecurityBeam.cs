using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SecurityBeam : MonoBehaviour
{
    public GameObject[] enemyAI;

    public AudioSource alarmSource;
    public AudioClip alarmClip;

    public GameObject securityDrone;
    public GameObject detectedPlayer;
    public Material beamMaterial;
    public Color beamColor;

    public bool lost = true;


    // Start is called before the first frame update
    void Start()
    {
        beamColor = beamMaterial.color;
        enemyAI = GameObject.FindGameObjectsWithTag("Enemy");
        detectedPlayer = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (lost == false)
        {
            NavMeshAgent droneAgent = securityDrone.GetComponent<NavMeshAgent>();
            droneAgent.SetDestination(detectedPlayer.transform.position);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            detectedPlayer = other.gameObject;
            lost = false;
        }

        if (lost == false)
        {
            beamMaterial.color = Color.red;
            WanderingAI wander = securityDrone.GetComponent<WanderingAI>();
            wander.enabled = false;
            SecuityCamera droneCamera = securityDrone.GetComponent<SecuityCamera>();
            droneCamera.enabled = false;
            NavMeshAgent droneAgent = securityDrone.GetComponent<NavMeshAgent>();
            droneAgent.speed = 2;
            droneAgent.SetDestination(other.transform.position);
            foreach (GameObject enemy in enemyAI)
            {
                FollowAI followAI = enemy.GetComponent<FollowAI>();
                followAI.targetTransform = other.gameObject.transform;
                followAI.maxFollowDistance = 500;
                followAI.agent.speed = 3;
                followAI.agent.SetDestination(followAI.targetTransform.position);
            }

        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(LostPlayer());
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }

    IEnumerator LostPlayer()
    {
        yield return new WaitForSeconds(10);
        lost = true;

        if (lost == true)
        {
            beamMaterial.color = beamColor;
            WanderingAI wander = securityDrone.GetComponent<WanderingAI>();
            wander.enabled = true;
            SecuityCamera droneCamera = securityDrone.GetComponent<SecuityCamera>();
            droneCamera.enabled = true;
            NavMeshAgent droneAgent = securityDrone.GetComponent<NavMeshAgent>();
            droneAgent.speed = 0.5f;
            foreach (GameObject enemy in enemyAI)
            {
                FollowAI followAI = enemy.GetComponent<FollowAI>();
                followAI.maxFollowDistance = 5;
                followAI.agent.speed = 1;
                followAI.currentWaypoint = (0 + Random.Range(0, 6)) % followAI.waypoints.Length;
                followAI.agent.SetDestination(followAI.waypoints[followAI.currentWaypoint].position);
            }
        }

    }
}
