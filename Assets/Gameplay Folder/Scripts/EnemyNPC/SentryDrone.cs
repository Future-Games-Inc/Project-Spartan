using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;
using static DroneHealth;

public class SentryDrone : MonoBehaviour
{
    public float DetectRange = 20;
    public float AttackRange = 15;
    public float AgroRange = 25;

    public States currentState;
    private States previousState;

    public bool patrolling;

    public float wanderRadius;
    public float wanderTimer;

    public float timer;

    private float DistanceToPlayer;

    public int Health = 100;
    public GameObject xpDrop;
    public GameObject xpDropExtra;
    public SpawnManager1 enemyCounter;
    public bool alive = true;
    public Transform[] lootSpawn;
    public float xpDropRate;

    public AudioSource audioSource;
    public AudioClip bulletHit;

    public GameObject explosionEffect;
    //public EnemyHealthBar healthBar;
    public AudioClip[] audioClip;
    public AudioClip reloadClip;
    public NavMeshAgent agent;

    private GameObject[] players;
    public Transform targetTransform;

    private float TurnSpeed = 5f;
    private bool isLookingAtPlayer = false;
    bool isFiring;

    public GameObject bullet;
    public Transform bulletTransform;
    public float shootForce;

    public float ammoLeft;
    public float maxAmmo = 10f;

    public float nextUpdateTime;

    public LayerMask obstacleMask;

    public int bulletModifier = 2;

    public bool hit;


    public enum States
    {
        Patrol,
        Follow,
        Attack
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        NavMeshHit closestHit;

        if (NavMesh.SamplePosition(agent.transform.position, out closestHit, 500f, NavMesh.AllAreas))
        {
            agent.enabled = false;
            agent.transform.position = closestHit.position;
            agent.enabled = true;
        }



        enemyCounter = GameObject.FindGameObjectWithTag("spawnManager").GetComponent<SpawnManager1>();
        InvokeRepeating("RandomSFX", 15, 20f);
        explosionEffect.SetActive(false);
        //healthBar.SetMaxHealth(Health);
        alive = true;
        timer = wanderTimer;
        StartCoroutine(Fire());
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

    public void SwitchStates(States input)
    {
        previousState = currentState;

        currentState = input;
    }

    public void Patrol()
    {
        patrolling = true;
        isFiring = false;
        agent.speed = 1f * GlobalSpeedManager.SpeedMultiplier;
        if (timer >= wanderTimer && agent.enabled == true)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
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
        if (agent.isOnNavMesh)
        {
            if (Time.time >= nextUpdateTime)
            {
                nextUpdateTime = Time.time + 1f; // Update every 1 second
                FindClosestEnemy();
            }

            float distanceToPlayer = Vector3.Distance(transform.position, targetTransform.position);

            if (patrolling)
                timer += Time.deltaTime;

            if (distanceToPlayer <= AttackRange)
            {
                SwitchStates(States.Attack);
                LookatTarget(1, 3f);
                agent.isStopped = true;
                Attack();
            }
            // give chase if not in range
            else if (distanceToPlayer > AttackRange && distanceToPlayer < AgroRange)
            {
                SwitchStates(States.Follow);
                agent.isStopped = false;
                Follow();
            }
            else
            {
                if (previousState != States.Patrol)
                {
                    Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                    agent.SetDestination(newPos);
                }
                SwitchStates(States.Patrol);
                agent.isStopped = false;
                Patrol();
            }

            if (isLookingAtPlayer)
            {
                Vector3 direction = targetTransform.position - transform.position;
                direction.y = 0;
                Quaternion desiredRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * TurnSpeed);
            }
        }
    }
    private bool CheckForPlayer()
    {
        Transform playerPOS = targetTransform;
        if (playerPOS == null)
            return false;

        if (Vector3.Distance(transform.position, playerPOS.position) > AttackRange)
            return false;

        Vector3 directionToTarget = (playerPOS.position - transform.position).normalized;
        if (Vector3.Angle(transform.forward, directionToTarget) > 110 / 2f)
            return false;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToTarget, out hit, AttackRange, obstacleMask))
        {
            // Debugging line
            if (hit.collider != null)
            {
                // More debugging
                if (hit.collider.gameObject.CompareTag("Player") || hit.collider.gameObject.CompareTag("ReactorInteractor"))
                {
                    return true;
                }
            }
        }

        return false;
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

    private void Follow()
    {
        patrolling = false;
        isFiring = false;
        agent.destination = targetTransform.position;
        agent.speed = 2.5f * GlobalSpeedManager.SpeedMultiplier;
    }
    private void Attack()
    {
        patrolling = false;
        transform.LookAt(targetTransform);
        isFiring = true;
    }

    IEnumerator Fire()
    {
        while (gameObject.activeSelf)
        {
            if (isFiring)
            {
                if (isFiring && ammoLeft <= 0)
                {
                    isFiring = false;
                    StartCoroutine(ReloadWeapon());
                }
                else if (CheckForPlayer())
                {
                    yield return new WaitForSeconds(0.25f);
                    GameObject spawnedBullet = Instantiate(bullet, bulletTransform.position, Quaternion.identity);
                    spawnedBullet.GetComponent<Rigidbody>().velocity = bulletTransform.forward * shootForce * GlobalSpeedManager.SpeedMultiplier;
                    ammoLeft--;
                }
            }
            yield return new WaitForSeconds(3f);
        }
    }

    IEnumerator ReloadWeapon()
    {
        audioSource.PlayOneShot(reloadClip);
        yield return new WaitForSeconds(2f);
        ammoLeft = maxAmmo;
        isFiring = true;
    }

    public void TakeDamage(int damage)
    {
        audioSource.PlayOneShot(bulletHit);
        Health -= damage;
        if (!hit)
            StartCoroutine(Hit());

        if (Health <= 0 && alive == true)
        {
            alive = false;
            enemyCounter.UpdateSecurity();

            explosionEffect.SetActive(true);
            explosionEffect.GetComponentInChildren<ParticleSystem>().Play();

            agent.enabled = false;
            GetComponent<Rigidbody>().isKinematic = false;

            DestroyEnemy();
        }
    }

    IEnumerator Hit()
    {
        hit = true;
        explosionEffect.SetActive(true);
        yield return new WaitForSeconds(1);
        explosionEffect.SetActive(false);
        hit = false;
    }

    private void DestroyEnemy()
    {
        foreach (Transform t in lootSpawn)
        {
            xpDropRate = 10f;
            if (Random.Range(0, 100f) < xpDropRate)
            {
                GameObject DropExtra = Instantiate(xpDropExtra, t.position, Quaternion.identity);
                DropExtra.GetComponent<Rigidbody>().isKinematic = false;
            }
            else
            {
                GameObject DropNormal = Instantiate(xpDrop, t.position, Quaternion.identity);
                DropNormal.GetComponent<Rigidbody>().isKinematic = false;
            }
        }

        //yield return new WaitForSeconds(.75f);
        Destroy(gameObject);
    }

    public void RandomSFX()
    {
        if (!audioSource.isPlaying)
            audioSource.PlayOneShot(audioClip[Random.Range(0, audioClip.Length)]);
    }
}

