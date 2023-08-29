using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using System.Collections;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Threading.Tasks;

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

    // Start is called before the first frame update
    public override void OnEnable()
    {
        base.OnEnable();
        if (PhotonNetwork.IsMasterClient)
        {
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
        }
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

    public void SwitchStates(States input)
    {
        previousState = currentState;

        currentState = input;
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            FindClosestEnemy();

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
                Agro = true;
            }

            // If the enemy has detected the player but doesn't have a clear line of sight
            if (Agro && !IsLineOfSightClear(targetTransform))
            {
                SwitchStates(States.Follow);
                agent.isStopped = false;
                agent.destination = targetTransform.position; // Move towards the player
                return; // Don't proceed to other behaviors until we have a clear line of sight
            }
            // if it is not agro, then patrol like normal
            // else then see if the player is in the valid agro range (bigger than detect range) then give chase
            //          if the player is outside of agro range, drop agro.
            if (!Agro)
            {
                SwitchStates(States.Patrol);
                agent.isStopped = false;
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
                if (distanceToPlayer <= AttackRange)
                {
                    SwitchStates(States.Attack);
                    LookatTarget(1, 3f);
                    agent.isStopped = true;
                    Attack();
                }
                // give chase if not in range
                else
                {
                    SwitchStates(States.Follow);
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

        if (agent != null && !agent.isOnNavMesh)
        {
            TakeDamage(300);
            enemyCounter.enemiesKilled--;
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
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }

    private void Patrol()
    {
        attackWeapon.fireWeaponBool = false;
        agent.speed = 1.5f;
        Agro = false;
        // patrols from one place to the next
        if (agent.remainingDistance <= agent.stoppingDistance && PatrolPauseDone)
        {
            PatrolPauseDone = false;
            agent.isStopped = false;
            PatrolDelay();
        }
    }

    private async void PatrolDelay()
    {
        attackWeapon.fireWeaponBool = false;
        await Task.Delay(3000);
        PatrolPauseDone = true;
        Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
        agent.SetDestination(newPos);
        // agent.speed = PatrolPoints.WalkingSpeed;
    }

    private void Follow()
    {
        attackWeapon.fireWeaponBool = false;
        PatrolDelay();
        agent.destination = targetTransform.position;
        agent.speed = 2.5f;
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
        async void shock()
        {
            attackWeapon.fireWeaponBool = false;
            //States preState = currentState;
            currentState = States.Shocked;
            agent.isStopped = true;
            animator.SetTrigger("Shock");
            animator.SetBool("ShockDone", false);

            // apply damage
            await Task.Delay(1000);
            TakeDamage(5);
            // play shock effect
            GameObject effect = PhotonNetwork.InstantiateRoomObject(shockEffect.name, transform.position, Quaternion.identity, 0, null);
            await Task.Delay(1000);
            PhotonNetwork.Destroy(effect);
            await Task.Delay(1000);
            TakeDamage(5);
            // play shock effect
            GameObject effect2 = PhotonNetwork.InstantiateRoomObject(shockEffect.name, transform.position, Quaternion.identity, 0, null);
            await Task.Delay(1000);
            PhotonNetwork.Destroy(effect2);
            await Task.Delay(1000);
            TakeDamage(5);
            // play shock effect
            GameObject effect3 = PhotonNetwork.InstantiateRoomObject(shockEffect.name, transform.position, Quaternion.identity, 0, null);
            await Task.Delay(1000);
            PhotonNetwork.Destroy(effect3);
            // enable movement

            animator.ResetTrigger("shock");
            animator.SetBool("ShockDone", true);
            currentState = previousState;
            agent.isStopped = false;
        }
        // if already shocked, ignore effects
        if (currentState == States.Shocked) return;
        shock();
    }

    public void TakeDamage(int damage)
    {
        if (!alive)
            return;
        //photonView.RPC("RPC_TakeDamageEnemy", RpcTarget.All, damage);

        // Raise take damage event
        object[] data = new object[] { damage };
        RaiseEventOptions options = new RaiseEventOptions() { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent((byte)PUNEventDatabase.FollowAI_TakeDamage, data, options, SendOptions.SendUnreliable);
    }

    public void RandomSFX()
    {
        if (!audioSource.isPlaying)
            audioSource.PlayOneShot(audioClip[Random.Range(0, audioClip.Length)]);
    }

    void StopHit()
    {
        // photonView.RPC("RPC_StopHit", RpcTarget.All);
        // Raise Event Here
        hitEffect.SetActive(false);
        firstHit = false;
    }

    public void EMPShock(GameObject Effect)
    {
        async void Shock()
        {
            attackWeapon.fireWeaponBool = false;
            //States preState = currentState;
            SwitchStates(States.Shocked);
            agent.isStopped = true;
            animator.SetTrigger("Shock");
            animator.SetBool("ShockDone", false);
            Destroy(Instantiate(Effect, gameObject.transform.position + new Vector3(0, 1, 0), Quaternion.identity), 1f);
            // apply damage
            await Task.Delay(1000);
            TakeDamage(5);
            // play shock effect
            Destroy(Instantiate(Effect, gameObject.transform.position + new Vector3(0, 1, 0), Quaternion.identity), 1f);
            agent.isStopped = true;
            await Task.Delay(1000);
            TakeDamage(5);
            // play shock effect
            Destroy(Instantiate(Effect, gameObject.transform.position + new Vector3(0, 1, 0), Quaternion.identity), 1f);
            agent.isStopped = true;
            await Task.Delay(1000);
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
        Shock();
    }

    //[PunRPC]
    //void RPC_EnemyHealthMax()
    //{
    //    healthBar.SetMaxHealth(Health);
    //}

    // Unwrap damage event and call local Take Damage method
    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (byte)PUNEventDatabase.FollowAI_TakeDamage)
        {
            object[] data = (object[])photonEvent.CustomData;
            int damage = (int)data[0];
            TakeDamageEnemy(damage);
        }
        if (photonEvent.Code == (byte)PUNEventDatabase.FollowAI_StopHit)
        {
            // Call StopHit after 3 seconds
            Invoke("StopHit", 3f);
        }
    }

    // Damages the enemy and kills it if health <= 0
    void TakeDamageEnemy(int damage)
    {
        audioSource.PlayOneShot(bulletHit);
        Health -= damage;
        //healthBar.SetCurrentHealth(Health);

        if (Health <= 0 && alive == true)
        {
            alive = false;
            enemyHealth.KillEnemy();

            // Updates Enemy counts
            RaiseEventOptions options = new RaiseEventOptions() { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent((byte)PUNEventDatabase.SpawnManager1_UpdateEnemyCount, null, options, SendOptions.SendUnreliable);
        }

        if (!firstHit)
        {
            hitEffect.SetActive(true);
            firstHit = true;
            // Invoke StopHit after 3 seconds so no need for Coroutine
            //Invoke("StopHit", 3f);
            RaiseEventOptions options = new RaiseEventOptions() { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent((byte)PUNEventDatabase.FollowAI_StopHit, null, options, SendOptions.SendUnreliable);
        }
    }

    // --------- OLD RPC FOR StopHit ---------------

    //[PunRPC]
    //void RPC_StopHit()
    //{
    //    hitEffect.SetActive(false);
    //    firstHit = false;
    //}



    // --------- OLD RPC FOR ENEMY TAKING DAMAGE ---------------

    //[PunRPC]
    //void RPC_TakeDamageEnemy(int damage)
    //{
    //    audioSource.PlayOneShot(bulletHit);
    //    Health -= damage;
    //    //healthBar.SetCurrentHealth(Health);

    //    if (Health <= 0 && alive == true)
    //    {
    //        alive = false;
    //        enemyHealth.KillEnemy();
    //    }

    //    if (!firstHit)
    //    {
    //        hitEffect.SetActive(true);
    //        firstHit = true;
    //        StartCoroutine(StopHit());
    //    }
    //}


}

