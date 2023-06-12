using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class FiringRangeAI : MonoBehaviour
{
    public enum States
    {
        Patrol,
        Chase,
        Attack,
        Shocked
    }
    [HideInInspector]
    public NavMeshAgent agent;
    private Animator animator;
    private ShootScript shootScript;
    private Ragdoll ragDoll;
    [HideInInspector]
    public AudioSource audioSource;
    public GameControl gameControl;

    [HideInInspector]
    public Transform targetTransform = null;

    [HideInInspector]
    public GameObject[] players;

    [Header("---------------- Ranges ------------------")]
    public float DetectRange;
    public float AttackRange;
    public float AgroRange;

    public LayerMask obstacleMask;

    public bool RangeDebug = true;
    private float DistanceToPlayer;
    [Header("-------------- Properties ----------------")]
    public int Health = 100;
    [HideInInspector]
    public bool inSight;
    [HideInInspector]
    public bool alive = true;
    public bool shootable = true;
    private Vector3 directionToTarget;
    private float TurnSpeed = 5f;
    private bool isLookingAtPlayer = false;
    private bool isShooting = false;
    // STATES --------------------
    public States currentState;
    private States previousState;
    [HideInInspector]
    public bool Agro = false;
    [HideInInspector]
    public bool shocked = false;
    [HideInInspector]
    public bool firstHit = false;
    // ---------------------------
    [HideInInspector]
    public int currentWaypoint;
    public Transform[] PatrolPoints;
    private int currentPatrolPointIndex = 0;
    private bool PatrolPauseDone = true;
    [Header("------------ Audio & Effects -------------")]
    public AudioClip[] VoiceLines;
    public AudioClip DefaultBulletHitSound;
    public GameObject hitEffect;
    [Header("------------ Weapons -------------")]
    public GameObject Bullet;
    public Transform FirePoint;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();
        shootScript = GetComponent<ShootScript>();
        ragDoll = GetComponent<Ragdoll>();
    }
    // Start is called before the first frame updat
    void Start()
    {
        hitEffect.SetActive(false);
        ragDoll.SetActive(false);
        InvokeRepeating("RandomSFX", 15, Random.Range(0, 30));
        //gameControl = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControl>();
        // GameObject waypointObject = GameObject.FindGameObjectWithTag("Waypoints");
        // waypoints = waypointObject.GetComponentsInChildren<Transform>();
        Patrol();
    }
    // Update is called once per frame
    void Update()
    {
        FindClosestEnemy();

        if (!alive) return;
        // calculates the distance from NPC to player
        float distanceToPlayer = Vector3.Distance(transform.position, targetTransform.position);
        // Debug.Log(distanceToPlayer);
        // match the animator Aggro with the NPC aggro
        animator.SetBool("Agro", Agro);

        if (currentState == States.Shocked)
        {
            return;
        }

        // If the player are seen by the NPC or if the distance is close enough
        if (distanceToPlayer <= DetectRange && CanSeeTarget())
        {
            Agro = true;
            animator.SetBool("Agro", Agro);
        }
        // if it is not agro, then patrol like normal
        // else then see if the player is in the valid agro range (bigger than detect range) then give chase
        //          if the player is outside of agro range, drop agro.
        if (!Agro)
        {
            SwitchStates(States.Patrol);
            Patrol();
        }
        else
        {
            // if the NPC is BEYOND the range that it could agro, drop the agro.
            if (distanceToPlayer > AgroRange)
            {
                Agro = false;
                // stop where it is
                agent.SetDestination(gameObject.transform.position);
                return;
            }
            // if in range of attacks
            if (distanceToPlayer <= AttackRange && Agro)
            {
                SwitchStates(States.Attack);
                LookatTarget(1, 3f);
                Attack();
            }
            // give chase if not in range
            else
            {
                SwitchStates(States.Chase);
                Follow();
            }
        }


        if (isLookingAtPlayer)
        {
            Vector3 direction = targetTransform.position - transform.position;
            direction.y = 0;
            Quaternion desiredRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * TurnSpeed);
        }

        // set the speed for the agent for the blend tree
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    public void SwitchStates(States input)
    {
        previousState = currentState;

        currentState = input;
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

    private bool CanSeeTarget()
    {
        Transform playerPOS = targetTransform;
        if (playerPOS == null)
            return false;

        if (Vector3.Distance(transform.position, playerPOS.position) > DetectRange)
            return false;

        Vector3 directionToTarget = (playerPOS.position - transform.position).normalized;
        if (Vector3.Angle(transform.forward, directionToTarget) > 110/2f)
            return false;

        RaycastHit[] hits = Physics.RaycastAll(transform.position, directionToTarget, DetectRange, obstacleMask);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
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
    // Cycle between patrol points
    private void Patrol()
    {
        
        // patrols from one place to the next
        if (agent.remainingDistance <= agent.stoppingDistance && PatrolPauseDone)
        {
            PatrolPauseDone = false;
            agent.isStopped = false;
            StartCoroutine(PatrolDelay());
        }


    }
    // Patrol to a point, waits a certain amount of seconds, and goes to the next point
    private IEnumerator PatrolDelay()
    {
        yield return new WaitForSeconds(3f);
        PatrolPauseDone = true;
        currentPatrolPointIndex = (currentPatrolPointIndex + 1) % PatrolPoints.Length;
        agent.destination = PatrolPoints[currentPatrolPointIndex].position;
        // agent.speed = PatrolPoints.WalkingSpeed;
    }

    private void Follow()
    {
        // interupts any patrolling process
        StopCoroutine(PatrolDelay());
        // set agent desitnation and speed
        agent.destination = targetTransform.position;
        // running speed
        // agent.speed = attributes.RunningSpeed;
    }

    private void Attack()
    {
        IEnumerator LookShootPause(float duration)
        {
            isShooting = true;
            /// LookatTarget(duration / 3, 3f);
            agent.isStopped = true;

            yield return new WaitForSeconds(duration);
            agent.isStopped = false;
            agent.SetDestination(targetTransform.position);
            isShooting = false;
        }

        IEnumerator Shoot(int amount)
        {
            for (int i = 0; i <= amount; i++)
            {
                // Instantiate a new instance of the Bullet prefab
                GameObject bulletInstance = Instantiate(Bullet, FirePoint.position, Quaternion.identity);
                if (shootable ) shootScript.Shoot(bulletInstance, FirePoint.position);
                yield return new WaitForSeconds(bulletInstance.GetComponent<BulletBehavior>().RateOfFire);
            }
        }

        if (!isShooting)
        {
            StartCoroutine(LookShootPause(3f));
            StartCoroutine(Shoot(10));
        }
    }

    public void LookatTarget(float duration, float RotationSpeed = 0.5f)
    {
        TurnSpeed = RotationSpeed;
        IEnumerator start()
        {
            isLookingAtPlayer = true;
            yield return new WaitForSeconds(duration);
            isLookingAtPlayer = false;
        }
        StartCoroutine(start());
    }

    public void TakeDamage(int damage = 0, BulletBehavior bullet = null)
    {
        // if there was a bullet in the damage taking option
        if (bullet == null)
        {
            if (alive == true)
            {
                if (!firstHit)
                {
                    StartCoroutine(FirstHit());
                }
                audioSource.PlayOneShot(DefaultBulletHitSound);
                Health -= damage;
            }
        } else
        // if the AI is just taking plain damage
        {
            if (alive == true)
            {
                if (!firstHit)
                {
                    StartCoroutine(FirstHit());
                }
                audioSource.PlayOneShot(bullet.hitSound);
                Health -= bullet.Damage;
            }
        }

        Agro = true;
        animator.SetBool("Agro", Agro);

        // Check for death
        if (Health <= 0 && alive == true)
        {
            StartCoroutine(Death());
        }
    }

    public void RandomSFX()
    {
        int playAudio = Random.Range(0, 70);
        if (!audioSource.isPlaying && playAudio <= 70)
            audioSource.PlayOneShot(VoiceLines[Random.Range(0, VoiceLines.Length)]);
    }

    IEnumerator Death()
    {
        alive = false;

        if (gameControl != null)
        {
            if (gameControl.enabled == true)
            {
                gameControl.EnemyKilled();
            }
        }
        // wait one frame
        yield return null;
        // activate ragdoll
        ragDoll.SetActive(true);
        agent.isStopped = true;
        StopAllCoroutines();
        Destroy(gameObject, 20);
    }
    IEnumerator FirstHit()
    {
        firstHit = true;
        hitEffect.SetActive(true);
        yield return new WaitForSeconds(3f);
        hitEffect.SetActive(false);
        firstHit = false;
    }

    // This function is complete unless we want to make adjustments/refinements
    public void EMPShock(GameObject Effect)
    {
        IEnumerator shock()
        {
            //States preState = currentState;
            SwitchStates(States.Shocked);
            agent.isStopped = true;
            animator.SetTrigger("Shock");
            animator.SetBool("ShockDone", false);
            Destroy(Instantiate(Effect, gameObject.transform.position + new Vector3(0,1,0), Quaternion.identity), 1f);
            // apply damage
            yield return new WaitForSeconds(1);
            TakeDamage(5);
            // play shock effect
            Destroy(Instantiate(Effect, gameObject.transform.position + new Vector3(0, 1, 0), Quaternion.identity), 1f);
            agent.isStopped = true;
            yield return new WaitForSeconds(1);
            TakeDamage(5);
            // play shock effect
            Destroy(Instantiate(Effect, gameObject.transform.position + new Vector3(0, 1, 0), Quaternion.identity), 1f);
            agent.isStopped = true;
            yield return new WaitForSeconds(1);
            TakeDamage(5);
            // play shock effect
            Destroy(Instantiate(Effect, gameObject.transform.position + new Vector3(0, 1, 0), Quaternion.identity), 0.5f);
            // enable movement
            animator.ResetTrigger("shock");
            animator.SetBool("ShockDone", true);
            currentState = previousState;
            agent.isStopped = false;
        }
        // if already shocked, ignore effects
        if (currentState == States.Shocked) return;
        StartCoroutine(shock());
    }


    /// <summary> ----------------------------------------------------------
    ///  Visualizatinon stuff
    /// </summary> ---------------------------------------------------------
    ///
    private void OnDrawGizmos()
    {
        if (!RangeDebug) return;
        // draw the NPC Vision cone
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * DetectRange);
        Vector3 rightDirection = Quaternion.Euler(0f, 110 / 2f, 0f) * transform.forward;
        Gizmos.DrawRay(transform.position, rightDirection * DetectRange);
        Vector3 leftDirection = Quaternion.Euler(0f, - 110 / 2f, 0f) * transform.forward;
        Gizmos.DrawRay(transform.position, leftDirection * DetectRange);
        // draw the ranges
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, DetectRange);
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, AgroRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
        // draw the destination
        if (agent != null)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(agent.destination, 0.5f);
        }
        // this part takes care of drawing the patrol points and line visuals between them
        Color lineColor = new Color32(0, 255, 0, 255);

        Transform[] PatrolPoints_Debug = null;

        if (Application.isEditor) PatrolPoints_Debug = PatrolPoints;

        for (int i = 0; i < PatrolPoints_Debug.Length; i++)
        {
            int colorVal = 40 * i;
            int colorValGreen = 255 - (40 * i);
            lineColor = new Color32((byte)colorVal, (byte)colorValGreen, (byte)colorVal, 255);

            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(PatrolPoints_Debug[i].position, 0.25f);
            if (i + 1 >= PatrolPoints_Debug.Length)
            {
                #if UNITY_EDITOR
                    UnityEditor.Handles.color = lineColor;
                    UnityEditor.Handles.DrawAAPolyLine(3, PatrolPoints_Debug[i].position, PatrolPoints_Debug[0].position);
                #endif
                return;
            }
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(PatrolPoints_Debug[i + 1].position, 0.25f);
            #if UNITY_EDITOR
                    UnityEditor.Handles.color = lineColor;
                    UnityEditor.Handles.DrawAAPolyLine(3, PatrolPoints_Debug[i].position, PatrolPoints_Debug[i + 1].position);
            #endif
        }
    }
}


