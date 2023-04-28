using UnityEngine;

public class StickyBullet : MonoBehaviour
{
    public float stickySurfaceDuration = 5.0f;
    public float stickySurfaceFriction = 0.5f;
    public GameObject stickySurfacePrefab;

    private bool hasHit = false;

    private void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasHit)
        {
            hasHit = true;

            // Create sticky surface on wall
            if (other.CompareTag("Wall") || other.gameObject.layer == 3)
            {
                GameObject stickySurface = Instantiate(stickySurfacePrefab, other.ClosestPoint(transform.position), Quaternion.identity);
                Destroy(stickySurface, stickySurfaceDuration);
                other.material.dynamicFriction = stickySurfaceFriction;
            }

            // Destroy bullet
            Destroy(gameObject);
        }
    }
}
