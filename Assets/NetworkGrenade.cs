using UnityEngine;
using Photon.Pun;
using System.Collections;

public class NetworkGrenade : MonoBehaviourPunCallbacks
{
    public enum GrenadeType
    {
        Prox,
        Smoke
    }

    public GrenadeType Type;

    [System.Serializable]
    public struct TargetInfo
    {
        public string Tag;
    }

    public TargetInfo[] Targets;

    public float explosionRadius = 5f;
    public int maxDamage = 80;
    public float explosionDelay = 5f;

    public GameObject player;
    public PlayerHealth playerHealth;
    public GameObject explosionEffect;
    public Rigidbody rb;

    public float throwForce;
    public float throwUpwardForce;

    public AudioSource audioSource;

    public GameObject renderer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LeftHand") || other.CompareTag("RightHand"))
        {
            player = other.transform.root.gameObject;
        }
    }

    public void Throw()
    {
        if (!photonView.IsMine) return; // Ensure only the owner can throw

        Camera cam = player.GetComponentInChildren<Camera>();
        Vector3 forceToAdd = cam.transform.forward * throwForce * throwUpwardForce;
        rb.AddForce(forceToAdd);
        photonView.RPC("GrenadeSound", RpcTarget.All);
        StartCoroutine(ExplodeDelayed());
    }

    public IEnumerator ExplodeDelayed()
    {
        yield return new WaitForSeconds(explosionDelay);
        photonView.RPC("Explode", RpcTarget.All);
    }

    [PunRPC]
    void GrenadeSound()
    {
        audioSource.Play();
    }

    [PunRPC]
    void Explode()
    {
        if (!photonView.IsMine) return; // Ensure only the owner can explode

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

        // Based on the tag, handle damage appropriately. This is a simplified version, extend as needed.
        switch (target.Tag)
        {
            case "Enemy":
                // Handle enemy damage
                FollowAI enemyDamageCrit = collider.GetComponent<FollowAI>();
                if (enemyDamageCrit.Health <= damage && enemyDamageCrit.alive == true && playerHealth != null)
                {
                    playerHealth.EnemyKilled("Normal");
                    enemyDamageCrit.TakeDamage(damage);
                }
                else if (enemyDamageCrit.Health > damage && enemyDamageCrit.alive == true && playerHealth != null)
                {
                    enemyDamageCrit.TakeDamage(damage);
                }
                break;
            case "BossEnemy":
                // Handle boss enemy damage
                FollowAI BossenemyDamageCrit = collider.GetComponent<FollowAI>();
                if (BossenemyDamageCrit.Health <= damage && BossenemyDamageCrit.alive == true && playerHealth != null)
                {
                    playerHealth.EnemyKilled("BossEnemy");
                    BossenemyDamageCrit.TakeDamage(damage);
                }
                else if (BossenemyDamageCrit.Health > damage && BossenemyDamageCrit.alive == true && playerHealth != null)
                {
                    BossenemyDamageCrit.TakeDamage(damage);
                }
                break;
            case "Security":
                // Handle security damage
                DroneHealth DroneenemyDamageCrit = collider.GetComponent<DroneHealth>();
                if (DroneenemyDamageCrit != null)
                    DroneenemyDamageCrit.TakeDamage(damage);
                else
                {
                    SentryDrone SentryenemyDamageCrit2 = collider.GetComponent<SentryDrone>();
                    SentryenemyDamageCrit2.TakeDamage(damage);
                }
                break;
            case "Player":
                // Handle player damage
                PlayerHealth PlayerenemyDamageCrit = collider.GetComponent<PlayerHealth>();
                if (PlayerenemyDamageCrit.Health <= damage && PlayerenemyDamageCrit.alive == true && playerHealth != null && collider.transform.root.gameObject != player)
                {
                    playerHealth.PlayersKilled();
                    PlayerenemyDamageCrit.TakeDamage(damage);
                }
                else if (PlayerenemyDamageCrit.Health > damage && PlayerenemyDamageCrit.alive == true && playerHealth != null)
                {
                    PlayerenemyDamageCrit.TakeDamage(damage);
                }
                break;
                // Add more cases as needed
        }
    }

    int CalculateDamage(float distance)
    {
        return (int)((1f - distance / explosionRadius) * maxDamage);
    }

    IEnumerator Destroy(float delay)
    {
        renderer.SetActive(false);
        yield return new WaitForSeconds(delay);
        PhotonNetwork.Destroy(gameObject);
    }
}