using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyDamage : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet") || other.CompareTag("EnemyBullet"))
            Debug.Log("Dummy Hit");
        if (other.CompareTag("Melee"))
            Debug.Log("Dummy Hit");
    }
}
