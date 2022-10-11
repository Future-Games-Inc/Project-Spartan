using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class AttackRadius : MonoBehaviour
{
    public SphereCollider collider;
    protected List<iDamageable> Damageables = new List<iDamageable>();
    public int Damage = 10;
    public float AttackDelay = 0.5f;

    public delegate void AttackEvent(iDamageable Target);
    public AttackEvent OnAttack;
    protected Coroutine AttackCoroutine;

    protected virtual void Awake()
    {
        collider = GetComponent<SphereCollider>();
    }


    protected virtual void OnTriggerEnter(Collider other)
    {
        Debug.Log(" On Trigger Eneter for Attack Radius activating");
        iDamageable damageable = other.GetComponent<iDamageable>();
        if(damageable != null)
        {
            Damageables.Add(damageable);
            if(AttackCoroutine == null)
            {
                AttackCoroutine = StartCoroutine(Attack());
            }
        }
    }


    protected virtual void OnTriggerExit(Collider other)
    {
        iDamageable damageable = other.GetComponent<iDamageable>();
        if(damageable != null)
        {
            Damageables.Remove(damageable);
            if(Damageables.Count == 0)
            {
                StopCoroutine(AttackCoroutine);
                AttackCoroutine = null;

            }
        }
    }

    protected virtual IEnumerator Attack()
    {
        WaitForSeconds wait = new WaitForSeconds(AttackDelay);
        yield return wait;

        iDamageable closestDamageable = null;
        float closesDistance = float.MaxValue;

        while(Damageables.Count > 0)
        {
            for(int i = 0; i < Damageables.Count; i++)
            {
                Transform damageableTransform = Damageables[i].GetTransform();
                float distance = Vector3.Distance(transform.position, damageableTransform.position);

                if(distance < closesDistance)
                {
                    closesDistance = distance;
                    closestDamageable = Damageables[i];
                }

            }

            if(closestDamageable != null)
            {
                OnAttack?.Invoke(closestDamageable);
                closestDamageable.TakeDamage(Damage);
            }

            closestDamageable = null;
            closesDistance = float.MaxValue;
            yield return wait;


            Damageables.RemoveAll(DisableDamages);
        }

        AttackCoroutine = null; 
    }

    protected bool DisableDamages(iDamageable damageable)
    {
        return damageable != null && !damageable.GetTransform().gameObject.activeSelf;


    }



}
