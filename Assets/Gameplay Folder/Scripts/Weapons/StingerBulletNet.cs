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
        // Find the manager if one hasn't been specified.
        if (this.PoolManager == null)
        {
            this.PoolManager = Object.FindObjectOfType<GameObjectPoolManager>();
        }
        StartCoroutine(ExplodeBullets());
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
            playerHealth = bulletOwner.GetComponent<PlayerHealth>();
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
                FollowAI enemyDamageCrit = other.GetComponentInParent<FollowAI>();
                if (enemyDamageCrit != null)
                {
                    if (enemyDamageCrit.Health <= (15 * bulletModifier) && enemyDamageCrit.alive == true && playerHealth != null)
                    {
                        playerHealth.EnemyKilled("Normal");
                    }
                    enemyDamageCrit.TakeDamage((15 * bulletModifier));
                }
            }
            else
            {
                FollowAI enemyDamageCrit = other.GetComponentInParent<FollowAI>();
                if (enemyDamageCrit != null)
                {
                    if (enemyDamageCrit.Health <= (10 * bulletModifier) && enemyDamageCrit.alive == true && playerHealth != null)
                    {
                        playerHealth.EnemyKilled("Normal");
                    }
                    enemyDamageCrit.TakeDamage((10 * bulletModifier));
                }
            }
            Explode();
        }

        if (other.CompareTag("BossEnemy"))
        {
            float criticalChance = 30f;

            //cal it at random probability
            if (Random.Range(0, 100f) < criticalChance)
            {
                FollowAI enemyDamageCrit = other.GetComponentInParent<FollowAI>();
                if (enemyDamageCrit != null)
                {
                    if (enemyDamageCrit.Health <= (10 * bulletModifier) && enemyDamageCrit.alive == true && playerHealth != null)
                    {
                        playerHealth.EnemyKilled("Boss");
                    }
                    enemyDamageCrit.TakeDamage((10 * bulletModifier));
                }
            }
            else
            {
                FollowAI enemyDamageCrit = other.GetComponentInParent<FollowAI>();
                if (enemyDamageCrit != null)
                {
                    if (enemyDamageCrit.Health <= (5 * bulletModifier) && enemyDamageCrit.alive == true && playerHealth != null)
                    {
                        playerHealth.EnemyKilled("Boss");
                    }
                    enemyDamageCrit.TakeDamage((5 * bulletModifier));
                }
            }
            Explode();
        }

        else if (other.CompareTag("Security"))
        {
            float criticalChance = 30f;

            //cal it at random probability
            if (Random.Range(0, 100f) < criticalChance)
            {
                //critical hit here
                DroneHealth enemyDamageCrit = other.GetComponentInParent<DroneHealth>();
                if (enemyDamageCrit != null)
                    enemyDamageCrit.TakeDamage((15 * bulletModifier));
                else
                {
                    SentryDrone enemyDamageCrit2 = other.GetComponent<SentryDrone>();
                    if (enemyDamageCrit2 != null)
                        enemyDamageCrit2.TakeDamage(15 * bulletModifier);
                }
                Explode();
            }

            else
            {
                DroneHealth enemyDamage = other.GetComponentInParent<DroneHealth>();
                if (enemyDamage != null)
                    enemyDamage.TakeDamage((10 * bulletModifier));
                else
                {
                    SentryDrone enemyDamage2 = other.GetComponentInParent<SentryDrone>();
                    if (enemyDamage2 != null)
                        enemyDamage2.TakeDamage(10 * bulletModifier);
                }
                Explode();
            }
            Explode();
        }

        else if (other.CompareTag("Tower"))
        {
            if (playerBullet)
            {
                //critical hit here
                ReactorCover reactorcover = other.GetComponentInParent<ReactorCover>();
                if (reactorcover != null)
                    reactorcover.TakeDamage(10 * bulletModifier);
            }
            Explode();
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
            Collider[] targets = Physics.OverlapSphere(transform.position, smallBulletTargetRadius);
            if (targets.Length > 0)
            {
                GameObject smallBullet = this.PoolManager.Acquire(smallBulletPrefab, transform.position, Quaternion.identity);
                smallBullet.GetComponent<StingerBulletMiniNet>().audioSource.PlayOneShot(smallBullet.GetComponent<StingerBulletMiniNet>().clip);
                smallBullet.transform.forward = Random.insideUnitSphere;
                smallBullet.gameObject.GetComponent<StingerBulletMiniNet>().bulletOwner = bulletOwner.gameObject;
                smallBullet.gameObject.GetComponent<StingerBulletMiniNet>().playerBullet = true;
                Transform target = targets[Random.Range(0, targets.Length)].transform;
                smallBullet.GetComponent<StingerBulletMiniNet>().SetTarget(target, smallBulletLifetime);
            }
        }

        // Create explosion effect and destroy bullet
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        this.PoolManager.Release(gameObject);
    }
}
