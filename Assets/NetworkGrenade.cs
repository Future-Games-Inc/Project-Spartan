using System.Collections;
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
    public Rigidbody rb;
    public AudioSource audioSource;
    public GameObject objectRenderer;



    private void OnEnable()
    {
        StartCoroutine(ExplodeDelayed());
    }

    public IEnumerator ExplodeDelayed()
    {
        yield return new WaitForSeconds(explosionDelay);
        explosionEffect.SetActive(true);

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var target in Targets)
        {
            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag(target.Tag))
                {
                    HandleDamage(collider, target);
                }
            }
        }

        float delay = (Type == GrenadeType.Prox) ? 1f : 15f;
        StartCoroutine(Destroy(delay));
    }

    void HandleDamage(Collider collider, TargetInfo target)
    {
        float distance = Vector3.Distance(transform.position, collider.transform.position);
        int damage = CalculateDamage(distance);

        // Handle damage based on the target's tag
        switch (target.Tag)
        {
            case "Player":
                HandleEnemyDamage(collider, damage, target.Tag);
                return;
                // Add more cases as needed
        }
    }

    void HandleEnemyDamage(Collider collider, int damage, string enemyType)
    {
        PlayerHealth enemyDamageCrit2 = collider.GetComponent<PlayerHealth>();
        if (enemyDamageCrit2 != null)
        {
            if (enemyDamageCrit2.alive)
            {
                enemyDamageCrit2.TakeDamage(damage);
            }
        }
    }

    int CalculateDamage(float distance)
    {
        return Mathf.Min((int)((1f - distance / explosionRadius) * (maxDamage/2)), 100);
    }

    IEnumerator Destroy(float delay)
    {
        objectRenderer.SetActive(false);
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
