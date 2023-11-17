using System.Collections;
using UnityEngine;

public class DeflectionShield : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip audioClip;

    public GameObject hitPrefab;
    public bool hit;

    // Called when another object enters a trigger collider attached to this object
    void OnTriggerEnter(Collider other)
    {
        audioSource.PlayOneShot(audioClip);
        // Check if the other object has the tag "EnemyBullet"
        if (other.CompareTag("EnemyBullet"))
        {
            if (!hit)
            {
                hit = true;
                hitPrefab.SetActive(true);
                StartCoroutine(Hit());
                Destroy(other.gameObject);
            }
        }
    }

    IEnumerator Hit()
    {
        yield return new WaitForSeconds(0.5f);
        hit = false;
        hitPrefab.SetActive(false);
    }
}

