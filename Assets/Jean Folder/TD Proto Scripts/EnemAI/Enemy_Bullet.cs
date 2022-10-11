using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class Enemy_Bullet : PoolableObject
{
    public float autoDestroyTime = 5f;
    public float moveSpeed = 2f;
    public int Damage = 5;
    public Rigidbody rigidbody;
    public GameObject impactEffect;

    private const string Disable_Method_Name = "Disable";

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();


    }

    private void OnEnable()
    {
        CancelInvoke(Disable_Method_Name);
        Invoke(Disable_Method_Name, autoDestroyTime);

    }

    private void OnTriggerEnter(Collider other)
    {
        iDamageable damageable;

        if(other.TryGetComponent<iDamageable>(out damageable))
        {
            GameObject effectInstance = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);
            Destroy(effectInstance, 5f);
            damageable.TakeDamage(Damage);

        }
        Disable();

    }

    private void Disable()
    {
        CancelInvoke(Disable_Method_Name);
        rigidbody.velocity = Vector3.zero;
        gameObject.SetActive(false);


    }


}
