using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[CreateAssetMenu(fileName = "Enemy Configuration", menuName = "ScriptableObject/Enemy Configuration" )]
public class EnemyScriptableObject : ScriptableObject
{
    //Enemy Chonfiguration
    public Enemigo Prefab;
    public AttackScriptableObject attackConfiguarions;

    //public Transform player;
    //public Transform targeted_HomeBase;


    //Enemy Stats
    public int Health = 100;

    //Movement stats
    public EnemyState defaultState;
    public float idleLocationRadius = 4f;
    public float idleMovespeedMultiplier = 0.5f;
    [Range(2, 10)]
    public int waypoints = 4; // number of way points
    //public Vector3[] waypointMarchingPath;

    public float LineOfSightRange = 6f;
    public float FieldOfView = 90f;

    //NavMeshAgent Configs
    [Header("NavMeshAgent Configurations")]
    public float ai_UpdateInterval = 0.1f;
    public float acceleration = 8;
    public float angularSpeed = 120;
    // -1 means everything. 
    public int areamask = -1;
    public int avoidancePriority = 50;
    public float bassOffset = 0;
    public float height = 2f;
    public ObstacleAvoidanceType obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
    public float radius = 0.5f;
    public float speed = 3f;
    public float stoppingDistance = 0.5f;

    public EnemyScriptableObject ScaleUpForLevel(ScalingScriptableObject scaling, int level)
    {
        EnemyScriptableObject scaledUpEnemy = CreateInstance<EnemyScriptableObject>();

        scaledUpEnemy.name = name;
        scaledUpEnemy.Prefab = Prefab;

        scaledUpEnemy.attackConfiguarions = attackConfiguarions.ScaleUpForLevel(scaling, level);

        scaledUpEnemy.Health = Mathf.FloorToInt(Health * scaling.healthCurve.Evaluate(level));
        scaledUpEnemy.defaultState = defaultState;
        scaledUpEnemy.idleLocationRadius = idleLocationRadius;
        scaledUpEnemy.idleMovespeedMultiplier = idleMovespeedMultiplier;
        scaledUpEnemy.waypoints = waypoints;
        scaledUpEnemy.LineOfSightRange = LineOfSightRange;
        scaledUpEnemy.FieldOfView = FieldOfView;

        scaledUpEnemy.ai_UpdateInterval = ai_UpdateInterval;
        scaledUpEnemy.acceleration = acceleration;
        scaledUpEnemy.angularSpeed = angularSpeed;

        scaledUpEnemy.areamask = areamask;
        scaledUpEnemy.avoidancePriority = avoidancePriority;

        scaledUpEnemy.bassOffset = bassOffset;
        scaledUpEnemy.height = height;
        scaledUpEnemy.obstacleAvoidanceType = obstacleAvoidanceType;
        scaledUpEnemy.radius = radius;

        scaledUpEnemy.speed = speed * scaling.speedCurve.Evaluate(level);
        scaledUpEnemy.stoppingDistance = stoppingDistance;

        return scaledUpEnemy;
    }



    public void Setupenemy(Enemigo enemigo)
    {
        enemigo.agent.acceleration = acceleration;
        enemigo.agent.angularSpeed = angularSpeed;
        enemigo.agent.areaMask = areamask;
        enemigo.agent.avoidancePriority = avoidancePriority;
        enemigo.agent.baseOffset = bassOffset;
        enemigo.agent.height = height;
        enemigo.agent.obstacleAvoidanceType = obstacleAvoidanceType;
        enemigo.agent.radius = radius;
        enemigo.agent.speed = speed;
        enemigo.agent.stoppingDistance = stoppingDistance;

        enemigo.movement.updateRate = ai_UpdateInterval;
        enemigo.movement.DefaultState = defaultState;
        enemigo.movement.IdleMoveSpeedMultiplier = idleMovespeedMultiplier;
        enemigo.movement.IdleLocationRadius = idleLocationRadius;
        enemigo.movement.waypoints = new Vector3[waypoints];
        enemigo.movement.lineOfSightChecker.fieldOfView = FieldOfView;
        enemigo.movement.lineOfSightChecker.collider.radius = LineOfSightRange;
        enemigo.movement.lineOfSightChecker.lineOfSightLayer = attackConfiguarions.lineOfSightLayers;

        //enemigo.movement.waypointMarchingPath = waypointMarchingPath;


        //enemigo.movement.player = player;
        //enemigo.movement.targeted_HomeBase = targeted_HomeBase;
        enemigo.start_Health = Health;
        enemigo.health = Health;
        
        //Health = Health;

        

        //(enemigo.AttackRadius.collider == null ? enemigo.AttackRadius.GetComponent<SphereCollider>() : enemigo.AttackRadius.collider).radius = attackConfiguarions.AttackRadius;
        //enemigo.AttackRadius.AttackDelay = attackConfiguarions.AttackDelay;
        //enemigo.AttackRadius.Damage = attackConfiguarions.Damage;
    }
}
