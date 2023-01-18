using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using CSCore;
using Invector.vCharacterController.AI;

public class FollowAI : MonoBehaviourPunCallbacks
{
    public enum States
    {
        Patrol,
        Follow,
        Attack
    }

    public NavMeshAgent agent;
    public Transform targetTransform;
    public EnemyHealthBar healthBar;

    public Transform[] waypoints;
    public GameObject[] players;

    [Header("AI Properties")]
    public float maxFollowDistance = 20f;
    public float shootDistance = 10f;
    public AIWeapon attackWeapon;
    public int Health;

    public bool inSight;
    private Vector3 directionToTarget;

    public States currentState;

    public int currentWaypoint;

    public AudioSource audioSource;
    public AudioClip bulletHit;
    public AudioClip[] audioClip;
    public PhotonView photonView;

    // Start is called before the first frame update
    void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
        photonView = GetComponent<PhotonView>();

        NavMeshTriangulation Triangulation = NavMesh.CalculateTriangulation();
        int VertexIndex = Random.Range(0, Triangulation.vertices.Length);
        NavMeshHit Hit;
        if (NavMesh.SamplePosition(Triangulation.vertices[VertexIndex], out Hit, 2f, 1))
        {
            agent.Warp(Hit.position);
            agent.enabled = true;
        }

        InvokeRepeating("RandomSFX", 15, Random.Range(0, 30));
        GameObject waypointObject = GameObject.FindGameObjectWithTag("Waypoints");
        waypoints = waypointObject.GetComponentsInChildren<Transform>();

        currentWaypoint = Random.Range(1, 9);

        FindClosestEnemy();
        healthBar.SetMaxHealth(Health);
    }

    public void FindClosestEnemy()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in players)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        targetTransform = closest.transform;
    }

    // Update is called once per frame
    void Update()
    {
        FindClosestEnemy();
        CheckForPlayer();
        UpdateStates();

    }

    private void UpdateStates()
    {
        switch (currentState)
        {
            case States.Patrol:
                Patrol();
                break;
            case States.Follow:
                Follow();
                break;
            case States.Attack:
                Attack();
                break;
        }
    }
    private void CheckForPlayer()
    {
        directionToTarget = targetTransform.position - transform.position;

        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, directionToTarget.normalized, out hitInfo))
        {
            inSight = hitInfo.transform.CompareTag("Player");
        }
    }
    private void Patrol()
    {
        attackWeapon.fireWeaponBool = false;
        if (agent.destination != waypoints[currentWaypoint].position && agent.enabled == true)
        {
            agent.destination = waypoints[currentWaypoint].position;
        }

        if (HasReached())
        {
            currentWaypoint = (currentWaypoint + Random.Range(1, 6)) % waypoints.Length;
        }

        if (inSight && directionToTarget.magnitude <= maxFollowDistance)
        {
            currentState = States.Follow;
        }
    }

    private void Follow()
    {
        attackWeapon.fireWeaponBool = false;
        if (directionToTarget.magnitude <= shootDistance && inSight)
        {
            agent.ResetPath();
            currentState = States.Attack;
        }

        else
        {
            if (targetTransform != null && agent.enabled == true)
            {
                agent.SetDestination(targetTransform.position);
            }

            if (directionToTarget.magnitude > maxFollowDistance && agent.enabled == true)
            {
                currentState = States.Patrol;
            }
        }
    }

    private void Attack()
    {
        if (!inSight || directionToTarget.magnitude > shootDistance)
        {
            currentState = States.Follow;
        }
        LookAtTarget();
        attackWeapon.Fire();
    }

    private void LookAtTarget()
    {
        Vector3 lookDirection = directionToTarget;
        lookDirection.y = 0f;

        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);

        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * agent.angularSpeed);
    }

    private bool HasReached()
    {
        return (agent.hasPath && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance);
    }

    public void TakeDamage(int damage)
    {
        photonView.RPC("RPC_TakeDamage", RpcTarget.All);
    }

    public void RandomSFX()
    {
        photonView.RPC("RPC_PlayAudio", RpcTarget.All);
    }

    [PunRPC]
    void RPC_TakeDamage(int damage)
    {
        if (!photonView.IsMine)
        { return; }

        audioSource.PlayOneShot(bulletHit);
        Health -= damage;
        healthBar.SetCurrentHealth(Health);
    }

    [PunRPC]
    void RPC_PlayAudio()
    {
        if (!photonView.IsMine) { return; }

        int playAudio = Random.Range(0, 70);
        if (!audioSource.isPlaying && playAudio <= 70)
            audioSource.PlayOneShot(audioClip[Random.Range(0, audioClip.Length)]);
    }
}
