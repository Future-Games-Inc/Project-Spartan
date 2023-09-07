using PathologicalGames;
using System.Collections;
using Umbrace.Unity.PurePool;
using UnityEngine;

public class StingerBulletNet : MonoBehaviour
{
    [Header("Bullet Behavior ---------------------------------------------------")]
    public GameObject smallBulletPrefab;
    public GameObject explosionPrefab;
    public Transform spawnTransform;

    private bool hasExploded = false;

    public GameObject bulletOwner;
    public PlayerHealth playerHealth;
    public bool playerBullet = false;
    public int bulletModifier;

    [Header("Bullet Effects ---------------------------------------------------")]
    public float energyPulseRadius = 5.0f;
    public int numSmallBullets = 5;
    public float smallBulletTargetRadius = 10.0f;
    public float smallBulletLifetime = 5.0f;
    public AudioSource audioSource;
    public AudioClip clip;

    public GameObjectPoolManager PoolManager;


    private void OnEnable()
    {
        StartCoroutine(ExplodeBullets());
        PoolManager = GameObject.FindGameObjectWithTag("Pool").GetComponent<GameObjectPoolManager>();

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

    private void OnTriggerEnter(Collider other)
    {
        if (playerBullet == true)
        {
            playerHealth = bulletOwner.GetComponentInParent<PlayerHealth>();
            bulletModifier = playerHealth.bulletModifier;
        }
        else
        {
            playerHealth = null;
        }

        if (other.CompareTag("Enemy"))
        {
            float criticalChance = 30f;

            //cal it at random probability
            if (Random.Range(0, 100f) < criticalChance)
            {
                FollowAI enemyDamageCrit = other.GetComponent<FollowAI>();
                if (enemyDamageCrit.Health <= (30 * bulletModifier) && enemyDamageCrit.alive == true && playerHealth != null)
                {
                    playerHealth.EnemyKilled("Normal");
                }
                enemyDamageCrit.TakeDamage((30 * bulletModifier));
                Explode();
            }
            else
            {
                FollowAI enemyDamageCrit = other.GetComponent<FollowAI>();
                if (enemyDamageCrit.Health <= (20 * bulletModifier) && enemyDamageCrit.alive == true && playerHealth != null)
                {
                    playerHealth.EnemyKilled("Normal");
                }
                enemyDamageCrit.TakeDamage((20 * bulletModifier));
                Explode();
            }

        }

        if (other.CompareTag("BossEnemy"))
        {
            float criticalChance = 30f;

            //cal it at random probability
            if (Random.Range(0, 100f) < criticalChance)
            {
                FollowAI enemyDamageCrit = other.GetComponent<FollowAI>();
                if (enemyDamageCrit.Health <= (30 * bulletModifier) && enemyDamageCrit.alive == true && playerHealth != null)
                {
                    playerHealth.EnemyKilled("Boss");
                }
                enemyDamageCrit.TakeDamage((30 * bulletModifier));
                Explode();
            }
            else
            {
                FollowAI enemyDamageCrit = other.GetComponent<FollowAI>();
                if (enemyDamageCrit.Health <= (20 * bulletModifier) && enemyDamageCrit.alive == true && playerHealth != null)
                {
                    playerHealth.EnemyKilled("Boss");
                }
                enemyDamageCrit.TakeDamage((20 * bulletModifier));
                Explode();
            }

        }

        else if (other.CompareTag("Security"))
        {
            float criticalChance = 30f;

            //cal it at random probability
            if (Random.Range(0, 100f) < criticalChance)
            {
                //critical hit here
                DroneHealth enemyDamageCrit = other.GetComponent<DroneHealth>();
                if (enemyDamageCrit != null)
                    enemyDamageCrit.TakeDamage((40 * bulletModifier));
                else
                {
                    SentryDrone enemyDamageCrit2 = other.GetComponent<SentryDrone>();
                    enemyDamageCrit2.TakeDamage(40 * bulletModifier);
                }
                Explode();
            }

            else
            {
                DroneHealth enemyDamage = other.GetComponent<DroneHealth>();
                if (enemyDamage != null)
                    enemyDamage.TakeDamage((30 * bulletModifier));
                else
                {
                    SentryDrone enemyDamage2 = other.GetComponent<SentryDrone>();
                    enemyDamage2.TakeDamage(30 * bulletModifier);
                }
                Explode();
            }
        }

        else if (other.CompareTag("Player") && other.transform.root.gameObject != bulletOwner)
        {
            float criticalChance = 10f;

            if (Random.Range(0, 100f) < criticalChance)
            {
                //critical hit here
                PlayerHealth playerDamageCrit = other.GetComponent<PlayerHealth>();
                if (playerDamageCrit.Health <= (10 * bulletModifier) && playerDamageCrit.alive == true && playerHealth != null)
                {
                    playerHealth.PlayersKilled();
                }
                playerDamageCrit.TakeDamage((10 * bulletModifier));
                Explode();
            }

            else
            {
                PlayerHealth playerDamage = other.GetComponent<PlayerHealth>();
                if (playerDamage.Health <= (5 * bulletModifier) && playerDamage.alive == true && playerHealth != null)
                {
                    playerHealth.PlayersKilled();
                }
                playerDamage.TakeDamage((5 * bulletModifier));
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
        // Create smaller bullets and target nearby enemies
        for (int i = 0; i < numSmallBullets; i++)
        {
            GameObject smallBullet = this.PoolManager.Acquire(smallBulletPrefab, transform.position, Quaternion.identity);
            smallBullet.GetComponent<StingerBulletMiniNet>().audioSource.PlayOneShot(smallBullet.GetComponent<StingerBulletMiniNet>().clip);
            smallBullet.transform.forward = Random.insideUnitSphere;
            smallBullet.gameObject.GetComponent<StingerBulletMiniNet>().bulletOwner = bulletOwner.gameObject;
            smallBullet.gameObject.GetComponent<StingerBulletMiniNet>().playerBullet = true;
            Collider[] targets = Physics.OverlapSphere(transform.position, smallBulletTargetRadius);
            if (targets.Length > 0)
            {
                Transform target = targets[Random.Range(0, targets.Length)].transform;
                smallBullet.GetComponent<StingerBulletMiniNet>().SetTarget(target, smallBulletLifetime);
            }
        }

        // Create explosion effect and destroy bullet
        this.PoolManager.Acquire(explosionPrefab, transform.position, Quaternion.identity);
        this.PoolManager.Release(gameObject);
    }
}
