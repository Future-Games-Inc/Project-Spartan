using UnityEngine;

public class BlackoutBullet : MonoBehaviour
{
    public int damage = 10;
    public float blastRadius = 5.0f;
    public float maxBlindDuration = 6.0f;
    public float minBlindDuration = 2.0f;
    public GameObject hitEffectPrefab;
    public GameObject smoke;

    private bool hasHit = false;

    private void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasHit)
        {
            hasHit = true;

            // Apply damage to enemy
            if (other.CompareTag("Player"))
            {
                other.GetComponent<PlayerHealth>().TakeDamage(damage);
            }

            // Apply blind effect to enemy
            if (other.CompareTag("Player"))
            {
                float distance = Vector3.Distance(transform.position, other.transform.position);
                float blindDuration = Mathf.Lerp(maxBlindDuration, minBlindDuration, distance / blastRadius);
                other.GetComponent<PlayerHealth>().ApplyBlindEffect(blindDuration);
            }

            // Apply hit effect
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            Instantiate(smoke,transform.position, Quaternion.identity);

            // Destroy bullet
            Destroy(gameObject);
        }
    }
}
