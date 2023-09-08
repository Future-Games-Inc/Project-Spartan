using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System;
using Unity.XR.CoreUtils;
using System.Linq;

public class FollowAI : MonoBehaviour
{
    public enum States
    {
        Patrol,
        Follow,
        Attack,
        Shocked
    }

    [Header("AI Settings --------------------------------------------------------------")]
    public float DetectRange = 20;
    public float AttackRange = 15;
    public float AgroRange = 25;
    public float wanderRadius = 500;

    public int Health;

    public LayerMask obstacleMask;

    private float DistanceToPlayer;

    public bool fireReady = false;

    public NavMeshAgent agent;
    public Transform targetTransform;
    //public EnemyHealthBar healthBar;
    public EnemyHealth enemyHealth;
    public AIWeapon attackWeapon;
    public GameObject hitEffect;
    public GameObject shockEffect;

    private float TurnSpeed = 5f;
    private bool isLookingAtPlayer = false;

    [Header("AI Behavior --------------------------------------------------------------")]
    private Vector3 directionToTarget;

    public bool inSight;
    public bool alive = true;
    private bool firstHit = false;

    private bool PatrolPauseDone = true;

    private GameObject[] players;

    public States currentState;
    private States previousState;

    [HideInInspector]
    public bool Agro = false;
    [HideInInspector]
    public bool shocked = false;

    [Header("AI Audio ----------------------------------------------------------------")]
    public AudioSource audioSource;
    private Animator animator;
    public AudioClip bulletHit;
    public AudioClip[] audioClip;
    public SpawnManager1 enemyCounter;

    public HitBox hitbox;
    public Ragdoll ragdoll;
    public bool stuck = false;
    public float stickySpeed = 0f;
    float nextUpdateTime;

    // Start is called before the first frame update
    public void OnEnable()
    {
        InvokeRepeating("RandomSFX", 15, 20);
        animator = GetComponent<Animator>();
        hitbox.ApplyTagRecursively(gameObject.transform);
        ragdoll.SetUp();

        NavMeshHit closestHit;

        if (NavMesh.SamplePosition(agent.transform.position, out closestHit, 500f, NavMesh.AllAreas))
        {
            agent.enabled = false;
            agent.transform.position = closestHit.position;
            agent.enabled = true;
        }

        if (agent.isOnNavMesh)
        {
            enemyCounter = GameObject.FindGameObjectWithTag("spawnManager").GetComponent<SpawnManager1>();
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);

            Patrol();
        }
    }

    public void FindClosestEnemy()
    {
        //players = FindObjectsOfType<XROrigin>().ToList<>;
        //GameObject closest = null;
        //float distance = Mathf.Infinity;
        //Vector3 position = transform.position;

        //foreach (GameObject go in players)
        //{
        //    Vector3 diff = go.transform.position - position;
        //    float curDistance = diff.sqrMagnitude;
        //    if (curDistance < distance)
        //    {
        //        closest = go;
        //        distance = curDistance;
        //    }
        //}

        targetTransform = FindObjectOfType<XROrigin>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.isOnNavMesh)
        {
            if (Time.time >= nextUpdateTime)
            {
                nextUpdateTime = Time.time + 1f; // Update every 1 second
                FindClosestEnemy();
            }

            if (!alive) return;
            // calculates the distance from NPC to player
            float distanceToPlayer = Vector3.Distance(transform.position, targetTransform.position);
            animator.SetBool("Agro", Agro);

            if (currentState == States.Shocked)
            {
                return;
            }

            // If the player are seen by the NPC or if the distance is close enough
            if (distanceToPlayer <= DetectRange && CheckForPlayer())
            {
                // When Agro changes
                Agro = true;
            }

            // If the enemy has detected the player but doesn't have a clear line of sight
            if (Agro && !IsLineOfSightClear(targetTransform))
            {
                currentState = States.Follow;

                agent.isStopped = false;
                agent.destination = targetTransform.position; // Move towards the player
                return; // Don't proceed to other behaviors until we have a clear line of sight
            }
            // if it is not agro, then patrol like normal
            // else then see if the player is in the valid agro range (bigger than detect range) then give chase
            //          if the player is outside of agro range, drop agro.
            if (!Agro)
            {
                currentState = States.Patrol;
                agent.isStopped = false;
                Patrol();
            }
            else
            {
                // if the NPC is BEYOND the range that it could agro, drop the agro.
                if (distanceToPlayer > AgroRange)
                {
                    // When Agro changes
                    Agro = false;
                    // stop where it is
                    agent.SetDestination(gameObject.transform.position);
                    return;
                }
                // if in range of attacks
                if (distanceToPlayer <= AttackRange)
                {
                    currentState = States.Attack;
                    LookatTarget(1, 3f);
                    agent.isStopped = true;
                    Attack();
                }
                // give chase if not in range
                else
                {
                    currentState = States.Follow;
                    agent.isStopped = false;
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

    private bool CheckForPlayer()
    {
        Transform playerPOS = targetTransform;
        if (playerPOS == null)
            return false;

        if (Vector3.Distance(transform.position, playerPOS.position) > DetectRange)
            return false;

        Vector3 directionToTarget = (playerPOS.position - transform.position).normalized;
        if (Vector3.Angle(transform.forward, directionToTarget) > 110 / 2f)
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

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = UnityEngine.Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }

    private void Patrol()
    {
        attackWeapon.fireWeaponBool = false;
        if (!stuck)
            agent.speed = 1.5f;
        else if (stuck)
            agent.speed = stickySpeed;
        Agro = false;
        // patrols from one place to the next
        if (agent.remainingDistance <= agent.stoppingDistance && PatrolPauseDone)
        {
            PatrolPauseDone = false;
            agent.isStopped = false;
            StartCoroutine(PatrolDelay());
        }
    }

    IEnumerator PatrolDelay()
    {
        attackWeapon.fireWeaponBool = false;
        yield return new WaitForSeconds(3);
        PatrolPauseDone = true;
        Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
        agent.SetDestination(newPos);
        //agent.speed = PatrolPoints.WalkingSpeed;
    }

    private void Follow()
    {
        attackWeapon.fireWeaponBool = false;
        agent.destination = targetTransform.position;
        if (!stuck)
            agent.speed = 2.5f;
        else if (stuck)
            agent.speed = stickySpeed;
    }

    private void Attack()
    {
        transform.LookAt(targetTransform);
        if (IsLineOfSightClear(targetTransform))
            attackWeapon.fireWeaponBool = true;
    }

    private bool IsLineOfSightClear(Transform target)
    {
        Vector3 directionToTarget = target.position - transform.position;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToTarget, out hit, Mathf.Infinity, obstacleMask))
        {
            if (hit.collider != null && hit.collider.gameObject.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
    }

    public void EMPShock()
    {
        IEnumerator shock()
        {
            attackWeapon.fireWeaponBool = false;
            //States preState = currentState;
            currentState = States.Shocked;
            agent.isStopped = true;
            animator.SetTrigger("Shock");
            animator.SetBool("ShockDone", false);

            // apply damage
            yield return new WaitForSeconds(1);
            TakeDamage(5);
            // play shock effect
            GameObject effect = Instantiate(shockEffect, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(1);
            Destroy(effect);
            yield return new WaitForSeconds(1);
            TakeDamage(5);
            // play shock effect
            GameObject effect2 = Instantiate(shockEffect, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(1);
            Destroy(effect2);
            yield return new WaitForSeconds(1);
            TakeDamage(5);
            // play shock effect
            GameObject effect3 = Instantiate(shockEffect, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(1);
            Destroy(effect3);
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

    public void TakeDamage(int damage)
    {
        if (!alive)
            return;
        audioSource.PlayOneShot(bulletHit);
        Health -= damage;
        //healthBar.SetCurrentHealth(Health);

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

    public void RandomSFX()
    {
        if (!audioSource.isPlaying)
            audioSource.PlayOneShot(audioClip[UnityEngine.Random.Range(0, audioClip.Length)]);
    }

    IEnumerator StopHit()
    {
        yield return new WaitForSeconds(1f);
        hitEffect.SetActive(false);
        firstHit = false;
    }
}

