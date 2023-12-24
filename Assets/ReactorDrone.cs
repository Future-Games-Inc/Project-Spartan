using System.Collections;
using Umbrace.Unity.PurePool;
using Unity.XR.CoreUtils;
using UnityEngine;
using static DroneHealth;

public class ReactorDrone : MonoBehaviour
{
    public enum States
    {
        Follow,
        Attack
    }

    [Header("AI Properties")]

    public Transform targetTransform;
    public Transform[] droneBulletSpawn;

    public GameObject droneBullet;

    public bool inSight;

    private Vector3 directionToTarget;

    public float shootDistance = 10f;
    public float shootForce = 75;

    public bool fireWeaponBool = false;

    public States currentState;
    public LayerMask obstacleMask;

    public MatchEffects matchEffects;

    public float sphereRadius = 0.5f;

    public GameObjectPoolManager PoolManager;

    private void Awake()
    {
        // Find the manager if one hasn't been specified.
        if (this.PoolManager == null)
        {
            this.PoolManager = Object.FindObjectOfType<GameObjectPoolManager>();
        }
    }

    private void Start()
    {
        matchEffects = GameObject.FindGameObjectWithTag("Props").GetComponent<MatchEffects>();
    }

    void Update()
    {
        if (!matchEffects.codeFound)
        {
            CheckForPlayer();
            FindClosestEnemy();
            directionToTarget = targetTransform.position - transform.position;

            UpdateStates();
        }
    }

    private void UpdateStates()
    {
        LookAtTarget();
        switch (currentState)
        {
            case States.Follow:
                Follow();
                break;
            case States.Attack:
                Attack();
                break;
        }
    }

    public void FindClosestEnemy()
    {
        //players = FindObjectsOfType<XROrigin>().ToList<>;
        //GameObject closest = null;
        //float distance = Mathf.Infinity;
        //Vector3 position = transform.position;

        //foreach (GameObject go in players)
        //{
        //    Vector3 diff = go.transform.position - position;
        //    float curDistance = diff.sqrMagnitude;
        //    if (curDistance < distance)
        //    {
        //        closest = go;
        //        distance = curDistance;
        //    }
        //}

        targetTransform = FindObjectOfType<XROrigin>().transform;
    }

    private void Follow()
    {
        fireWeaponBool = false;
        StopCoroutine(FireWeapon());

        if (directionToTarget.magnitude <= shootDistance && CheckForPlayer())
        {
            currentState = States.Attack;
        }
    }

    private bool CheckForPlayer()
    {
        Transform playerPOS = targetTransform;
        if (playerPOS == null)
        {
            return false;
        }

        if (Vector3.Distance(transform.position, playerPOS.position) > shootDistance)
        {
            return false;
        }

        Vector3 directionToTarget = ((playerPOS.position + new Vector3(0, 3, 0)) - transform.position).normalized;
        if (Vector3.Angle(transform.forward, directionToTarget) > 110 / 2f)
        {
            return false;
        }

        RaycastHit hit;
        if (Physics.SphereCast(transform.position, sphereRadius, directionToTarget, out hit, shootDistance, obstacleMask))
        {
            // Check for PlayerHealth in the hit object and its parents
            return CheckForPlayerHealthInHierarchy(hit.collider.gameObject);
        }
        return false;
    }

    private bool CheckForPlayerHealthInHierarchy(GameObject obj)
    {
        while (obj != null)
        {
            if (obj.GetComponent<PlayerHealth>() != null)
                return true;
            obj = obj.transform.parent?.gameObject;
        }
        return false;
    }

    private void Attack()
    {
        if (!CheckForPlayer() || directionToTarget.magnitude > shootDistance)
        {
            currentState = States.Follow;
        }
        StartCoroutine(FireWeapon());
    }

    private void LookAtTarget()
    {
        Vector3 lookDirection = directionToTarget;
        lookDirection.y = 0f;

        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);

        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * 1000 * GlobalSpeedManager.SpeedMultiplier);
    }

    IEnumerator FireWeapon()
    {
        if (!fireWeaponBool && !matchEffects.codeFound && matchEffects.startMatchBool)
        {
            fireWeaponBool = true;
            foreach (Transform spawn in droneBulletSpawn)
            {
                GameObject spawnedBullet = this.PoolManager.Acquire(droneBullet, spawn.position, Quaternion.identity);
                spawnedBullet.GetComponent<Bullet>().audioSource.PlayOneShot(spawnedBullet.GetComponent<Bullet>().clip);
                spawnedBullet.GetComponent<Bullet>().bulletModifier = 6;
                spawnedBullet.GetComponent<Rigidbody>().velocity = spawn.right * shootForce * GlobalSpeedManager.SpeedMultiplier;
            }
        }
        yield return new WaitForSeconds(Random.Range(0.25f, 0.75f));
        fireWeaponBool = false;
    }
}
