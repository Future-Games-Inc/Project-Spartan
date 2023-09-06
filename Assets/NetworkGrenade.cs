using System.Collections;
using UnityEngine;
using Photon.Pun;

public class NetworkGrenade : MonoBehaviourPunCallbacks
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
    public float throwForce;
    public float throwUpwardForce;

    [System.Serializable]
    public struct TargetInfo
    {
        public string Tag;
    }
    public TargetInfo[] Targets;

    public GameObject player;
    public PlayerHealth playerHealth;
    public GameObject explosionEffect;
    public Rigidbody rb;
    public AudioSource audioSource;
    public GameObject objectRenderer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LeftHand") || other.CompareTag("RightHand"))
        {
            player = other.transform.root.gameObject;
        }
    }

    public void Throw()
    {
        if (!photonView.IsMine) return;

        ApplyThrowForce();
        audioSource.Play();
        StartCoroutine(ExplodeDelayed());
    }

    private void ApplyThrowForce()
    {
        Vector3 forceDirection = transform.forward;
        Vector3 forceToAdd = forceDirection * throwForce * throwUpwardForce;
        rb.AddForce(forceToAdd);
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

        float delay = (Type == GrenadeType.Prox) ? 0.2f : 15f;
        StartCoroutine(Destroy(delay));
    }

    void HandleDamage(Collider collider, TargetInfo target)
    {
        float distance = Vector3.Distance(transform.position, collider.transform.position);
        int damage = CalculateDamage(distance);

        // Handle damage based on the target's tag
        switch (target.Tag)
        {
            case "Enemy":
            case "BossEnemy":
                HandleEnemyDamage(collider, damage, target.Tag);
                break;
            case "Security":
                HandleSecurityDamage(collider, damage);
                break;
            case "Player":
                HandlePlayerDamage(collider, damage);
                break;
                // Add more cases as needed
        }
    }

    void HandleEnemyDamage(Collider collider, int damage, string enemyType)
    {
        // Simplified enemy damage handling logic here
        FollowAI enemyDamageCrit = collider.GetComponent<FollowAI>();
        if (enemyDamageCrit.alive)
        {
            enemyDamageCrit.TakeDamage(damage);
            if (enemyDamageCrit.Health <= damage && playerHealth != null)
            {
                playerHealth.EnemyKilled(enemyType);
            }
        }
    }

    void HandleSecurityDamage(Collider collider, int damage)
    {
        // Simplified security damage handling logic here
        DroneHealth droneHealth = collider.GetComponent<DroneHealth>();
        if (droneHealth != null)
            droneHealth.TakeDamage(damage);
        else
        {
            SentryDrone sentry = collider.GetComponent<SentryDrone>();
            sentry.TakeDamage(damage);
        }
    }

    void HandlePlayerDamage(Collider collider, int damage)
    {
        // Simplified player damage handling logic here
        PlayerHealth playerHealthCrit = collider.GetComponent<PlayerHealth>();
        if (playerHealthCrit.alive && collider.transform.root.gameObject != player)
        {
            playerHealthCrit.TakeDamage(damage);
            if (playerHealthCrit.Health <= damage && playerHealth != null)
            {
                playerHealth.PlayersKilled();
            }
        }
    }

    int CalculateDamage(float distance)
    {
        return (int)((1f - distance / explosionRadius) * maxDamage);
    }

    IEnumerator Destroy(float delay)
    {
        objectRenderer.SetActive(false);
        yield return new WaitForSeconds(delay);
        PhotonNetwork.Destroy(gameObject);
    }
}
