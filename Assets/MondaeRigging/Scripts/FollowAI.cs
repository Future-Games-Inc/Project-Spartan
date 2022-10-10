using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class FollowAI : MonoBehaviour
{
    public enum States
    {
        Patrol,
        Follow,
        Attack
    }

    public NavMeshAgent agent;
    public Transform targetTransform;

    public Transform[] waypoints;

    [Header("AI Properties")]
    public float maxFollowDistance = 20f;
    public float shootDistance = 10f;
    public AIWeapon attackWeapon;
    public float Health = 100f;

    public bool inSight;
    private Vector3 directionToTarget;

    public States currentState;

    public int currentWaypoint;
    // Start is called before the first frame update
    void Start()
    {        
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        currentWaypoint = Random.Range(0, 15);

        FindClosestEnemy();

    }

    public GameObject FindClosestEnemy()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Player");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
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
        return closest;
    }

    // Update is called once per frame
    void Update()
    {
        CheckForPlayer();
        UpdateStates();
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

        RaycastHit hitInfo;
        if(Physics.Raycast(transform.position, directionToTarget.normalized, out hitInfo))
        {
            inSight = hitInfo.transform.CompareTag("Player");
        }
    }
    private void Patrol()
    {
        if(agent.destination != waypoints[currentWaypoint].position)
        {
            agent.destination = waypoints[currentWaypoint].position;
        }

        if(HasReached())
        {
            currentWaypoint = (currentWaypoint + Random.Range(1,6)) % waypoints.Length;
        }

        if(inSight && directionToTarget.magnitude <= maxFollowDistance)
        {
            currentState = States.Follow;
        }
    }

    private void Follow()
    {
        if (directionToTarget.magnitude <= shootDistance && inSight)
        {
            agent.ResetPath();
            currentState = States.Attack;
        }

        else
        {
            if (targetTransform != null)
            {
                agent.SetDestination(targetTransform.position);
            }

            if(directionToTarget.magnitude > maxFollowDistance)
            {
                currentState = States.Patrol;
            }
        }
    }

    private void Attack()
    {
        if(!inSight || directionToTarget.magnitude > shootDistance)
        {
            currentState = States.Follow;
        }
        LookAtTarget();
        attackWeapon.Fire();
    }

    private void LookAtTarget()
    {
        Vector3 lookDirection = directionToTarget;
        lookDirection.y = 0f;

        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);

        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * agent.angularSpeed);
    }

    private bool HasReached()
    {
        return (agent.hasPath && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance);
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;

        if(Health <= 0)
        {
            Invoke(nameof(DestroyEnemy), 5f);
        }    
    }

    private void DestroyEnemy()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
