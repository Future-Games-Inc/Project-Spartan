using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using System.Collections;

public class FollowAI : MonoBehaviourPunCallbacks
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

    public int currentWaypoint;
    public Transform[] PatrolPoints;
    private int currentPatrolPointIndex = 0;
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

    // Start is called before the first frame update
    void OnEnable()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            InvokeRepeating("RandomSFX", 15, 20);
            animator = GetComponent<Animator>();
            GameObject parentObject = GameObject.FindGameObjectWithTag("Waypoints");
            if (parentObject != null)
            {
                // Get the child transforms of the parent object
                PatrolPoints = parentObject.GetComponentsInChildren<Transform>();
            }

            Patrol();

            //photonView.RPC("RPC_EnemyHealthMax", RpcTarget.All);
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

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
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
            if (distanceToPlayer <= DetectRange && CheckForPlayer())
            {
                Agro = true;
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
            StartCoroutine(PatrolDelay());
        }
    }

    private IEnumerator PatrolDelay()
    {
        attackWeapon.fireWeaponBool = false;
        yield return new WaitForSeconds(3f);
        PatrolPauseDone = true;
        currentPatrolPointIndex = (currentPatrolPointIndex + 1) % PatrolPoints.Length;
        agent.destination = PatrolPoints[currentPatrolPointIndex].position;
        // agent.speed = PatrolPoints.WalkingSpeed;
    }

    private void Follow()
    {
        attackWeapon.fireWeaponBool = false;
        StopCoroutine(PatrolDelay());
        agent.destination = targetTransform.position;
        agent.speed = 2.5f;
    }

    private void Attack()
    {
        transform.LookAt(targetTransform);
        attackWeapon.fireWeaponBool = true;
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
            animator.SetBool("ShockDone", true);
            currentState = previousState;
            agent.isStopped = false;
        }
        // if already shocked, ignore effects
        if (currentState == States.Shocked) return;
        StartCoroutine(shock());
    }

    private void LookAtTarget()
    {
        Vector3 lookDirection = directionToTarget;
        lookDirection.y = 0f;

        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);

        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * agent.angularSpeed);
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
            audioSource.PlayOneShot(audioClip[Random.Range(0, audioClip.Length)]);
    }

    IEnumerator StopHit()
    {
        yield return new WaitForSeconds(3f);
        photonView.RPC("RPC_StopHit", RpcTarget.All);
    }

    public void EMPShock(GameObject Effect)
    {
        IEnumerator shock()
        {
            attackWeapon.fireWeaponBool = false;
            //States preState = currentState;
            SwitchStates(States.Shocked);
            agent.isStopped = true;
            animator.SetTrigger("Shock");
            animator.SetBool("ShockDone", false);
            Destroy(Instantiate(Effect, gameObject.transform.position + new Vector3(0, 1, 0), Quaternion.identity), 1f);
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

    //[PunRPC]
    //void RPC_EnemyHealthMax()
    //{
    //    healthBar.SetMaxHealth(Health);
    //}

    [PunRPC]
    void RPC_StopHit()
    {
        hitEffect.SetActive(false);
        firstHit = false;
    }

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
}

