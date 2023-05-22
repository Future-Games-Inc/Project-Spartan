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
    public float maxFollowDistance = 20f;
    public float shootDistance = 10f;
    public int Health;
    public NavMeshAgent agent;
    public Transform targetTransform;
    //public EnemyHealthBar healthBar;
    public EnemyHealth enemyHealth;
    public AIWeapon attackWeapon;
    public GameObject hitEffect;

    [Header("AI Behavior --------------------------------------------------------------")]
    private Vector3 directionToTarget;
    public States currentState = States.Patrol;
    public bool inSight;
    public bool alive = true;
    private bool firstHit = false;

    public Transform[] waypoints;
    private GameObject[] players;

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

            waypoints = GameObject.FindGameObjectWithTag("Waypoints").GetComponentsInChildren<Transform>();

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

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (currentState != States.Shocked)
            {
                FindClosestEnemy();
                CheckForPlayer();
                UpdateStates();

            }
            else if (currentState == States.Shocked)
            {

                agent.isStopped = true;
                agent.SetDestination(gameObject.transform.position);
            }
        }
    }

    private void UpdateStates()
    {
        switch (currentState)
        {
            case States.Patrol:
                Patrol();
                break;
            case States.Follow:
                Follow();
                break;
            case States.Attack:
                Attack();
                break;
        }
    }
    private void CheckForPlayer()
    {
        directionToTarget = targetTransform.position - transform.position;

        float distance = directionToTarget.magnitude;
        if (distance <= shootDistance)
        {
            currentState = States.Attack;
            inSight = true;
        }
        else if (distance <= maxFollowDistance && distance > shootDistance)
        {
            currentState = States.Follow;
            inSight = true;
        }
        else
        {
            currentState = States.Patrol;
            inSight = false;
        }
    }

    private void Patrol()
    {
        attackWeapon.fireWeaponBool = false;
        if (agent.destination == null)
        {
            agent.SetDestination(waypoints[Random.Range(0, waypoints.Length)].position);
        }

        if (Vector3.Distance(transform.position, agent.destination) < 2f)
        {
            agent.SetDestination(waypoints[Random.Range(0, waypoints.Length)].position);
        }
    }

    private void Follow()
    {
        attackWeapon.fireWeaponBool = false;
        agent.SetDestination(targetTransform.position);
    }

    private void Attack()
    {
        transform.LookAt(targetTransform);
        attackWeapon.fireWeaponBool = true;
        audioSource.PlayOneShot(audioClip[1]);
    }

    public void EMPShock()
    {
        IEnumerator shock()
        {
            //States preState = currentState;
            currentState = States.Shocked;
            agent.isStopped = true;
            animator.enabled = false;

            // apply damage
            yield return new WaitForSeconds(1);
            TakeDamage(5);
            // play shock effect
            GameObject effect = PhotonNetwork.InstantiateRoomObject(hitEffect.name, transform.position, Quaternion.identity, 0, null);
            yield return new WaitForSeconds(1);
            PhotonNetwork.Destroy(effect);
            yield return new WaitForSeconds(1);
            TakeDamage(5);
            // play shock effect
            GameObject effect2 = PhotonNetwork.InstantiateRoomObject(hitEffect.name, transform.position, Quaternion.identity, 0, null);
            yield return new WaitForSeconds(1);
            PhotonNetwork.Destroy(effect2);
            yield return new WaitForSeconds(1);
            TakeDamage(5);
            // play shock effect
            GameObject effect3 = PhotonNetwork.InstantiateRoomObject(hitEffect.name, transform.position, Quaternion.identity, 0, null);
            yield return new WaitForSeconds(1);
            PhotonNetwork.Destroy(effect3);
            // enable movement

            currentState = States.Follow;
            agent.isStopped = false;

            animator.enabled = true;
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

