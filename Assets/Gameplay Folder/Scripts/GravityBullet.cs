using UnityEngine;

public class GravityBullet : MonoBehaviour
{
    public float gravityWellRadius = 10.0f;
    public float gravityWellForce = 100.0f;

    private bool hasHit = false;

    public GameObject hitEffectPrefab;

    private void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasHit)
        {
            hasHit = true;

            // Apply force to nearby objects
            Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, gravityWellRadius);
            foreach (Collider collider in nearbyObjects)
            {
                if (!collider.CompareTag("Player"))
                {
                    Rigidbody rb = collider.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        Vector3 direction = (collider.transform.position - transform.position).normalized;
                        rb.AddForce(direction * gravityWellForce, ForceMode.Impulse);
                    }
                }
            }

            // Destroy bullet
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasHit)
        {
            hasHit = true;

            // Apply force to nearby objects
            Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, gravityWellRadius);
            foreach (Collider collider in nearbyObjects)
            {
                if (!collider.CompareTag("Player"))
                {
                    Rigidbody rb = collider.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        Vector3 direction = (collider.transform.position - transform.position).normalized;
                        rb.AddForce(direction * gravityWellForce, ForceMode.Impulse);
                    }
                }
            }
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);

            // Destroy bullet
            Destroy(gameObject);
        }
    }
}
