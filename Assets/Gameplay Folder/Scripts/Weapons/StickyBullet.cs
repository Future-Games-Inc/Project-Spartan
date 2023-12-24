using Umbrace.Unity.PurePool;
using UnityEngine;

public class StickyBullet : MonoBehaviour
{
    [Header("Bullet Behavior ---------------------------------------------------")]
    public GameObject stickySurfacePrefab;

    private bool hasHit = false;

    [Header("Bullet Effects ---------------------------------------------------")]
    public float stickySurfaceDuration = 5.0f;
    public float stickySurfaceFriction = 0.5f;

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


    private void OnTriggerEnter(Collider other)
    {
        if (!hasHit)
        {
            hasHit = true;

            // Create sticky surface on wall
            if (!other.CompareTag("Enemy") || !other.CompareTag("Security") || !other.CompareTag("Player") || !other.CompareTag("BossEnemy"))
            {
                GameObject stickySurface = this.PoolManager.Acquire(stickySurfacePrefab, other.ClosestPoint(transform.position), Quaternion.identity);
            }

            // Destroy bullet
            this.PoolManager.Release(gameObject);
        }
    }
}
