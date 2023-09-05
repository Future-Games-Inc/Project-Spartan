using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using System.Collections;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Threading.Tasks;
using System;

public class FollowAI : MonoBehaviourPunCallbacks, IOnEventCallback
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
    public override void OnEnable()
    {
        base.OnEnable();

        InvokeRepeating("RandomSFX", 15, 20);
        animator = GetComponent<Animator>();
        hitbox.ApplyTagRecursively(gameObject.transform);
        ragdoll.SetUp();

        enemyCounter = GameObject.FindGameObjectWithTag("spawnManager").GetComponent<SpawnManager1>();
        Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
        agent.SetDestination(newPos);

        Patrol();

        //photonView.RPC("RPC_EnemyHealthMax", RpcTarget.All);
        //Listen to PhotonEvents

        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        //Stop listening to PhotonEvents
        PhotonNetwork.RemoveCallbackTarget(this);
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
        if (PhotonNetwork.IsMasterClient)
        {
            if (Time.time >= nextUpdateTime)
            {
                nextUpdateTime = Time.time + 1f; // Update every 1 second
                FindClosestEnemy();
            }

            if (!alive) return;
            // calculates the distance from NPC to player
            float distanceToPlayer = Vector3.Distance(transform.position, targetTransform.position);
            photonView.RPC("RPC_SyncAnimator", RpcTarget.All, "Agro", Agro);

            if (currentState == States.Shocked)
            {
                return;
            }

            // If the player are seen by the NPC or if the distance is close enough
            if (distanceToPlayer <= DetectRange && CheckForPlayer())
            {
                // When Agro changes
                photonView.RPC("RPC_SetAgro", RpcTarget.All, true);
            }

            // If the enemy has detected the player but doesn't have a clear line of sight
            if (Agro && !IsLineOfSightClear(targetTransform))
            {
                photonView.RPC("RPC_SwitchState", RpcTarget.All, States.Follow.ToString());

                agent.isStopped = false;
                agent.destination = targetTransform.position; // Move towards the player
                return; // Don't proceed to other behaviors until we have a clear line of sight
            }
            // if it is not agro, then patrol like normal
            // else then see if the player is in the valid agro range (bigger than detect range) then give chase
            //          if the player is outside of agro range, drop agro.
            if (!Agro)
            {
                photonView.RPC("RPC_SwitchState", RpcTarget.All, States.Patrol.ToString());
                agent.isStopped = false;
                Patrol();
            }
            else
            {
                // if the NPC is BEYOND the range that it could agro, drop the agro.
                if (distanceToPlayer > AgroRange)
                {
                    // When Agro changes
                    photonView.RPC("RPC_SetAgro", RpcTarget.All, false);
                    // stop where it is
                    agent.SetDestination(gameObject.transform.position);
                    return;
                }
                // if in range of attacks
                if (distanceToPlayer <= AttackRange)
                {
                    photonView.RPC("RPC_SwitchState", RpcTarget.All, States.Attack.ToString());
                    LookatTarget(1, 3f);
                    agent.isStopped = true;
                    Attack();
                }
                // give chase if not in range
                else
                {
                    photonView.RPC("RPC_SwitchState", RpcTarget.All, States.Follow.ToString());
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
        photonView.RPC("RPC_SetAgro", RpcTarget.All, false);
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
        if (!PhotonNetwork.IsMasterClient) return;
        IEnumerator shock()
        {
            attackWeapon.fireWeaponBool = false;
            //States preState = currentState;
            currentState = States.Shocked;
            agent.isStopped = true;
            animator.SetTrigger("Shock");
            photonView.RPC("RPC_SyncAnimator", RpcTarget.All, "ShockDone", false);

            // apply damage
            yield return new WaitForSeconds(1);
            TakeDamage(5);
            // play shock effect
            GameObject effect = PhotonNetwork.InstantiateRoomObject(shockEffect.name, transform.position, Quaternion.identity, 0, null);
            yield return new WaitForSeconds(1);
            PhotonNetwork.Destroy(effect);
            yield return new WaitForSeconds(1);
            TakeDamage(5);
            // play shock effect
            GameObject effect2 = PhotonNetwork.InstantiateRoomObject(shockEffect.name, transform.position, Quaternion.identity, 0, null);
            yield return new WaitForSeconds(1);
            PhotonNetwork.Destroy(effect2);
            yield return new WaitForSeconds(1);
            TakeDamage(5);
            // play shock effect
            GameObject effect3 = PhotonNetwork.InstantiateRoomObject(shockEffect.name, transform.position, Quaternion.identity, 0, null);
            yield return new WaitForSeconds(1);
            PhotonNetwork.Destroy(effect3);
            // enable movement

            animator.ResetTrigger("shock");
            photonView.RPC("RPC_SyncAnimator", RpcTarget.All, "ShockDone", true);
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
        photonView.RPC("RPC_TakeDamageEnemy", RpcTarget.All, damage);
    }

    public void RandomSFX()
    {
        if (!audioSource.isPlaying)
            audioSource.PlayOneShot(audioClip[UnityEngine.Random.Range(0, audioClip.Length)]);
    }

    IEnumerator StopHit()
    {
        yield return new WaitForSeconds(1f);
        photonView.RPC("RPC_StopHit", RpcTarget.All);
    }

    // Unwrap damage event and call local Take Damage method
    public void OnEvent(EventData photonEvent)
    {
    }

    [PunRPC]
    void RPC_StopHit()
    {
        hitEffect.SetActive(false);
        firstHit = false;
    }



    // --------- OLD RPC FOR ENEMY TAKING DAMAGE ---------------

    [PunRPC]
    void RPC_TakeDamageEnemy(int damage)
    {
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

    [PunRPC]
    void RPC_SwitchState(string newState)
    {
        currentState = (States)Enum.Parse(typeof(States), newState);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send local health to other clients
            stream.SendNext(Health);
        }
        else
        {
            // Receive health updates from other clients
            Health = (int)stream.ReceiveNext();
        }
    }
    [PunRPC]
    void RPC_SetAgro(bool agro)
    {
        Agro = agro;
    }

    [PunRPC]
    void RPC_SyncAnimator(string paramName, bool state)
    {
        animator.SetBool(paramName, state);
    }


}

