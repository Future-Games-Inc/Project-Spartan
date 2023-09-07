using System.Collections;
using System.Linq;
using UnityEngine;

public class DroneCompanion : MonoBehaviour
{
    public enum States
    {
        Follow,
        Attack
    }

    [Header("AI Properties")]

    public Transform targetTransform;
    public Transform droneBulletSpawn;

    public GameObject[] enemies;
    public GameObject[] bosses;
    public GameObject[] security;
    public GameObject[] allEnemies;
    public GameObject droneBullet;

    public GameObject player;

    public bool inSight;

    private Vector3 directionToTarget;

    public float shootDistance = 10f;
    public float shootForce = 75;

    public bool fireWeaponBool = false;

    public States currentState;

    private void OnEnable()
    {
        FindClosestEnemy();
        StartCoroutine(FireWeapon());
    }

    void Update()
    {
        FindClosestEnemy();
        UpdateStates();
    }

    private void UpdateStates()
    {
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
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        bosses = GameObject.FindGameObjectsWithTag("BossEnemy");
        security = GameObject.FindGameObjectsWithTag("Security");
        allEnemies = enemies.Concat(bosses).Concat(security).ToArray();

        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in allEnemies)
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

    private void Follow()
    {
        fireWeaponBool = false;
        directionToTarget = targetTransform.position - transform.position;

        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, directionToTarget.normalized, out hitInfo))
        {
            inSight = hitInfo.transform.CompareTag("Enemy") || hitInfo.transform.CompareTag("BossEnemy") || hitInfo.transform.CompareTag("Security");
        }

        if (directionToTarget.magnitude <= shootDistance && inSight)
        {
            currentState = States.Attack;
        }
    }

    private void Attack()
    {
        if (!inSight || directionToTarget.magnitude > shootDistance)
        {
            currentState = States.Follow;
        }
        LookAtTarget();
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
            while (!fireWeaponBool)
                yield return null;
            GameObject spawnedBullet = Instantiate(droneBullet, droneBulletSpawn.position, Quaternion.identity);
            spawnedBullet.GetComponent<Bullet>().audioSource.PlayOneShot(spawnedBullet.GetComponent<Bullet>().clip);
            spawnedBullet.GetComponent<Bullet>().bulletModifier = (int)Random.Range(1, 4);
            spawnedBullet.GetComponent<Bullet>().bulletOwner = player;
            spawnedBullet.GetComponent<Bullet>().playerBullet = true;
            spawnedBullet.GetComponent<Rigidbody>().velocity = droneBulletSpawn.right * shootForce;
            yield return new WaitForSeconds(.25f);
        }
    }
}
