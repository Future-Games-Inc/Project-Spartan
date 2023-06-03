using Photon.Pun;
using UnityEngine;

public class StingerBulletMiniNet : MonoBehaviourPunCallbacks
{
    [Header("Bullet Behavior ---------------------------------------------------")]
    public GameObject explosionPrefab;
    public GameObject bulletOwner;
    public PlayerHealth playerHealth;
    public bool playerBullet = false;

    private Transform target;
    private float lifetime;

    [Header("Bullet Effects ---------------------------------------------------")]
    public float explosionRadius = 2.0f;
    public float explosionForce = 15.0f;

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

    [System.Obsolete]
    private void OnTriggerEnter(Collider other)
    {
        if (playerBullet == true)
        {
            playerHealth = bulletOwner.GetComponent<PlayerHealth>();
        }
        else
        {
            playerHealth = null;
        }

        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("BossEnemy"))
        {
            FollowAI enemyDamageCrit = other.GetComponent<FollowAI>();
            if (enemyDamageCrit.Health <= (10) && enemyDamageCrit.alive == true && playerHealth != null)
            {
                playerHealth.EnemyKilled();
                enemyDamageCrit.TakeDamage(10);
            }
            else if (enemyDamageCrit.Health > (10) && enemyDamageCrit.alive == true && playerHealth != null)
            {
                enemyDamageCrit.TakeDamage(10);
            }
            Explode();
            return;
        }

        if (other.CompareTag("Security"))
        {
            //critical hit here
            DroneHealth enemyDamageCrit = other.GetComponent<DroneHealth>();
            enemyDamageCrit.TakeDamage(15);
            Explode();
            return;
        }

        if (other.CompareTag("Player") && other != bulletOwner)
        {
            //critical hit here
            PlayerHealth playerDamageCrit = other.GetComponent<PlayerHealth>();
            if (playerDamageCrit.Health <= (5) && playerDamageCrit.alive == true && playerHealth != null)
            {
                playerHealth.PlayersKilled();
            }
            playerDamageCrit.TakeDamage(5);
            Explode();
            return;
        }

        if (target == null)
        {
            Explode();
            return;
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

        PhotonNetwork.InstantiateRoomObject(explosionPrefab.name, transform.position, Quaternion.identity, 0, null);
        PhotonNetwork.Destroy(gameObject);
    }
}