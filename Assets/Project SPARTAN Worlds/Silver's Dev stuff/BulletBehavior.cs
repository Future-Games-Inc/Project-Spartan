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
    public GameObject BreakEffect;
    public bool AutoTracking = false;
    [Header("Audio Properties  -------------")]
    public bool PlayAudioOnSpawn = false;
    public AudioClip fireSound;
    public AudioClip hitSound;
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
        if (PlayAudioOnSpawn) audioSource.PlayOneShot(fireSound);
    }

    private void OnTriggerEnter(Collider other)
    {
        collisionAndTriggerCheck(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        collisionAndTriggerCheck(collision.collider);
    }

    private void collisionAndTriggerCheck(Collider other)
    {
        Debug.Log("hit:" + other.tag + " : " + other.name);
        // if break on impact
        if (!other.gameObject.CompareTag("Bullet") && BreakOnImpact == true) Destroy(gameObject);
        if ((other.CompareTag("Enemy") || other.CompareTag("BossEnemy")))
        {
            // select custom functions for damage
            switch (Type)
            {
                case "EMP Bullet":
                    EMPBulletDamage(other);
                    break;
                default:
                    DefaultDamage(other);
                    break;
            }
        }
        if (other.CompareTag("Player"))
        {
            // select custom functions for damage
            switch (Type)
            {
                case "EMP Bullet":
                    EMPBulletDamage(other);
                    break;
                default:
                    DefaultDamage(other);
                    break;
            }
        }
    }

    /// <summary> -------------------------------------------------------------------
    ///                           CUSTOME BULLET FUNCTIONS
    /// </summary> -------------------------------------------------------------------
    void DefaultDamage(Collider target)
    {
        FiringRangeAI enemyDamageReg = target.GetComponentInParent<FiringRangeAI>();
        enemyDamageReg.TakeDamage(bullet: this);
        Destroy(gameObject);
    }
    void EMPBulletDamage(Collider target)
    {
        FiringRangeAI enemyDamageReg = target.GetComponentInParent<FiringRangeAI>();
        enemyDamageReg.TakeDamage(bullet: this);
        enemyDamageReg.EMPShock(BreakEffect);
        Destroy(gameObject);
    }
}
