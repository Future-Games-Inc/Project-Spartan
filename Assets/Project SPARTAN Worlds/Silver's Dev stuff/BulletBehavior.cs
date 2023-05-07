using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    [Header("Bullet Properties -------------")]
    [Tooltip("The name that will ID this bullet for it's own separate homing/damage/extra features functions")]
    public string Type = "Default";
    public int Damage;
    public float RateOfFire;
    public float TravelSpeed;
    public float Duration;
    [Header("Other Properties -------------")]
    public bool BreakOnImpact = true;
    public bool BreakOnEnemyImpact = true;
    public bool AutoTracking = false;
    [Header("Audio Properties  -------------")]
    public AudioClip clip;
    private AudioSource audioSource;
    // Start is called before the first frame update
    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }
    void Start()
    {
        Destroy(gameObject, Duration);
        // attach clip and play
        // audioSource.PlayOneShot(clip);
    }

    private void OnTriggerEnter(Collider other)
    {
        // if break on impact
        if (!other.gameObject.CompareTag("Bullet") && BreakOnImpact == true) Destroy(gameObject);
        if ((other.CompareTag("Enemy") || other.CompareTag("BossEnemy")))
        {
            // select custom functions for damage
            switch (Type)
            {
                case "EMP Bullet":
                    EMPBulletDamage(other, Damage);
                    break;
                default:
                    DefaultDamage(other, Damage);
                    break;
            }
        }
        if (other.CompareTag("Player"))
        {
            // select custom functions for damage
            switch (Type)
            {
                case "EMP Bullet":
                    EMPBulletDamage(other, Damage);
                    break;
                default:
                    DefaultDamage(other, Damage);
                    break;
            }
        }
        /// <summary> -------------------------------------------------------------------
        ///                           CUSTOME BULLET FUNCTIONS
        /// </summary> -------------------------------------------------------------------
        void DefaultDamage(Collider target, float damage)
        {
            FiringRangeAI enemyDamageReg = target.GetComponent<FiringRangeAI>();
            enemyDamageReg.TakeDamage(Damage);
            Destroy(gameObject);
        }
        void EMPBulletDamage(Collider target, float damage)
        {
            FiringRangeAI enemyDamageReg = target.GetComponent<FiringRangeAI>();
            enemyDamageReg.TakeDamage(Damage);
            enemyDamageReg.EMPShock();
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // still need to modify to allow bullets pass through enemies if we want to do that later on
        if (!collision.gameObject.CompareTag("Bullet") && BreakOnImpact == true) Destroy(gameObject);
    }

}
