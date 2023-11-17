using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.XR.CoreUtils;
using static DroneHealth;
using System.Collections.Generic;

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
    public float DetectRangeStart;
    public float AttackRangeStart;
    public float AgroRangeStart;
    public float wanderRadius = 500;

    public int Health;

    public LayerMask obstacleMask;

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
    public SpawnManager1 enemyCounter;

    public HitBox hitbox;
    public Ragdoll ragdoll;
    public bool stuck = false;
    public float stickySpeed = 0f;
    float nextUpdateTime;

    public Rigidbody attackProjectile;

    public bool isStealthActive;

    public GameObject gun;
    public float sphereRadius = 0.5f;

    // Start is called before the first frame update
    public void OnEnable()
    {
        DetectRangeStart = DetectRange;
        AttackRangeStart = AttackRange;
        AgroRangeStart = AgroRange;
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

    public void ActivateStealth()
    {
        if (!isStealthActive)
        {
            StartCoroutine(StealthRoutine());
        }
    }

    private IEnumerator StealthRoutine()
    {
        isStealthActive = true;

        // Halve the ranges
        DetectRange /= 4;
        AttackRange /= 4;
        AgroRange /= 4;

        yield return new WaitForSeconds(15f);

        // Reset the ranges
        DetectRange = DetectRangeStart;
        AttackRange = AttackRangeStart;
        AgroRange = AgroRangeStart;

        isStealthActive = false;
    }

    public void FindClosestEnemy()
    {
        GameObject closest = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        // Only one object of XROrigin in the scene, so we use FindObjectOfType
        XROrigin xrOrigin = FindObjectOfType<XROrigin>();

        // If player faction is not match owner, include XROrigin object in comparison
        closest = xrOrigin.gameObject;
        closestDistanceSqr = (xrOrigin.transform.position - currentPosition).sqrMagnitude;

        // Find all GameObjects with the specific components
        ReinforcementAI[] followAIs = FindObjectsOfType<ReinforcementAI>();

        // Check all the GameObjects found to see which is closest
        CheckClosest(followAIs, currentPosition, ref closest, ref closestDistanceSqr);

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
        if (!alive) return;

        if (agent.isOnNavMesh)
        {
            if (Time.time >= nextUpdateTime)
            {
                nextUpdateTime = Time.time + 1f; // Update every 1 second
                FindClosestEnemy();
            }

            // Calculate the distance from NPC to player
            float distanceToPlayer = Vector3.Distance(transform.position, targetTransform.position);
            animator.SetBool("Agro", Agro);

            if (currentState == States.Shocked)
            {
                return;
            }

            // If the player is seen by the NPC or if the distance is close enough
            if (distanceToPlayer <= DetectRange && CheckForPlayer())
            {
                Agro = true;
                animator.SetFloat("Speed", agent.velocity.magnitude * GlobalSpeedManager.SpeedMultiplier);
            }
            else
            {
                Agro = false;
            }

            // If the enemy has detected the player but doesn't have a clear line of sight
            if (Agro && !IsLineOfSightClear(targetTransform))
            {
                currentState = States.Follow;
                agent.isStopped = false;
                agent.destination = targetTransform.position;
                animator.SetFloat("Speed", agent.velocity.magnitude * GlobalSpeedManager.SpeedMultiplier);
            }

            if (!Agro)
            {
                currentState = States.Patrol;
                agent.isStopped = false;
                animator.SetFloat("Speed", agent.velocity.magnitude * GlobalSpeedManager.SpeedMultiplier);
                Patrol();
            }
            else
            {
                // if the NPC is BEYOND the range that it could agro, drop the agro.
                if (distanceToPlayer > AgroRange)
                {
                    Agro = false;
                    attackWeapon.fireWeaponBool = false;
                    agent.SetDestination(gameObject.transform.position);
                    animator.SetFloat("Speed", agent.velocity.magnitude * GlobalSpeedManager.SpeedMultiplier);
                }

                // if in range of attacks
                if (distanceToPlayer <= AttackRange)
                {
                    Agro = true;
                    currentState = States.Attack;
                    LookatTarget(1, 3f);
                    agent.isStopped = true;
                    animator.SetFloat("Speed", agent.velocity.magnitude * GlobalSpeedManager.SpeedMultiplier);
                    Attack();
                }
                // give chase if not in range
                else
                {
                    fireReady = false;
                    currentState = States.Follow;
                    agent.isStopped = false;
                    animator.SetFloat("Speed", agent.velocity.magnitude * GlobalSpeedManager.SpeedMultiplier);
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

            animator.SetFloat("Speed", agent.velocity.magnitude * GlobalSpeedManager.SpeedMultiplier);
        }
    }

    public void LookatTarget(float duration, float RotationSpeed = 0.5f)
    {
        TurnSpeed = RotationSpeed * GlobalSpeedManager.SpeedMultiplier;
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
            agent.speed = 1.5f * GlobalSpeedManager.SpeedMultiplier;
        else if (stuck)
            agent.speed = stickySpeed * GlobalSpeedManager.SpeedMultiplier;
        Agro = false;
        // patrols from one place to the next

        if (CheckForPlayer())
        {
            StopCoroutine(PatrolDelay());
            return;
        }

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
        attackWeapon.fireWeaponBool = false;
        fireReady = false;
        agent.destination = targetTransform.position;
        if (!stuck)
            agent.speed = 2.5f * GlobalSpeedManager.SpeedMultiplier;
        else if (stuck)
            agent.speed = stickySpeed * GlobalSpeedManager.SpeedMultiplier;
    }

    private void Attack()
    {
        if (agent.isStopped)
            fireReady = true;
        else if (!agent.isStopped)
            fireReady = false;

        transform.LookAt(targetTransform);
        if (IsLineOfSightClear(targetTransform))
        {
            gun.SetActive(true);
            attackWeapon.fireWeaponBool = true;
        }
        // else logic is handled by the Update method now.
    }

    private bool IsLineOfSightClear(Transform target)
    {
        Vector3 directionToTarget = target.position - transform.position;

        RaycastHit hit;
        if (Physics.SphereCast(transform.position, sphereRadius, directionToTarget, out hit, AttackRange, obstacleMask))
        {
            // Debugging line
            if (hit.collider != null)
            {
                // More debugging
                if (hit.collider.gameObject.CompareTag("Player") || hit.collider.gameObject.CompareTag("ReactorInteractor") || hit.collider.gameObject.CompareTag("Reinforcements")
                    || hit.collider.gameObject.CompareTag("Bullet") || hit.collider.gameObject.CompareTag("RightHand") || hit.collider.gameObject.CompareTag("LeftHand")
                    || hit.collider.gameObject.CompareTag("RHand") || hit.collider.gameObject.CompareTag("LHand") || hit.collider.gameObject.CompareTag("EnemyBullet")
                    || hit.collider.gameObject.CompareTag("PickupSlot") || hit.collider.gameObject.CompareTag("PickupStorage") || hit.collider.gameObject.CompareTag("toxicRadius")
                    || hit.collider.gameObject.CompareTag("Untagged"))
                {
                    if (hit.collider.gameObject.GetComponentInParent<PlayerHealth>() != null)
                        return true;
                }
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

