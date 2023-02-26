using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using System.Collections;

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
    public EnemyHealth enemyHealth;

    public Transform[] waypoints;
    public GameObject[] players;
    public GameObject hitEffect;

    [Header("AI Properties")]
    public float maxFollowDistance = 20f;
    public float shootDistance = 10f;
    public AIWeapon attackWeapon;
    public int Health;

    public bool inSight;
    public bool alive = true;
    public bool firstHit = false;
    private Vector3 directionToTarget;

    public States currentState;

    public int currentWaypoint;

    public AudioSource audioSource;
    public AudioClip bulletHit;
    public AudioClip[] audioClip;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        alive = true;
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

        photonView.RPC("RPC_EnemyHealthMax", RpcTarget.All);
        FindClosestEnemy();
    }

    public void FindClosestEnemy()
    {
        photonView.RPC("RPC_FindClosestEnemy", RpcTarget.All);
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
        photonView.RPC("RPC_UpdateStates", RpcTarget.All);
    }
    private void CheckForPlayer()
    {
        photonView.RPC("RPC_CheckForPlayer", RpcTarget.All);
    }
    private void Patrol()
    {
        photonView.RPC("RPC_Patrol", RpcTarget.All);
    }

    private void Follow()
    {
        photonView.RPC("RPC_Follow", RpcTarget.All);
    }

    private void Attack()
    {
        photonView.RPC("RPC_Attack", RpcTarget.All);
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
        if (alive == true)
            photonView.RPC("RPC_TakeDamageEnemy", RpcTarget.All, damage);
    }

    public void RandomSFX()
    {
        photonView.RPC("RPC_PlayAudioEnemy", RpcTarget.All);
    }

    IEnumerator StopHit()
    {
        yield return new WaitForSeconds(3f);
        photonView.RPC("RPC_StopHit", RpcTarget.All);
    }

    [PunRPC]
    void RPC_EnemyHealthMax()
    {
        healthBar.SetMaxHealth(Health);
    }

    [PunRPC]
    void RPC_StopHit()
    {
        hitEffect.SetActive(false);
        firstHit = false;
    }

    [PunRPC]
    void RPC_TakeDamageEnemy(int damage)
    {
        audioSource.PlayOneShot(bulletHit);
        Health -= damage;
        healthBar.SetCurrentHealth(Health);

        if (Health <= 0 && alive == true)
        {
            alive = false;
            enemyHealth.KillEnemy();
        }

        if (!firstHit)
        {
            hitEffect.SetActive(true);
            firstHit = true;
            StartCoroutine(StopHit());
        }
    }

    [PunRPC]
    void RPC_PlayAudioEnemy()
    {
        int playAudio = Random.Range(0, 70);
        if (!audioSource.isPlaying && playAudio <= 70)
            audioSource.PlayOneShot(audioClip[Random.Range(0, audioClip.Length)]);
    }

    [PunRPC]
    void RPC_UpdateStates()
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

    [PunRPC]
    void RPC_CheckForPlayer()
    {
        directionToTarget = targetTransform.position - transform.position;

        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, directionToTarget.normalized, out hitInfo))
        {
            inSight = hitInfo.transform.CompareTag("Player");
        }
    }

    [PunRPC]
    void RPC_Patrol()
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

    [PunRPC]
    void RPC_Follow()
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

    [PunRPC]
    void RPC_Attack()
    {
        attackWeapon.fireWeaponBool = true;
        if (!inSight || directionToTarget.magnitude > shootDistance)
        {
            currentState = States.Follow;
        }
        LookAtTarget();
    }

    [PunRPC]
    void RPC_FindClosestEnemy()
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
}

