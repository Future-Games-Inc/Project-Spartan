using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class FiringRangeAI : MonoBehaviour
{
    public enum States
    {
        Patrol,
        Follow,
        Attack,
        Shocked
    }

    public NavMeshAgent agent;
    public Transform targetTransform;
    public GameControl gameControl;
    private Animator animator;

    public Transform[] waypoints;
    public GameObject[] players;
    public GameObject hitEffect;

    [Header("AI Properties")]
    public float maxFollowDistance = 20f;
    public float shootDistance = 10f;
    public int Health;

    public bool inSight;
    public bool alive = true;
    private Vector3 directionToTarget;

    public States currentState;

    public int currentWaypoint;

    public AudioSource audioSource;
    public AudioClip bulletHit;
    public AudioClip[] audioClip;
    public GameObject deathEffect;

    public bool firstHit = false;
    [HideInInspector]
    public bool shocked = false;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    // Start is called before the first frame updat
    void Start()
    {
        // Health = 100;
        agent = GetComponent<NavMeshAgent>();
        deathEffect.SetActive(false);
        hitEffect.SetActive(false);
        //gameControl = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControl>();

        alive = true;

        InvokeRepeating("RandomSFX", 15, Random.Range(0, 30));
        // GameObject waypointObject = GameObject.FindGameObjectWithTag("Waypoints");
        // waypoints = waypointObject.GetComponentsInChildren<Transform>();
        currentWaypoint = Random.Range(0, waypoints.Length);
        FindClosestEnemy();
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
        // if the AI is in a shocked mode stop them from moving
        if (currentState != States.Shocked)
        {
            FindClosestEnemy();
            CheckForPlayer();
            UpdateStates();

        }
        else if (currentState == States.Shocked)
        {
            
            agent.isStopped = true;
            agent.SetDestination(gameObject.transform.position);
        }


        if (Health <= 0 && alive == true)
        {
            alive = false;
            deathEffect.SetActive(true);
            StartCoroutine(Death());
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
            inSight = hitInfo.transform.CompareTag("Player");
        }
    }
    private void Patrol()
    {
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
        {
            if (!firstHit)
            {
                hitEffect.SetActive(true);
                firstHit = true;
                StartCoroutine(StopHit());
            }
            audioSource.PlayOneShot(bulletHit);
            Health -= damage;
        }
    }

    public void RandomSFX()
    {
        int playAudio = Random.Range(0, 70);
        if (!audioSource.isPlaying && playAudio <= 70)
            audioSource.PlayOneShot(audioClip[Random.Range(0, audioClip.Length)]);
    }

    IEnumerator Death()
    {
        //if (gameControl.enabled == true)
        //{
        //    gameControl.EnemyKilled();
        //}
        yield return new WaitForSeconds(0.75f);
        Destroy(gameObject);
    }
    IEnumerator StopHit()
    {
        yield return new WaitForSeconds(3f);
        hitEffect.SetActive(false);
        firstHit = false;
    }

    public void EMPShock()
    {
        IEnumerator shock()
        {
            //States preState = currentState;
            currentState = States.Shocked;
            agent.isStopped = true;
            animator.enabled = false;

            // apply damage
            yield return new WaitForSeconds(1);
            TakeDamage(5);
            // play shock effect
            Destroy(Instantiate(hitEffect, transform.position, Quaternion.identity), 1f);
            yield return new WaitForSeconds(1);
            TakeDamage(5);
            // play shock effect
            Destroy(Instantiate(hitEffect, transform.position, Quaternion.identity), 1f);
            yield return new WaitForSeconds(1);
            TakeDamage(5);
            // play shock effect
            Destroy(Instantiate(hitEffect, transform.position, Quaternion.identity), 1f);
            // enable movement

            currentState = States.Follow;
            agent.isStopped = false;

            animator.enabled = true;
        }
        // if already shocked, ignore effects
        if (currentState == States.Shocked) return;
        StartCoroutine(shock());
    }

}


