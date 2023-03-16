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
    public AIWeapon attackWeapon;
    public GameObject hitEffect;
    public AudioClip bulletHit;
    public AudioClip[] audioClip;

    [Header("AI Properties")]
    public float maxFollowDistance = 20f;
    public float shootDistance = 10f;
    public int Health;

    private Vector3 directionToTarget;
    private States currentState = States.Patrol;
    private bool inSight;
    public bool alive = true;
    private bool firstHit = false;
    public int currentWaypoint;

    public Transform[] waypoints;
    private GameObject[] players;

    private NavMeshTriangulation triangulation;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            agent = GetComponent<NavMeshAgent>();
            audioSource = GetComponent<AudioSource>();
            alive = true;

            triangulation = NavMesh.CalculateTriangulation();
            int vertexIndex = Random.Range(0, triangulation.vertices.Length);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(triangulation.vertices[vertexIndex], out hit, 2f, 1))
            {
                agent.Warp(hit.position);
                agent.enabled = true;
            }

            InvokeRepeating("RandomSFX", 15, Random.Range(0, 30));

            waypoints = GameObject.FindGameObjectWithTag("Waypoints").GetComponentsInChildren<Transform>();
            currentWaypoint = Random.Range(1, waypoints.Length);

            photonView.RPC("RPC_EnemyHealthMax", RpcTarget.All);
            players = GameObject.FindGameObjectsWithTag("Player");
            FindClosestEnemy();
        }
    }

    public void FindClosestEnemy()
    {
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
        if (PhotonNetwork.IsMasterClient)
        {
            CheckForPlayer();
            UpdateStates();
        }
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
            inSight = hitInfo.transform == targetTransform;

            if (inSight && hitInfo.distance <= shootDistance)
            {
                currentState = States.Attack;
            }
            else if (inSight && hitInfo.distance <= maxFollowDistance)
            {
                currentState = States.Follow;
            }
            else
            {
                currentState = States.Patrol;
            }
        }
        else
        {
            inSight = false;
            currentState = States.Patrol;
        }
    }

    private void Patrol()
    {
        attackWeapon.fireWeaponBool = false;
        agent.speed = 3.5f;

        if (Vector3.Distance(transform.position, waypoints[currentWaypoint].position) < 2f)
        {
            currentWaypoint = Random.Range(1, waypoints.Length);
        }

        agent.SetDestination(waypoints[currentWaypoint].position);
    }

    private void Follow()
    {
        attackWeapon.fireWeaponBool = false;
        if (targetTransform == null)
        {
            currentState = States.Patrol;
            return;
        }

        directionToTarget = targetTransform.position - transform.position;
        float distance = directionToTarget.magnitude;

        if (distance > maxFollowDistance)
        {
            currentState = States.Patrol;
            return;
        }

        if (distance < shootDistance)
        {
            currentState = States.Attack;
            return;
        }

        if (agent.enabled)
        {
            agent.SetDestination(targetTransform.position);
        }
    }

    private void Attack()
    {
        if (alive && inSight)
        {
            if (directionToTarget.magnitude < shootDistance)
            {
                transform.LookAt(targetTransform);
                attackWeapon.fireWeaponBool = true;
                audioSource.PlayOneShot(audioClip[1]);
            }
            else
            {
                currentState = States.Follow;
            }
        }
        else
        {
            currentState = States.Patrol;
        }
    }

    private void LookAtTarget()
    {
        Vector3 lookDirection = directionToTarget;
        lookDirection.y = 0f;

        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);

        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * agent.angularSpeed);
    }

    public void TakeDamage(int damage)
    {
        if (!alive)
            return;
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
}

