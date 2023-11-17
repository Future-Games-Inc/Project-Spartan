using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkGrenade : MonoBehaviour
{
    public enum GrenadeType
    {
        Prox,
        Smoke
    }

    public GrenadeType Type;
    public float explosionRadius = 5f;
    public int maxDamage = 80;
    public float explosionDelay = 5f;

    [System.Serializable]
    public struct TargetInfo
    {
        public string Tag;
    }
    public TargetInfo[] Targets;

    public GameObject explosionEffect;
    public AudioSource audioSource;
    public GameObject objectRenderer;



    private void OnEnable()
    {
        StartCoroutine(ExplodeDelayed());
        audioSource.Play();
    }

    public IEnumerator ExplodeDelayed()
    {
        yield return new WaitForSeconds(explosionDelay);
        explosionEffect.SetActive(true);

        if (Type == GrenadeType.Prox)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

            // Create a set of target tags for constant-time lookups
            HashSet<string> targetTags = new HashSet<string>();
            foreach (var target in Targets)
            {
                targetTags.Add(target.Tag);
            }

            if (targetTags.Contains("Player"))
            {
                PlayerHealth player = GameObject.FindGameObjectWithTag("Player").GetComponentInParent<PlayerHealth>();
                float distance = Vector3.Distance(transform.position, player.gameObject.transform.position);
                int damage = CalculateDamage(distance);
                if (player.alive)
                {
                    player.TakeDamage(damage);
                }
            }
        }

        float delay = (Type == GrenadeType.Prox) ? 1f : 15f;
        StartCoroutine(Destroy(delay));
    }

    int CalculateDamage(float distance)
    {
        int damage = (int)((1f - distance / explosionRadius) * maxDamage);
        return Mathf.Clamp(damage, 0, 100);
    }

    IEnumerator Destroy(float delay)
    {
        objectRenderer.SetActive(false);
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
