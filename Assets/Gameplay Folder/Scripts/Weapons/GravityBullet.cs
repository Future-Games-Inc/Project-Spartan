using Umbrace.Unity.PurePool;
using UnityEngine;

public class GravityBullet : MonoBehaviour
{
    [Header("Bullet Behavior ---------------------------------------------------")]
    private bool hasHit = false;

    public GameObject hitEffectPrefab;
    public GameObject bulletOwner;
    public PlayerHealth playerHealth;
    public bool playerBullet = false;

    [Header("Bullet Effects ---------------------------------------------------")]
    public float gravityWellRadius = 20.0f;

    public AudioSource audioSource;
    public AudioClip clip;
    public GameObjectPoolManager PoolManager;

    private void OnEnable()
    {
        // Find the manager if one hasn't been specified.
        if (this.PoolManager == null)
        {
            this.PoolManager = Object.FindObjectOfType<GameObjectPoolManager>();
        }
    }

    private void AffectNearbyObjects()
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, gravityWellRadius);
        foreach (Collider collider in nearbyObjects)
        {
            if (collider.CompareTag("Enemy") || collider.CompareTag("BossEnemy"))
            {
                FollowAI agent = collider.GetComponent<FollowAI>();
                if (agent != null)
                {
                    agent.EMPShock();
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasHit)
        {
            hasHit = true;

            AffectNearbyObjects();

            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            this.PoolManager.Release(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasHit)
        {
            hasHit = true;

            AffectNearbyObjects();

            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            this.PoolManager.Release(gameObject);
        }
    }
}
