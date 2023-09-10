using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SecurityBeam : MonoBehaviour
{
    public List<GameObject> detectedEnemies; // List to store the enemies detected in range

    public float detectionRange = 100f; // Range within which to find enemies

    public AudioSource alarmSource;
    public AudioClip alarmClip;

    public GameObject securityDrone;
    public GameObject detectedPlayer;
    public Material beamMaterialNormal;
    public Material beamMaterialDetected;
    public GameObject securityBeam;

    public bool lost = true;
    public bool neverFound = true;
    public float lostTimer;

    void OnEnable()
    {
        detectedEnemies = new List<GameObject>();
        detectedPlayer = null;
        InvokeRepeating("AlarmSound", 5f, 3f);
        StartCoroutine(Lost());
    }

    private void LostPlayer()
    {
        lost = true;
        neverFound = true;

        securityBeam.GetComponent<MeshRenderer>().material = beamMaterialNormal;
        detectedPlayer = null;
        securityDrone.GetComponent<WanderingAI>().enabled = true;
        securityDrone.GetComponent<SecuityCamera>().enabled = true;
        securityDrone.GetComponent<NavMeshAgent>().speed = 0.5f;

        // Reset the properties for enemies in the detectedEnemies list
        foreach (GameObject enemy in detectedEnemies)
        {
            if (enemy.TryGetComponent(out FollowAI followAi))
            {
                followAi.AgroRange = 25f;
                followAi.DetectRange = 30f;
                if (enemy.tag == "Enemy")
                    followAi.agent.speed = 1.5f;
                else
                    followAi.agent.speed = 0.8f;
            }
        }
    }

    public void FoundPlayer()
    {
        lostTimer = 0;
        lost = false;
        neverFound = false;

        securityBeam.GetComponent<MeshRenderer>().material = beamMaterialDetected;
        securityDrone.GetComponent<WanderingAI>().enabled = false;
        securityDrone.GetComponent<SecuityCamera>().enabled = false;
        securityDrone.GetComponent<NavMeshAgent>().speed = 2;

        // Find and store enemies within a certain range
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange);
        detectedEnemies.Clear(); // Clear the previous list
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Enemy") || col.CompareTag("BossEnemy"))
            {
                detectedEnemies.Add(col.gameObject);
            }
        }

        // Update the properties for enemies in the detectedEnemies list
        foreach (GameObject enemy in detectedEnemies)
        {
            if (enemy.TryGetComponent(out FollowAI followAi))
            {
                followAi.AgroRange = 40f;
                followAi.DetectRange = 50f;
                followAi.agent.speed = 3f;
            }
        }
    }

    IEnumerator Lost()
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
                else if (!droneAgent.isOnNavMesh)
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
            yield return new WaitForSeconds(0.25f);
        }
    }

    public void AlarmSound()
    {
        if (!alarmSource.isPlaying && lost == false)
            alarmSource.PlayOneShot(alarmClip);
    }
}
