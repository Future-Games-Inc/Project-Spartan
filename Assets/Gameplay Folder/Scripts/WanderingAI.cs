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
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

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
}

