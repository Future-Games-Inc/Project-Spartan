using System.Collections;
using UnityEngine;

public class StingerBullet : MonoBehaviour
{
    public float energyPulseRadius = 5.0f;
    public int numSmallBullets = 100;
    public float smallBulletTargetRadius = 10.0f;
    public float smallBulletLifetime = 5.0f;
    public GameObject smallBulletPrefab;
    public GameObject explosionPrefab;
    public Transform spawnTransform;

    private Rigidbody rb;
    private bool hasExploded = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(ExplodeBullets());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Bullet"))
        {
            if (!hasExploded)
            {
                hasExploded = true;
                Explode();
            }
        }
    }

    IEnumerator ExplodeBullets()
    {
        yield return new WaitForSeconds(3);
        if (!hasExploded)
        {
            hasExploded = true;
            Explode();
        }
    }

    private void Explode()
    {
        // Create an energy pulse that pushes enemies away
        Collider[] colliders = Physics.OverlapSphere(transform.position, energyPulseRadius);
        foreach (Collider collider in colliders)
        {
            if (!collider.CompareTag("Player"))
            {
                Rigidbody targetRb = collider.GetComponent<Rigidbody>();
                if (targetRb != null)
                {
                    Vector3 direction = targetRb.transform.position - transform.position;
                    direction.Normalize();
                    targetRb.AddForce(direction * 30.0f, ForceMode.Impulse);
                }
            }
        }

        // Create smaller bullets and target nearby enemies
        for (int i = 0; i < numSmallBullets; i++)
        {
            GameObject smallBullet = Instantiate(smallBulletPrefab, transform.position, Quaternion.identity);
            smallBullet.transform.forward = Random.insideUnitSphere;
            Collider[] targets = Physics.OverlapSphere(transform.position, smallBulletTargetRadius);
            if (targets.Length > 0)
            {
                Transform target = targets[Random.Range(0, targets.Length)].transform;
                smallBullet.GetComponent<SmallBullet>().SetTarget(target, smallBulletLifetime);
            }
        }

        // Create explosion effect and destroy bullet
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
