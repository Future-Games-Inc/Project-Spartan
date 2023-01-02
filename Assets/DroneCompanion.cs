using Invector.vCharacterController.AI;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

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
    public GameObject[] allEnemies;
    public GameObject droneBullet;   

    public bool inSight;

    private Vector3 directionToTarget;

    public float shootDistance = 10f;
    public float shootForce;

    public States currentState;

    private void Start()
    {
        FindClosestEnemy();
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
        allEnemies = enemies.Concat(bosses).ToArray();

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
        directionToTarget = targetTransform.position - transform.position;

        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, directionToTarget.normalized, out hitInfo))
        {
            inSight = hitInfo.transform.CompareTag("Enemy") || hitInfo.transform.CompareTag("BossEnemy");
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
        StartCoroutine(FireWeapon());
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
        yield return new WaitForSeconds(2);
        GameObject spawnedBullet = PhotonNetwork.Instantiate(droneBullet.name, droneBulletSpawn.position, Quaternion.identity);
        spawnedBullet.GetComponent<Bullet>().bulletModifier = (int)Random.Range(1, 4);
        shootForce = (int)Random.Range(40, 75);
        spawnedBullet.GetComponent<Rigidbody>().velocity = droneBulletSpawn.right * shootForce;
    }
}
