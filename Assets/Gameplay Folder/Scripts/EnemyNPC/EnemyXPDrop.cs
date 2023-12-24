using UnityEngine;
using BNG;
using System.Collections;
using Umbrace.Unity.PurePool;

public class EnemyXPDrop : MonoBehaviour
{
    public Pickup pickupData; // Reference to the Pickup ScriptableObject
    public SpawnManager1 spawnManager;
    public LayerMask groundLayer;
    private Rigidbody rb;
    public Grabbable grabbable;
    public GameObject buffText;
    public AudioSource audioSource;
    public AudioClip pickupClip;

    public bool contact;
    public MatchEffects matchEffects;
    public bool active = true;

    public GameObjectPoolManager PoolManager;

    // Start is called before the first frame update
    void OnEnable()
    {

        spawnManager = GameObject.FindGameObjectWithTag("spawnManager").GetComponent<SpawnManager1>();
        matchEffects = GameObject.FindGameObjectWithTag("Props").GetComponent<MatchEffects>();
        switch (pickupData.pickupType)
        {
            case "XP":
                rb = GetComponent<Rigidbody>();
                // Freeze X and Z initially
                rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
                break;
        }
        StartCoroutine(NoContact());
        if (this.PoolManager == null)
        {
            this.PoolManager = Object.FindObjectOfType<GameObjectPoolManager>();
        }

    }

    IEnumerator NoContact()
    {
        yield return new WaitForSeconds(10);
        if (contact == false)
        {
            this.PoolManager.Release(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LeftHand") || other.CompareTag("RightHand") || other.CompareTag("Player"))
        {
            contact = true;
            PlayerHealth playerHealth = other.gameObject.GetComponentInParent<PlayerHealth>();
            switch (pickupData.pickupType)
            {
                case "XP":
                    if (active)
                    {
                        active = false;
                        audioSource.PlayOneShot(pickupClip);
                        float xpDrop = 10f;
                        if (Random.Range(0, 100f) < xpDrop)
                        {
                            if (playerHealth.faction == "CintSix Cartel")
                                playerHealth.UpdateSkills(pickupData.xpAmount + 5);
                            else
                                playerHealth.UpdateSkills(pickupData.xpAmount);
                        }
                        else
                        {
                            if (playerHealth.faction == "CintSix Cartel")
                                playerHealth.UpdateSkills(pickupData.xpAmount/2 + 5);
                            else
                                playerHealth.UpdateSkills(pickupData.xpAmount/2);
                        }
                        StartCoroutine(DelayDestroy());
                    }
                    break;
            }
        }

        if (other.CompareTag("PickupSlot"))
        {
            PlayerHealth playerHealth = other.gameObject.GetComponentInParent<PlayerHealth>();
            switch (pickupData.pickupType)
            {

                case "Health":
                    spawnManager.UpdateHealthCount();
                    playerHealth.AddHealth(pickupData.healthAmount);
                    this.PoolManager.Release(gameObject);
                    break;

                case "EMP":
                    Collider[] colliders = Physics.OverlapSphere(transform.position, 20f);
                    foreach (Collider collider in colliders)
                    {
                        if (collider.CompareTag("Security"))
                        {
                            DroneHealth enemyDamageCrit = collider.GetComponentInParent<DroneHealth>();
                            enemyDamageCrit.TakeDamage(200);
                        }
                        if (collider.CompareTag("Enemy") || collider.CompareTag("BossEnemy"))
                        {
                            FollowAI enemyDamageCrit = collider.GetComponentInParent<FollowAI>();
                            enemyDamageCrit.TakeDamage(30);
                            enemyDamageCrit.EMPShock();
                        }
                    }
                    this.PoolManager.Release(gameObject);
                    break;

                case "toxicDrop":
                    playerHealth.Toxicity(pickupData.toxicAmount);
                    this.PoolManager.Release(gameObject);
                    break;

                case "bulletModifier":
                    playerHealth.BulletImprove(pickupData.bulletModifierDamage, pickupData.bulletModifierCount);
                    this.PoolManager.Release(gameObject);
                    break;

                case "Shield":
                    playerHealth.AddArmor(pickupData.armorAmount);
                    this.PoolManager.Release(gameObject);
                    break;

                case "CUAHack":
                    matchEffects.AddTime(30);
                    this.PoolManager.Release(gameObject);
                    break;

                default:
                    this.PoolManager.Release(gameObject);
                    break;
            }
        }
    }

    void Update()
    {
        switch (pickupData.pickupType)
        {
            case "XP":
                CheckGroundDistance();
                break;
        }
    }

    public void FreezeConstraints()
    {
        CheckGroundDistance();
    }

    private void CheckGroundDistance()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, groundLayer))
        {
            if (hit.distance < 0.5f && !grabbable.BeingHeld)
            {
                // When the object is less than 0.2m from the ground, unfreeze X and Z
                rb.constraints &= ~RigidbodyConstraints.FreezePositionX;
                rb.constraints &= ~RigidbodyConstraints.FreezePositionZ;
            }
            else
            {
                rb.constraints &= RigidbodyConstraints.FreezePositionX;
                rb.constraints &= RigidbodyConstraints.FreezePositionZ;
            }
        }
    }

    public void EnableText()
    {
        buffText.SetActive(true);
    }

    public void EDisableText()
    {
        buffText.SetActive(false);
    }

    public void rescale()
    {
        this.gameObject.transform.localScale = Vector3.one;
    }

    IEnumerator DelayDestroy()
    {
        yield return new WaitForSeconds(0.5f);
        this.PoolManager.Release(gameObject);
    }
}