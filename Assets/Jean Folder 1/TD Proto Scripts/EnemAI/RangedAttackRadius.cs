using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangedAttackRadius : AttackRadius
{
    public NavMeshAgent agent;
    public Enemy_Bullet bulletPrefab;
    public Vector3 bulletSpawnOffset = new Vector3(0, 1, 0);
    public LayerMask layermask;
    private ObjectPool bulletPool;
    private float sphereCastRadius = 0.1f;
    private RaycastHit hit;

    private iDamageable target_Damageable;



    public void CreateBulletPool()
    {

        if(bulletPool == null)
        {

        bulletPool = ObjectPool.CreateInstance(bulletPrefab, Mathf.CeilToInt((1 / AttackDelay) * bulletPrefab.autoDestroyTime));

        }


    }




    protected override IEnumerator Attack()
    {
        WaitForSeconds wait = new WaitForSeconds(AttackDelay);

        yield return wait;

        while (Damageables.Count > 0)
        {
            for(int i = 0; i < Damageables.Count; i++)
            {
                if(HasLineOfSightTo(Damageables[i].GetTransform()))
                {
                    target_Damageable = Damageables[i];
                    OnAttack?.Invoke(Damageables[i]);
                    agent.enabled = false;
                    break;

                }

            }
            if(target_Damageable != null)
            {
                PoolableObject poolableObject = bulletPool.GetObject();
                if(poolableObject != null)
                {
                    bulletPrefab = poolableObject.GetComponent<Enemy_Bullet>();
                    bulletPrefab.Damage = Damage;
                    bulletPrefab.transform.position = transform.position + bulletSpawnOffset;
                    bulletPrefab.transform.rotation = agent.transform.rotation;
                    bulletPrefab.rigidbody.AddForce(agent.transform.forward * bulletPrefab.moveSpeed, ForceMode.VelocityChange);
                }
            }

            else
            {
                agent.enabled = true; // No Target in the line of sight
            }

            yield return wait;

            if(target_Damageable == null || !HasLineOfSightTo(target_Damageable.GetTransform()))
            {

                agent.enabled = true;
            }

            Damageables.RemoveAll(DisableDamages);
        }

        agent.enabled = true;
        AttackCoroutine = null;

    }

    private bool HasLineOfSightTo(Transform target)
    {
        if (Physics.SphereCast(target.position + bulletSpawnOffset, sphereCastRadius, ((target.position + bulletSpawnOffset) - (transform.position + bulletSpawnOffset)).normalized, out hit, collider.radius, layermask))
        {

            iDamageable damageable;
            if(hit.collider.TryGetComponent<iDamageable>(out damageable))
            {
                return damageable.GetTransform() == target;


            }

            // to be fixed.... for multiplayer scendarios

        }
        return false;
    }



    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);

        if(AttackCoroutine == null)
        {
            agent.enabled = true;

        }


    }





}
