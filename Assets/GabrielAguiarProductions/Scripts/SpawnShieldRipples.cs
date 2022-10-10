using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SpawnShieldRipples : MonoBehaviour
{
    public GameObject shieldRipples;  

    private VisualEffect shieldRipplesVFX;
    public float blastRadius = 5f;
    public float explosionForce = 700f;

    private void OnCollisionEnter(Collision co)
    {
        if (co.gameObject.tag == "Enemy")
        {
            var ripples = Instantiate(shieldRipples, transform) as GameObject;
            shieldRipplesVFX = ripples.GetComponent<VisualEffect>();
            shieldRipplesVFX.SetVector3("SphereCenter", co.contacts[0].point);

            Destroy(ripples, 2);

            Collider[] collidersToDestroy = Physics.OverlapSphere(transform.position, blastRadius);

            foreach (Collider nearbyObject in collidersToDestroy)
            {
                Enemy_TD enemy = nearbyObject.GetComponent<Enemy_TD>();
                if (enemy != null)
                {
                    enemy.TakeDamage(10);
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
        }
    }
}
