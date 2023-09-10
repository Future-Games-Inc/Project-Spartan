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


    private void OnEnable()
    {
        

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
            FollowAI enemyDamageCrit = other.GetComponent<FollowAI>();
            if (enemyDamageCrit.Health <= (10) && enemyDamageCrit.alive == true && playerHealth != null)
            {
                playerHealth.EnemyKilled("Normal");
                enemyDamageCrit.TakeDamage(10);
            }

            else if (enemyDamageCrit.Health > (10) && enemyDamageCrit.alive == true && playerHealth != null)
            {
                enemyDamageCrit.TakeDamage(10);
            }
            Explode();
            return;
        }

        else if (other.gameObject.CompareTag("BossEnemy"))
        {
            FollowAI enemyDamageCrit = other.GetComponent<FollowAI>();
            if (enemyDamageCrit.Health <= (10) && enemyDamageCrit.alive == true && playerHealth != null)
            {
                playerHealth.EnemyKilled("Boss");
                enemyDamageCrit.TakeDamage(10);
            }

            else if (enemyDamageCrit.Health > (10) && enemyDamageCrit.alive == true && playerHealth != null)
            {
                enemyDamageCrit.TakeDamage(10);
            }
            Explode();
            return;
        }

        else if (other.CompareTag("Security"))
        {
            //critical hit here
            DroneHealth enemyDamageCrit = other.GetComponent<DroneHealth>();
            if (enemyDamageCrit != null)
                enemyDamageCrit.TakeDamage(15);
            else
            {
                SentryDrone enemyDamageCrit2 = other.GetComponent<SentryDrone>();
                enemyDamageCrit2.TakeDamage(15);
            }
            Explode();
            return;
        }

        else if (other.CompareTag("Player") && other.transform.root.gameObject != bulletOwner)
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

        else if (other.CompareTag("Tower"))
        {
            if (playerBullet)
            {
                //critical hit here
                ReactorCover reactorcover = other.GetComponentInParent<ReactorCover>();
                reactorcover.TakeDamage(10);
            }
        }
    }

    private void Explode()
    {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}