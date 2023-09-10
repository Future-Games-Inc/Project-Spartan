using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;

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


    private void Start()
    {
        StartCoroutine(FireWeapon());
    }

    void Update()
    {
        FindClosestEnemy();
        UpdateStates();
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
        directionToTarget = targetTransform.position - transform.position;

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

        RaycastHit[] hits = Physics.RaycastAll(transform.position, directionToTarget, shootDistance, obstacleMask);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject.CompareTag("Player"))
            {
                return true;
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

        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * 80);
    }

    IEnumerator FireWeapon()
    {
        while (true)
        {
            if (fireWeaponBool)
            {
                GameObject spawnedBullet = Instantiate(droneBullet, droneBulletSpawn.position, Quaternion.identity);
                spawnedBullet.GetComponent<Bullet>().audioSource.PlayOneShot(spawnedBullet.GetComponent<Bullet>().clip);
                spawnedBullet.GetComponent<Bullet>().bulletModifier = 6;
                spawnedBullet.GetComponent<Rigidbody>().velocity = droneBulletSpawn.right * shootForce;
            }
            yield return new WaitForSeconds(.25f);
        }
    }
}
