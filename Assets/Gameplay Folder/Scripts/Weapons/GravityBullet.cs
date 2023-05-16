using Photon.Pun;
using UnityEngine;

public class GravityBullet : MonoBehaviourPunCallbacks
{
    [Header("Bullet Behavior ---------------------------------------------------")]
    private bool hasHit = false;

    public GameObject hitEffectPrefab;
    public GameObject bulletOwner;
    public PlayerHealth playerHealth;
    public bool playerBullet = false;

    [Header("Bullet Effects ---------------------------------------------------")]
    public float gravityWellRadius = 10.0f;
    public float gravityWellForce = 100.0f;

    private void Update()
    {

    }

    [System.Obsolete]
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

                if (other.CompareTag("Player"))
                {
                    //critical hit here
                    PlayerHealth playerDamageCrit = other.GetComponent<PlayerHealth>();
                    if (playerDamageCrit.Health <= (5) && playerDamageCrit.alive == true && playerHealth != null)
                    {
                        playerHealth.PlayersKilled();
                    }
                    Rigidbody rb = collider.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        Vector3 direction = (collider.transform.position - transform.position).normalized;
                        rb.AddForce(direction * gravityWellForce, ForceMode.Impulse);
                        playerDamageCrit.TakeDamage(5);
                    }
                }
            }
            PhotonNetwork.InstantiateRoomObject(hitEffectPrefab.name, transform.position, Quaternion.identity, 0, null);

            // Destroy bullet
            PhotonNetwork.Destroy(gameObject);
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
            PhotonNetwork.InstantiateRoomObject(hitEffectPrefab.name, transform.position, Quaternion.identity, 0, null);

            // Destroy bullet
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
