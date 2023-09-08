using UnityEngine;
using UnityEngine.AI;

public class WanderingAI : MonoBehaviour
{

    public float wanderRadius;
    public float wanderTimer;

    public NavMeshAgent agent;
    public float timer;

    // Use this for initialization
    void OnEnable()
    {
        timer = wanderTimer;
        NavMeshHit closestHit;

        if (NavMesh.SamplePosition(agent.transform.position, out closestHit, 500f, NavMesh.AllAreas))
        {
            agent.enabled = false;
            agent.transform.position = closestHit.position;
            agent.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.isOnNavMesh)
        {
            timer += Time.deltaTime;

            if (timer >= wanderTimer && agent.enabled == true)
            {
                Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                agent.SetDestination(newPos);
                timer = 0;
            }
        }
        else
        {
            NavMeshHit closestHit;

            if (NavMesh.SamplePosition(agent.transform.position, out closestHit, 500f, NavMesh.AllAreas))
            {
                agent.enabled = false;
                agent.transform.position = closestHit.position;
                agent.enabled = true;
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
}

