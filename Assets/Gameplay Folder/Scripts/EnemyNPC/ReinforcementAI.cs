using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.XR.CoreUtils;

public class ReinforcementAI : MonoBehaviour
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

    public bool fireReady = false;

    public NavMeshAgent agent;
    public Transform targetTransform;
    //public EnemyHealthBar healthBar;
    public ReinforcementHealth enemyHealth;
    public ReinforcementWeapon attackWeapon;
    public GameObject hitEffect;
    public GameObject shockEffect;

    private float TurnSpeed = 5f;
    private bool isLookingAtPlayer = false;

    [Header("AI Behavior --------------------------------------------------------------")]
    public bool inSight;
    public bool alive = true;
    private bool firstHit = false;

    private bool PatrolPauseDone = true;

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

    public HitBox hitbox;
    public Ragdoll ragdoll;
    public bool stuck = false;
    public float stickySpeed = 0f;
    float nextUpdateTime;

    public PlayerHealth player;
    public MatchEffects match;

    // Start is called before the first frame update
    public void OnEnable()
    {
        InvokeRepeating("RandomSFX", 15, 20);
        animator = GetComponent<Animator>();
        hitbox.ApplyTagRecursively(gameObject.transform);
        ragdoll.SetUp();

        match = GameObject.FindGameObjectWithTag("Props").GetComponentInParent<MatchEffects>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInParent<PlayerHealth>();

        NavMeshHit closestHit;

        if (NavMesh.SamplePosition(agent.transform.position, out closestHit, 500f, NavMesh.AllAreas))
        {
            agent.enabled = false;
            agent.transform.position = closestHit.position;
            agent.enabled = true;
        }

        if (agent.isOnNavMesh)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);

            Patrol();
        }
    }

    public void FindClosestEnemy()
    {
        GameObject closest = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        // Only one object of XROrigin in the scene, so we use FindObjectOfType
        XROrigin xrOrigin = FindObjectOfType<XROrigin>();

        // If player faction is not match owner, include XROrigin object in comparison
        if (player.faction != match.owner && xrOrigin != null)
        {
            closest = xrOrigin.gameObject;
            closestDistanceSqr = (xrOrigin.transform.position - currentPosition).sqrMagnitude;
        }

        // Find all GameObjects with the specific components
        FollowAI[] followAIs = FindObjectsOfType<FollowAI>();
        DroneHealth[] droneHealths = FindObjectsOfType<DroneHealth>();
        SentryDrone[] sentryDrones = FindObjectsOfType<SentryDrone>();

        // Check all the GameObjects found to see which is closest
        CheckClosest(followAIs, currentPosition, ref closest, ref closestDistanceSqr);
        CheckClosest(droneHealths, currentPosition, ref closest, ref closestDistanceSqr);
        CheckClosest(sentryDrones, currentPosition, ref closest, ref closestDistanceSqr);

        // Set the targetTransform to the transform of the closest object found.
        if (closest != null)
        {
            targetTransform = closest.transform;
        }
    }

    private void CheckClosest<T>(T[] gameObjects, Vector3 currentPosition, ref GameObject closest, ref float closestDistanceSqr) where T : Component
    {
        foreach (T obj in gameObjects)
        {
            GameObject go = obj.gameObject;
            float distanceSqr = (go.transform.position - currentPosition).sqrMagnitude;
            if (distanceSqr < closestDistanceSqr)
            {
                closest = go;
                closestDistanceSqr = distanceSqr;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").GetComponentInParent<PlayerHealth>();

        if (!alive) return;

        if (agent.isOnNavMesh)
        {
            transform.LookAt(agent.destination);
            if (Time.time >= nextUpdateTime)
            {
                nextUpdateTime = Time.time + 1f; // Update every 1 second
                FindClosestEnemy();
            }

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
                // set the speed for the agent for the blend tree
                animator.SetFloat("Speed", agent.velocity.magnitude);
            }
            else
                Agro = false;

            // If the enemy has detected the player but doesn't have a clear line of sight
            if (Agro && !IsLineOfSightClear(targetTransform))
            {
                currentState = States.Follow;
                fireReady = false;
                agent.isStopped = false;
                agent.destination = targetTransform.position; // Move towards the player
                                                              // set the speed for the agent for the blend tree
                animator.SetFloat("Speed", agent.velocity.magnitude);
            }
            // if it is not agro, then patrol like normal
            // else then see if the player is in the valid agro range (bigger than detect range) then give chase
            //          if the player is outside of agro range, drop agro.
            if (!Agro)
            {
                currentState = States.Patrol;
                agent.isStopped = false;
                // set the speed for the agent for the blend tree
                animator.SetFloat("Speed", agent.velocity.magnitude);
                Patrol();
            }
            else
            {
                // if the NPC is BEYOND the range that it could agro, drop the agro.
                if (distanceToPlayer > AgroRange)
                {
                    // When Agro changes
                    Agro = false;
                    attackWeapon.fireWeaponBool = false;
                    // stop where it is
                    agent.SetDestination(gameObject.transform.position);
                    // set the speed for the agent for the blend tree
                    animator.SetFloat("Speed", agent.velocity.magnitude);
                }
                // if in range of attacks
                if (distanceToPlayer <= AttackRange)
                {
                    Agro = true;
                    currentState = States.Attack;
                    LookatTarget(1, 3f);
                    agent.isStopped = true;
                    // set the speed for the agent for the blend tree
                    animator.SetFloat("Speed", agent.velocity.magnitude);
                    Attack();
                }
                // give chase if not in range
                else
                {
                    fireReady = false;
                    currentState = States.Follow;
                    agent.isStopped = false;
                    // set the speed for the agent for the blend tree
                    animator.SetFloat("Speed", agent.velocity.magnitude);
                    Follow();
                }
            }

            if (CheckForPlayer())
            {
                LookatTarget(1f, 3f);  // make the enemy turn towards the player
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

        return Vector3.Distance(transform.position, playerPOS.position) <= DetectRange;
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
        fireReady = false;
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
        fireReady = false;
        attackWeapon.fireWeaponBool = false;
        yield return new WaitForSeconds(3);
        PatrolPauseDone = true;
        Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
        agent.SetDestination(newPos);
        //agent.speed = PatrolPoints.WalkingSpeed;
    }

    private void Follow()
    {
        fireReady = false;
        attackWeapon.fireWeaponBool = false;
        agent.destination = targetTransform.position;
        if (!stuck)
            agent.speed = 2.5f;
        else if (stuck)
            agent.speed = stickySpeed;
    }

    private void Attack()
    {
        if (agent.isStopped)
            fireReady = true;
        else if(!agent.isStopped)
            fireReady = false;
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
            if (hit.collider.gameObject.CompareTag("Player") || hit.collider.gameObject.CompareTag("ReactorInteractor") || hit.collider.gameObject.CompareTag("Enemy")
                || hit.collider.gameObject.CompareTag("Security") || hit.collider.gameObject.CompareTag("BossEnemy") || hit.collider.gameObject.CompareTag("EnemyBullet")
                || hit.collider.gameObject.CompareTag("RightHand") || hit.collider.gameObject.CompareTag("LeftHand") || hit.collider.gameObject.CompareTag("RHand") || hit.collider.gameObject.CompareTag("LHand") || hit.collider.gameObject.CompareTag("Bullet"))
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

