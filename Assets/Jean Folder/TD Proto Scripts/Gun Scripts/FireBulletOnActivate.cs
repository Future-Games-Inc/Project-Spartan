using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class FireBulletOnActivate : MonoBehaviour
{
    private Transform target;
    [SerializeField]
    [Tooltip("Range of the gun to identify the enemy target")]
    public float range = 15f;
    public string enemyTag = "Enemy";

    public GameObject bulletprefab;
    public Transform spawnpoint;
    [SerializeField] LineRenderer lineRenderer;
    public float fireSpeed = 20;
    public AudioSource source;
    public AudioClip clip;

    public Transform gunTransform;
    public float rayCastLength = 100f;

    private XRGrabInteractable grabbable;

    private bool hasHit;
    private RaycastHit hit;
    


    // Start is called before the first frame update
    void Start()
    {
        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
        grabbable.activated.AddListener(FireBullet);
        InvokeRepeating("UpdateTarget", 0f, 0.5f);

    }

    // Update is called once per frame
    void Update()
    {
        var ray = new Ray(origin: spawnpoint.transform.position, spawnpoint.transform.forward );
        RaycastHit updatedHit;
        if (Physics.Raycast(ray, out updatedHit, 100))
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, spawnpoint.transform.position);
            lineRenderer.SetPosition(1, updatedHit.point);

            hit = updatedHit;
        }

        bool hasHit = Physics.Raycast(ray, out updatedHit, rayCastLength);

        ;
    }

    public void FireBullet(ActivateEventArgs arg)
    {


        //GameObject spawnedBullet = Instantiate(bulletprefab);
        //spawnedBullet.transform.position = spawnpoint.position;
        //spawnedBullet.GetComponent<Rigidbody>().velocity = spawnpoint.forward * fireSpeed;
        //Destroy(spawnpoint, 6f);
        Shoot();
        source.PlayOneShot(clip);


    }

    public void Shoot()
    {

        GameObject bulletGo = (GameObject)Instantiate(bulletprefab, spawnpoint.position, spawnpoint.rotation);
        Bullet_TD bullet = bulletGo.GetComponent<Bullet_TD>();

        if (bullet != null)
        {

            bullet.Seek(target);

        }

    }

    void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        float shortedDistance = Mathf.Infinity;

        GameObject nearestEnemy = null;
        //For each enemy that we find
        foreach (GameObject enemy in enemies)
        {
            //get the distance to that enemy
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            //if the distance we found is smaller than the distance we seen before
            if (distanceToEnemy < shortedDistance)
            {
                // if true then the shortest distance to this enemy is this enemy. 
                shortedDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }



        }
        if (nearestEnemy != null && shortedDistance <= range)
        {
            target = nearestEnemy.transform;
        }
        else
        {
            target = null;
        }

    }


}
