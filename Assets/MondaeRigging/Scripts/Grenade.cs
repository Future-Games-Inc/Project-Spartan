using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float delay = 3f;
    public GameObject explosionEffect;
    public float blastRadius = 5f;
    public float explosionForce = 700;

    float countdown;
    bool hasExploded;
    public bool alreadyPickedUp;
    public bool dropped;
    // Start is called before the first frame update
    void Start()
    {
        countdown = delay;
        alreadyPickedUp = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (dropped == true && alreadyPickedUp == true)
        {
            StartCoutrdown();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("LeftHand") || other.CompareTag("LeftHand"))
        {
            alreadyPickedUp=true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        dropped = true;
    }

    void Explode()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);
        Collider[] collidersToDestroy = Physics.OverlapSphere(transform.position, blastRadius);

        foreach(Collider nearbyObject in collidersToDestroy)
        {
            Enemy_TD enemy = nearbyObject.GetComponent<Enemy_TD>();
            if(enemy != null)
            {
                enemy.TakeDamage(100);
            }
        }

        Collider[] collidersToMove = Physics.OverlapSphere(transform.position, blastRadius);
        foreach (Collider nearbyObject in collidersToMove)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, blastRadius);
            }
        }
        Destroy(gameObject);
    }

    public void StartCoutrdown()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0 && !hasExploded)
        {
            Explode();
            hasExploded = true;
        }
    }
}
