using System.Collections;
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
    public Transform droneBulletSpawn;

    public GameObject droneBullet;

    public bool inSight;

    private Vector3 directionToTarget;

    public float shootDistance = 10f;
    public float shootForce = 75;

    public bool fireWeaponBool = false;

    public States currentState;
    public LayerMask obstacleMask;

    public MatchEffects matchEffects;


    private void Start()
    {
        StartCoroutine(FireWeapon());
        matchEffects = GameObject.FindGameObjectWithTag("Props").GetComponent<MatchEffects>();
    }

    void Update()
    {
        if (!matchEffects.codeFound)
        {
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

        if (directionToTarget.magnitude <= shootDistance && CheckForPlayer())
        {
            currentState = States.Attack;
        }
    }

    private bool CheckForPlayer()
    {
        Transform playerPOS = targetTransform;
        if (playerPOS == null)
            return false;

        if (Vector3.Distance(transform.position, playerPOS.position) > shootDistance)
            return false;

        Vector3 directionToTarget = (playerPOS.position - transform.position).normalized;
        if (Vector3.Angle(transform.forward, directionToTarget) > 110 / 2f)
            return false;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToTarget, out hit, shootDistance, obstacleMask))
        {
            // Debugging line
            if (hit.collider != null)
            {
                // More debugging
                if (hit.collider.gameObject.CompareTag("Player") || hit.collider.gameObject.CompareTag("ReactorInteractor"))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void Attack()
    {
        if (!CheckForPlayer() || directionToTarget.magnitude > shootDistance)
        {
            currentState = States.Follow;
        }
        fireWeaponBool = true;
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
        while (true)
        {
            if (fireWeaponBool && !matchEffects.codeFound && matchEffects.startMatchBool)
            {
                GameObject spawnedBullet = Instantiate(droneBullet, droneBulletSpawn.position, Quaternion.identity);
                spawnedBullet.GetComponent<Bullet>().audioSource.PlayOneShot(spawnedBullet.GetComponent<Bullet>().clip);
                spawnedBullet.GetComponent<Bullet>().bulletModifier = 6;
                spawnedBullet.GetComponent<Rigidbody>().velocity = droneBulletSpawn.right * shootForce * GlobalSpeedManager.SpeedMultiplier;
            }
            yield return new WaitForSeconds(.25f);
        }
    }
}
