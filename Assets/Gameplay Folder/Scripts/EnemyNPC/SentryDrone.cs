using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class SentryDrone : MonoBehaviourPunCallbacks, IOnEventCallback
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

    public LayerMask obstacleMask;

    public enum States
    {
        Patrol,
        Follow,
        Attack
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        enemyCounter = GameObject.FindGameObjectWithTag("spawnManager").GetComponent<SpawnManager1>();
        InvokeRepeating("RandomSFX", 15, 20f);
        explosionEffect.SetActive(false);
        //healthBar.SetMaxHealth(Health);
        alive = true;
        timer = wanderTimer;
        StartCoroutine(Fire());
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

    }

    public override void OnDisable()
    {
        base.OnDisable();
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }
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

    public void SwitchStates(States input)
    {
        previousState = currentState;

        currentState = input;
    }

    public void Patrol()
    {
        patrolling = true;
        isFiring = false;
        agent.speed = 1f;
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
        if (PhotonNetwork.IsMasterClient)
        {
            FindClosestEnemy();

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
                if(previousState != States.Patrol)
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

        if (agent != null && !agent.isOnNavMesh)
        {
            TakeDamage(300);
        }
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

    private void Follow()
    {
        patrolling = false;
        isFiring = false;
        agent.destination = targetTransform.position;
        agent.speed = 2.5f;
    }
    private void Attack()
    {
        patrolling = false;
        transform.LookAt(targetTransform);
        isFiring = true;
    }

    IEnumerator Fire()
    {
        while (true)
        {
            if (isFiring && ammoLeft > 0 && IsLineOfSightClear(targetTransform))
            {
                yield return new WaitForSeconds(0.25f);
                GameObject spawnedBullet = PhotonNetwork.InstantiateRoomObject(bullet.name, bulletTransform.position, Quaternion.identity, 0, null);
                spawnedBullet.GetComponent<Bullet>().audioSource.PlayOneShot(spawnedBullet.GetComponent<Bullet>().clip);
                spawnedBullet.GetComponent<Bullet>().bulletModifier = 4;
                shootForce = (int)Random.Range(40, 75);
                spawnedBullet.GetComponent<Rigidbody>().velocity = bulletTransform.forward * shootForce;

                ammoLeft--;
                yield return new WaitForSeconds(0.25f);
            }

            else if (isFiring && ammoLeft <= 0)
            {
                isFiring = false;
                StartCoroutine(ReloadWeapon());
            }

            yield return new WaitForSeconds(0.25f);
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
        //photonView.RPC("RPC_TakeDamage", RpcTarget.All, damage);
        audioSource.PlayOneShot(bulletHit);
        Health -= damage;
        //healthBar.SetCurrentHealth(Health);

        if (Health <= 0 && alive == true)
        {
            alive = false;
            enemyCounter.photonView.RPC("RPC_UpdateSecurity", RpcTarget.All);

            explosionEffect.SetActive(true);
            explosionEffect.GetComponentInChildren<ParticleSystem>().Play();

            agent.enabled = false;
            GetComponent<Rigidbody>().isKinematic = false;

            DestroyEnemy();
        }
    }

    private void DestroyEnemy()
    {
        foreach (Transform t in lootSpawn)
        {
            xpDropRate = 10f;
            if (Random.Range(0, 100f) < xpDropRate)
            {
                GameObject DropExtra = PhotonNetwork.InstantiateRoomObject(xpDropExtra.name, t.position, Quaternion.identity, 0);
                DropExtra.GetComponent<Rigidbody>().isKinematic = false;
            }
            else
            {
                GameObject DropNormal = PhotonNetwork.InstantiateRoomObject(xpDrop.name, t.position, Quaternion.identity, 0);
                DropNormal.GetComponent<Rigidbody>().isKinematic = false;
            }
        }

       //yield return new WaitForSeconds(.75f);
        PhotonNetwork.Destroy(gameObject);
    }

    public void RandomSFX()
    {
        if (!audioSource.isPlaying)
            audioSource.PlayOneShot(audioClip[Random.Range(0, audioClip.Length)]);
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (byte)PUNEventDatabase.SentryDrone_TakeDamage)
        {
            object[] data = (object[])photonEvent.CustomData;
            int damage = (int)data[0];
            TakeDamage(damage);
        }
    }

    //[PunRPC]
    //void RPC_TakeDamage(int damage)
    //{
    //    audioSource.PlayOneShot(bulletHit);
    //    Health -= damage;
    //    //healthBar.SetCurrentHealth(Health);

    //    if (Health <= 0 && alive == true)
    //    {
    //        alive = false;
    //        enemyCounter.photonView.RPC("RPC_UpdateSecurity", RpcTarget.All);

    //        explosionEffect.SetActive(true);

    //        agent.enabled = false;
    //        GetComponent<Rigidbody>().isKinematic = false;

    //        DestroyEnemy();
    //    }
    //}
}

