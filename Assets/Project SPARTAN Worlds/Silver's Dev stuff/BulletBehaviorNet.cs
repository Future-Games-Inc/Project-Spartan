using Photon.Pun;
using System.Collections;
using UnityEngine;

public class BulletBehaviorNet : MonoBehaviourPunCallbacks
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
    private AudioSource audioSource;

    // Start is called before the first frame update
    private void OnEnable()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        StartCoroutine(Destroy(Duration));
    }
    IEnumerator Destroy(float duration)
    {
        yield return new WaitForSeconds(duration);
        PhotonNetwork.Destroy(gameObject);
        // attach clip and play
        // audioSource.PlayOneShot(clip);
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

        // if break on impact
        if (!other.gameObject.CompareTag("Bullet") && BreakOnImpact == true)
            PhotonNetwork.Destroy(gameObject);
        if ((other.CompareTag("Enemy") || other.CompareTag("BossEnemy")))
        {
            // select custom functions for damage
            switch (Type)
            {
                case "EMP Bullet":
                    EMPBulletDamageEnemy(other, Damage);
                    break;
                default:
                    DefaultDamageEnemy(other, Damage);
                    break;
            }
        }

        if (other.CompareTag("Player"))
        {
            // select custom functions for damage
            switch (Type)
            {
                case "EMP Bullet":
                    EMPBulletDamagePlayer(other, Damage);
                    break;
                default:
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
                default:
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
                playerHealth.EnemyKilled();
                enemyDamageReg.TakeDamage(Damage);
            }
            else if (enemyDamageReg.Health > (10) && enemyDamageReg.alive == true && playerHealth != null)
            {
                enemyDamageReg.TakeDamage(Damage);
            }
            PhotonNetwork.Destroy(gameObject);
        }
        void EMPBulletDamageEnemy(Collider target, float damage)
        {
            FollowAI enemyDamageReg = target.GetComponent<FollowAI>();
            if (enemyDamageReg.Health <= (Damage) && enemyDamageReg.alive == true && playerHealth != null)
            {
                playerHealth.EnemyKilled();
                enemyDamageReg.TakeDamage(Damage);
            }
            else if (enemyDamageReg.Health > (10) && enemyDamageReg.alive == true && playerHealth != null)
            {
                enemyDamageReg.TakeDamage(Damage);
            }
            enemyDamageReg.EMPShock();
            PhotonNetwork.Destroy(gameObject);
        }

        void DefaultDamagePlayer(Collider target, float damage)
        {
            PlayerHealth enemyDamageReg = target.GetComponent<PlayerHealth>();
            if (enemyDamageReg.Health <= (Damage) && enemyDamageReg.alive == true && playerHealth != null)
            {
                playerHealth.PlayersKilled();
            }
            enemyDamageReg.TakeDamage(Damage);
            PhotonNetwork.Destroy(gameObject);
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
            PhotonNetwork.Destroy(gameObject);
        }

        void DefaultDamageSecurity(Collider target, float damage)
        {
            DroneHealth enemyDamageReg = target.GetComponent<DroneHealth>();
            enemyDamageReg.TakeDamage(Damage);
            PhotonNetwork.Destroy(gameObject);
        }
        void EMPBulletDamageSecurity(Collider target, float damage)
        {
            DroneHealth enemyDamageReg = target.GetComponent<DroneHealth>();
            enemyDamageReg.TakeDamage(Damage * 200);
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // still need to modify to allow bullets pass through enemies if we want to do that later on
        if (!collision.gameObject.CompareTag("Bullet") && BreakOnImpact == true)
            PhotonNetwork.Destroy(gameObject);
    }

}
