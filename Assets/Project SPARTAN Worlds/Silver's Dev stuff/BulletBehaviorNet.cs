using PathologicalGames;
using System.Collections;
using Umbrace.Unity.PurePool;
using UnityEngine;

public class BulletBehaviorNet : MonoBehaviour
{
    [Header("Bullet Properties -------------")]
    [Tooltip("The name that will ID this bullet for it's own separate homing/damage/extra features functions")]
    public string Type = "Default";
    public int Damage;
    public float RateOfFire;
    public float TravelSpeed;
    public float Duration;
    public GameObject bulletOwner;
    public PlayerHealth playerHealth;
    public bool playerBullet = false;

    [Header("Other Properties -------------")]
    public bool BreakOnImpact = true;
    public bool BreakOnEnemyImpact = true;
    public bool AutoTracking = false;

    [Header("Audio Properties  -------------")]
    public AudioClip clip;
    public AudioSource audioSource;



    // Start is called before the first frame update
    private void OnEnable()
    {

        

        StartCoroutine(Destroy(Duration));
    }
    IEnumerator Destroy(float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
        // attach clip and play
        // audioSource.PlayOneShot(clip);
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

        // if break on impact
        if (!other.gameObject.CompareTag("Bullet") && BreakOnImpact == true)
            Destroy(gameObject);
        if ((other.CompareTag("Enemy")))
        {
            // select custom functions for damage
            switch (Type)
            {
                case "EMP Bullet":
                    EMPBulletDamageEnemy(other, Damage);
                    break;
                case "Default":
                    DefaultDamageEnemy(other, Damage);
                    break;
            }
        }

        if ((other.CompareTag("BossEnemy")))
        {
            // select custom functions for damage
            switch (Type)
            {
                case "EMP Bullet":
                    EMPBulletDamageBossEnemy(other, Damage);
                    break;
                case "Default":
                    DefaultDamageBossEnemy(other, Damage);
                    break;
            }
        }

        if (other.CompareTag("Player") && other.transform.root.gameObject != bulletOwner)
        {
            // select custom functions for damage
            switch (Type)
            {
                case "EMP Bullet":
                    EMPBulletDamagePlayer(other, Damage);
                    break;
                case "Default":
                    DefaultDamagePlayer(other, Damage);
                    break;
            }
        }

        if (other.CompareTag("Security"))
        {
            // select custom functions for damage
            switch (Type)
            {
                case "EMP Bullet":
                    EMPBulletDamageSecurity(other, Damage);
                    break;
                case "Default":
                    DefaultDamageSecurity(other, Damage);
                    break;
            }
        }
        /// <summary> -------------------------------------------------------------------
        ///                           CUSTOME BULLET FUNCTIONS
        /// </summary> -------------------------------------------------------------------
        void DefaultDamageEnemy(Collider target, float damage)
        {
            FollowAI enemyDamageReg = target.GetComponent<FollowAI>();
            if (enemyDamageReg.Health <= (Damage) && enemyDamageReg.alive == true && playerHealth != null)
            {
                playerHealth.EnemyKilled("Normal");
                enemyDamageReg.TakeDamage(Damage);
            }
            else if (enemyDamageReg.Health > (10) && enemyDamageReg.alive == true && playerHealth != null)
            {
                enemyDamageReg.TakeDamage(Damage);
            }
            Destroy(gameObject);
        }
        void EMPBulletDamageEnemy(Collider target, float damage)
        {
            FollowAI enemyDamageReg = target.GetComponent<FollowAI>();
            if (enemyDamageReg.Health <= (Damage) && enemyDamageReg.alive == true && playerHealth != null)
            {
                playerHealth.EnemyKilled("Normal");
                enemyDamageReg.TakeDamage(Damage);
            }
            else if (enemyDamageReg.Health > (10) && enemyDamageReg.alive == true && playerHealth != null)
            {
                enemyDamageReg.TakeDamage(Damage);
            }
            enemyDamageReg.EMPShock();
            Destroy(gameObject);
        }

        void DefaultDamageBossEnemy(Collider target, float damage)
        {
            FollowAI enemyDamageReg = target.GetComponent<FollowAI>();
            if (enemyDamageReg.Health <= (Damage) && enemyDamageReg.alive == true && playerHealth != null)
            {
                playerHealth.EnemyKilled("Boss");
                enemyDamageReg.TakeDamage(Damage);
            }
            else if (enemyDamageReg.Health > (10) && enemyDamageReg.alive == true && playerHealth != null)
            {
                enemyDamageReg.TakeDamage(Damage);
            }
            Destroy(gameObject);
        }
        void EMPBulletDamageBossEnemy(Collider target, float damage)
        {
            FollowAI enemyDamageReg = target.GetComponent<FollowAI>();
            if (enemyDamageReg.Health <= (Damage) && enemyDamageReg.alive == true && playerHealth != null)
            {
                playerHealth.EnemyKilled("Boss");
                enemyDamageReg.TakeDamage(Damage);
            }
            else if (enemyDamageReg.Health > (10) && enemyDamageReg.alive == true && playerHealth != null)
            {
                enemyDamageReg.TakeDamage(Damage);
            }
            enemyDamageReg.EMPShock();
            Destroy(gameObject);
        }

        void DefaultDamagePlayer(Collider target, float damage)
        {
            PlayerHealth enemyDamageReg = target.GetComponent<PlayerHealth>();
            if (enemyDamageReg.Health <= (Damage) && enemyDamageReg.alive == true && playerHealth != null)
            {
                playerHealth.PlayersKilled();
            }
            enemyDamageReg.TakeDamage(Damage);
            Destroy(gameObject);
        }
        void EMPBulletDamagePlayer(Collider target, float damage)
        {
            PlayerHealth enemyDamageReg = target.GetComponent<PlayerHealth>();
            if (enemyDamageReg.Health <= (Damage) && enemyDamageReg.alive == true && playerHealth != null)
            {
                playerHealth.PlayersKilled();
            }
            enemyDamageReg.TakeDamage(Damage);
            enemyDamageReg.EMPShock();
            Destroy(gameObject);
        }

        void DefaultDamageSecurity(Collider target, float damage)
        {
            DroneHealth enemyDamageReg = target.GetComponent<DroneHealth>();
            if (enemyDamageReg != null)
                enemyDamageReg.TakeDamage(Damage);
            else
            {
                SentryDrone enemyDamageReg2 = other.GetComponent<SentryDrone>();
                enemyDamageReg2.TakeDamage(Damage);
            }
            Destroy(gameObject);
        }
        void EMPBulletDamageSecurity(Collider target, float damage)
        {
            DroneHealth enemyDamageReg = target.GetComponent<DroneHealth>();
            if (enemyDamageReg != null)
                enemyDamageReg.TakeDamage(Damage);
            else
            {
                SentryDrone enemyDamageReg2 = other.GetComponent<SentryDrone>();
                enemyDamageReg2.TakeDamage(Damage * 200);
            }
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // still need to modify to allow bullets pass through enemies if we want to do that later on
        if (!collision.gameObject.CompareTag("Bullet") && BreakOnImpact == true)
            Destroy(gameObject);
    }

}
