using PathologicalGames;
using Umbrace.Unity.PurePool;
using UnityEngine;

public class StingerBulletMiniNet : MonoBehaviour
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

    private void OnTriggerEnter(Collider other)
    {
        if (playerBullet == true)
        {
            playerHealth = bulletOwner.GetComponentInParent<PlayerHealth>();
        }
        else
        {
            playerHealth = null;
        }

        if (other.gameObject.CompareTag("Enemy"))
        {
            FollowAI enemyDamageCrit = other.GetComponentInParent<FollowAI>();
            if (enemyDamageCrit != null)
            {
                if (enemyDamageCrit.Health <= (10) && enemyDamageCrit.alive == true && playerHealth != null)
                {
                    playerHealth.EnemyKilled("Normal");
                    enemyDamageCrit.TakeDamage(10);
                }

                else if (enemyDamageCrit.Health > (10) && enemyDamageCrit.alive == true && playerHealth != null)
                {
                    enemyDamageCrit.TakeDamage(10);
                }
            }
            Explode();
            return;
        }

        else if (other.gameObject.CompareTag("BossEnemy"))
        {
            FollowAI enemyDamageCrit = other.GetComponentInParent<FollowAI>();
            if (enemyDamageCrit != null)
            {
                if (enemyDamageCrit.Health <= (10) && enemyDamageCrit.alive == true && playerHealth != null)
                {
                    playerHealth.EnemyKilled("Boss");
                    enemyDamageCrit.TakeDamage(10);
                }

                else if (enemyDamageCrit.Health > (10) && enemyDamageCrit.alive == true && playerHealth != null)
                {
                    enemyDamageCrit.TakeDamage(10);
                }
            }
            Explode();
            return;
        }

        else if (other.CompareTag("Security"))
        {
            //critical hit here
            DroneHealth enemyDamageCrit = other.GetComponentInParent<DroneHealth>();
            if (enemyDamageCrit != null)
                enemyDamageCrit.TakeDamage(15);
            else
            {
                SentryDrone enemyDamageCrit2 = other.GetComponentInParent<SentryDrone>();
                if (enemyDamageCrit != null)
                    enemyDamageCrit2.TakeDamage(15);
            }
            Explode();
            return;
        }

        if (target == null)
        {
            Explode();
            return;
        }

        else if (other.CompareTag("Tower"))
        {
            if (playerBullet)
            {
                //critical hit here
                ReactorCover reactorcover = other.GetComponentInParent<ReactorCover>();
                if (reactorcover != null)
                    reactorcover.TakeDamage(10);
            }
        }
    }

    private void Explode()
    {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        this.PoolManager.Release(gameObject);
    }
}