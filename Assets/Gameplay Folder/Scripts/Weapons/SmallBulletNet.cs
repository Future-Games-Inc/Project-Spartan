using UnityEngine;

public class SmallBulletNet : MonoBehaviour
{
    public float explosionRadius = 2.0f;
    public float explosionForce = 30.0f;
    public GameObject explosionPrefab;

    private Transform target;
    private float lifetime;

    public void SetTarget(Transform target, float lifetime)
    {
        this.target = target;
        this.lifetime = lifetime;
    }

    private void Update()
    {
        if (target != null)
        {
            transform.LookAt(target);
            transform.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime * 10.0f);
        }

        lifetime -= Time.deltaTime;
        if (lifetime <= 0.0f)
        {
            Explode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<FiringRangeAI>().TakeDamage(50);
            Explode();
        }

        if (target == null)
        {
            Explode();
        }
    }

    private void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider collider in colliders)
        {
            if (!collider.CompareTag("Player"))
            {
                Rigidbody targetRb = collider.GetComponent<Rigidbody>();
                if (targetRb != null)
                {
                    Vector3 direction = targetRb.transform.position - transform.position;
                    direction.Normalize();
                    targetRb.AddForce(direction * explosionForce, ForceMode.Impulse);
                }
            }
        }

        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}