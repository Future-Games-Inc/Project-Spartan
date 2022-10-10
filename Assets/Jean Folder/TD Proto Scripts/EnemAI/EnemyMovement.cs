using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(AgentLinkMover))]
public class EnemyMovement : MonoBehaviour
{
    public Transform player;

    public Transform targeted_HomeBase;

    public EnemyLineOfSightCheck lineOfSightChecker;
    //public AISensor AI_Sensor;
    public NavMeshTriangulation triangulation;
    public float updateRate = 0.1f; // How quickly we recalcula te the targets transform path. 
    private NavMeshAgent agent;
    private AgentLinkMover LinkMover;
    [SerializeField]
    private Animator animator = null;

    public EnemyState DefaultState;
    private EnemyState _state;
    public EnemyState State
    {
        get
        {
            return _state;
        }
        set
        {
            onStateChange?.Invoke(_state, value);
            _state = value;
        }
    }

    public delegate void StateChangeEvent(EnemyState oldstate, EnemyState newState);
    public StateChangeEvent onStateChange;
    public float IdleLocationRadius = 4f;
    public float IdleMoveSpeedMultiplier = 0.5f;
    public Vector3[] waypoints = new Vector3[4];
    public Vector3[] waypointMarchingPath; 

    [SerializeField]
    private int WaypoinIndex = 0;

    private const string IsWalking = "IsWalking";
    private const string Jump = "Jump";
    private const string Landed = "Landed";

    private Coroutine FollowCoroutine;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        LinkMover = GetComponent<AgentLinkMover>();

        LinkMover.OnLinkStart += HandleLinkStart;
        LinkMover.OnLinkEnd += HandleLinkEnd;

        lineOfSightChecker.onGainSight += handleGainSight;
        lineOfSightChecker.onLoseSight += handleLoseSight;

        onStateChange += HandleStateChange;

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        targeted_HomeBase = GameObject.FindGameObjectWithTag("HomeBase").GetComponent<Transform>();

    }

    private void Start()
    {
        //player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        //targeted_HomeBase = GameObject.FindGameObjectWithTag("HomeBase").GetComponent<Transform>();

    }


    private void handleGainSight(PlayerStats player)
    {
        Debug.Log("Enemy has spotted you!! RUN");
        State = EnemyState.Chase;
    }

    private void handleLoseSight(PlayerStats player)
    {
        Debug.Log("Lost sight of player Default state activate.");

        State = DefaultState;
    }

    private void OnDisable()
    {
        _state = DefaultState; // use _state to avoid triggering OnStateChange when recyclin object in the pool
    }

    public void Spawn()
    {
        for (int i = 0; i < waypoints.Length; i ++)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(triangulation.vertices[Random.Range(0, triangulation.vertices.Length)], out hit, 2f, agent.areaMask))
            {
                waypoints[i] = hit.position;
            }
            else
            {
                Debug.LogError("Unable to find position for navmesh new Triangulation vertex!");
            }

        }
        onStateChange?.Invoke(EnemyState.Spawn, DefaultState);

    }

    // Start is called before the first frame update
    private void Update()
    {
        // may need to remove line
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }





    private void HandleLinkStart()
    {
        animator.SetTrigger(Jump);
    }

    private void HandleLinkEnd()
    {

        animator.SetTrigger(Landed);
    }

    private void HandleStateChange(EnemyState oldState, EnemyState newstate)
    {
        // if the old state is not the new state
        if(oldState != newstate)
        {
            if(FollowCoroutine != null)
            {

                StopCoroutine(FollowCoroutine);
            }
            if(oldState == EnemyState.Idle)
            {
                agent.speed /= IdleMoveSpeedMultiplier;

            }

            switch(newstate)
            {
                case EnemyState.Idle:
                    FollowCoroutine = StartCoroutine(DoIdleMotion());
                    break;

                case EnemyState.Patrol:
                    FollowCoroutine = StartCoroutine(DoPatrolMotion());
                    break;

                case EnemyState.Chase:
                    FollowCoroutine = StartCoroutine(FollowTarget());
                        break;

                case EnemyState.WaypointMarch:
                    FollowCoroutine = StartCoroutine(DoMarchToHomeBase());
                    break;

            }


        }


    }


    private IEnumerator DoMarchToHomeBase()
    {
        WaitForSeconds wait = new WaitForSeconds(updateRate);

        while (true)
        {
            if (agent.enabled)
            {
                agent.SetDestination(targeted_HomeBase.transform.position);
            }
            yield return wait;
        }

    }



    private IEnumerator DoIdleMotion()
    {
        WaitForSeconds wait = new WaitForSeconds(updateRate);
        agent.speed *= IdleMoveSpeedMultiplier;

        while(true)
        {
            if (!agent.enabled || !agent.isOnNavMesh)
                {
                yield return wait;

                }
            else if (agent.remainingDistance <= agent.stoppingDistance)
            {
                Vector2 point = UnityEngine.Random.insideUnitCircle * IdleLocationRadius;
                NavMeshHit hit;

                if(NavMesh.SamplePosition(agent.transform.position + new Vector3(point.x, 0, point.y), out hit, 2f, agent.areaMask))
                {
                    agent.SetDestination(hit.position);

                }

            }

            yield return wait;

        }

    }

    private IEnumerator DoPatrolMotion()
    {
        WaitForSeconds wait = new WaitForSeconds(updateRate);

        yield return new WaitUntil(() => agent.enabled && agent.isOnNavMesh);
        agent.SetDestination(waypoints[WaypoinIndex]);

        while(true)
        {
            if(agent.isOnNavMesh && agent.enabled && agent.remainingDistance <= agent.stoppingDistance)
            {
                WaypoinIndex++;
                if(WaypoinIndex >= waypoints.Length)
                {
                    WaypoinIndex = 0;

                }
                agent.SetDestination(waypoints[WaypoinIndex]);

            }

            yield return wait;
        }

    }


    private IEnumerator FollowTarget()
    {
        WaitForSeconds wait = new WaitForSeconds(updateRate);

        while (true)
        {
            if (agent.enabled)
            {
                agent.SetDestination(player.transform.position);
            }
            yield return wait;
        }

    }


    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < waypoints.Length; i ++)
        {
            Gizmos.DrawWireSphere(waypoints[i], 0.25f);
            if(i+1 < waypoints.Length)
            {
                Gizmos.DrawLine(waypoints[i], waypoints[i + 1]);
            }
            else
            {
                Gizmos.DrawLine(waypoints[i], waypoints[0]);
            }
        }
    }


    public void FoundPlayer(Transform player_)
    {
        player = player_;

    }


}
