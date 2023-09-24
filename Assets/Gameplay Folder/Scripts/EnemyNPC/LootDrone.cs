using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static DroneHealth;

public class LootDrone : MonoBehaviour
{
    public float DetectRange = 20;
    public float LootRange = 3;
    public float DeactiveRange = 5;

    public float wanderRadius;
    public float wanderTimer;

    public float timer;

    private float DistanceToPlayer;

    public States currentState;
    private States previousState;

    private GameObject[] caches;
    public Transform targetTransform;

    private float TurnSpeed = 5f;
    private bool isLookingAtPlayer = false;

    public NavMeshAgent agent;

    public bool patrolling = true;
    public bool isLooting;
    public bool positionSet;

    public GameObject attachedCache;
    public Transform attachTransform;
    public float nextUpdateTime;

    public enum States
    {
        Patrol,
        Loot
    }
    // Start is called before the first frame update
    public void FindClosestEnemy()
    {
        caches = GameObject.FindGameObjectsWithTag("Cache");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;

        foreach (GameObject go in caches)
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

    public void SwitchStates(States input)
    {
        previousState = currentState;

        currentState = input;
    }

    public void Patrol()
    {
        if (!positionSet)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
        }
        positionSet = true;
        patrolling = true;
        isLooting = false;
        agent.speed = 1f * GlobalSpeedManager.SpeedMultiplier;
        if (timer >= wanderTimer)
        {
            patrolling = false;
            isLooting = true;
            positionSet = false;
            agent.SetDestination(targetTransform.position);
            targetTransform.GetComponentInParent<WeaponCrate>().Obstacle(false);
            timer = 0;
        }
    }

    public void Loot()
    {
        agent.speed = 2f * GlobalSpeedManager.SpeedMultiplier;
        if (agent.remainingDistance <= LootRange)
        {
            if (attachedCache == null)
            {
                // Attach the cache to the drone
                attachedCache = targetTransform.gameObject;
                attachedCache.transform.parent = attachTransform;
                attachedCache.transform.localPosition = Vector3.zero;
                agent.isStopped = true;


                // Move the drone to a new location within the randomNavSphere
                Vector3 newPosition = RandomNavSphere(transform.position, wanderRadius, -1);
                agent.SetDestination(newPosition);
                StartCoroutine(PauseDelay());
            }
            else if (agent.remainingDistance <= agent.stoppingDistance + 10 && !agent.pathPending)
            {
                // Drop the attached cache back on the map
                attachedCache.transform.parent = null;
                targetTransform.GetComponentInParent<WeaponCrate>().Obstacle(true);

                // Reset variables for the next loot phase
                attachedCache = null;
                isLooting = false;
                patrolling = true;
            }
        }
    }

    IEnumerator PauseDelay()
    {
        yield return new WaitForSeconds(2);
        agent.isStopped = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet") || other.CompareTag("EnemyBullet"))
        {
            if (attachedCache != null)
            {
                attachedCache.transform.parent = null;
                targetTransform.GetComponentInParent<WeaponCrate>().Obstacle(true);
                isLooting = false;
                patrolling = true;
            }
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

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextUpdateTime)
        {
            nextUpdateTime = Time.time + 1f; // Update every 1 second
            FindClosestEnemy();
        }

        float distanceToPlayer = Vector3.Distance(transform.position, targetTransform.position);

        if (patrolling)
        {
            SwitchStates(States.Patrol);
            timer += Time.deltaTime;
            Patrol();
        }

        if (isLooting)
        {
            SwitchStates(States.Loot);
            if (attachedCache == null)
                LookatTarget(1, 3f);
            Loot();
        }

        // Check if the loot is completed
        if (attachedCache != null && agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            // Drop the attached cache back on the map
            attachedCache.transform.parent = null;

            // Reset variables for the next loot phase
            attachedCache = null;
            isLooting = false;
            patrolling = true;
        }

        if (isLookingAtPlayer)
        {
            Vector3 direction = targetTransform.position - transform.position;
            direction.y = 0;
            Quaternion desiredRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * TurnSpeed);
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
}
