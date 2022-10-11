using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    private Transform target;
    [Header("Turrets Range")]
    [SerializeField]public float range = 15f;

    [Header("Turret Fire Rate")]
    public float fireRate = 1f;
    private float fireCountDown = 0f;

    [Header("Turrets Rotation Speed")]
    public float rotationSpeed = 2f;
    [Header("Update Target")][Range(0,1)]
    public float updatedTarget = 1f;

    [Header("Turrets Bullet & Fire Point")]
    public GameObject bulletPrefab;
    public Transform firePoint;


    [Header("Turrent Enemy")]
    public string enemyTag = "Enemy";
    [Header("Turret Pivot Point")]
    public Transform turretsPartToRotate;

    [Header("Use Laser")]
    public bool useLazer = false;
    public LineRenderer lineRenderer;
    public ParticleSystem impactEffect;
    public Light impactLight;


    // Pausing Mechanism
    private void Awake()
    {
        GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;


    }
    // Pausing Mechanism
    private void OnDestroy()
    {
        GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }
    // Pausing Mechanism
    private void OnGameStateChanged(GameState newGameState)
    {
        enabled = newGameState == GameState.Gameplay;
    }

    void Start()
    {
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
    }

    void UpdateTarget()
    {
        //Find all enemies
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        float shortedDistance = Mathf.Infinity;

        GameObject nearestEnemy = null;
        //For each enemy that we find
        foreach (GameObject enemy in enemies)
        {
            //get the distance to that enemy
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            //if the distance we found is smaller than the distance we seen before
            if(distanceToEnemy < shortedDistance)
            {
                // if true then the shortest distance to this enemy is this enemy. 
                shortedDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if(nearestEnemy != null && shortedDistance <= range)
        {
            target = nearestEnemy.transform;
        }
        else
        {
            target = null;
        }

    }

     void Update()
    {
        // if dont have a target dont to a thing. 
        if (target == null)
        {
            if(useLazer)
            {
                if (lineRenderer.enabled)
                {
                    lineRenderer.enabled = false;
                    impactEffect.Stop();
                    impactLight.enabled = false;

                }
                    

            }


            return;
        }

        LockOnTarget();

        if(useLazer)
        {
            Lazer();

        }
        else
        {

            if (fireCountDown <= 0f)
            {
                Shoot();
                fireCountDown = 1f / fireRate;
            }
            fireCountDown -= Time.deltaTime;

        }

        


    }

    void Lazer()
    {
        if(!lineRenderer.enabled)
        {
            lineRenderer.enabled = true;
            impactEffect.Play();
            impactLight.enabled = true;
        }
        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, target.position);

        Vector3 dir = firePoint.position - target.position;
        impactEffect.transform.position = target.position + dir.normalized ;
        impactEffect.transform.rotation = Quaternion.LookRotation(dir);

    }


    private void LockOnTarget()
    {
        //Turrets Direction and Rotation Logic when Target Lock on
        Vector3 direction = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 rotation = Quaternion.Lerp(turretsPartToRotate.rotation, lookRotation, Time.deltaTime * rotationSpeed).eulerAngles;
        turretsPartToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    void Shoot()
    {
        //casting ???
        GameObject bulletGo = (GameObject) Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet_TD bullet = bulletGo.GetComponent<Bullet_TD>();

        if(bullet != null)
        {
            bullet.Seek(target);
        }
    }

     void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }



}
