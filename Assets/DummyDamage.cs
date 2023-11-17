using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyDamage : MonoBehaviour
{
    public GameObject explosionObject;
    public bool hit;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet") || other.CompareTag("EnemyBullet") || other.CompareTag("Melee"))
        {
            if (!hit)
                StartCoroutine(Hit());
        }

    }

    IEnumerator Hit()
    {
        hit = true; 
        explosionObject.SetActive(true);
        yield return new WaitForSeconds(1);
        hit = false;
        explosionObject.SetActive(false);
    }
}
