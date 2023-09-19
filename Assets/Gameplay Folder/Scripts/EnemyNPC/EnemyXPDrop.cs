using UnityEngine;
using BNG;
using Umbrace.Unity.PurePool;
using System.Collections;

public class EnemyXPDrop : MonoBehaviour
{
    public Pickup pickupData; // Reference to the Pickup ScriptableObject
    public SpawnManager1 spawnManager;
    public LayerMask groundLayer;
    private Rigidbody rb;
    public Grabbable grabbable;

    public bool contact;


    // Start is called before the first frame update
    void OnEnable()
    {

        spawnManager = GameObject.FindGameObjectWithTag("spawnManager").GetComponent<SpawnManager1>();
        switch (pickupData.pickupType)
        {
            case "XP":
                rb = GetComponent<Rigidbody>();
                // Freeze X and Z initially
                rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
                break;
        }
        StartCoroutine(NoContact());
    }

    IEnumerator NoContact()
    {
        yield return new WaitForSeconds(10);
        if (contact == false)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LeftHand") || other.CompareTag("RightHand"))
        {
            contact = true;
            PlayerHealth playerHealth = other.gameObject.GetComponentInParent<PlayerHealth>();
            switch (pickupData.pickupType)
            {
                case "XP":
                    float xpDrop = 10f;
                    if (Random.Range(0, 100f) < xpDrop)
                    {
                        playerHealth.UpdateSkills(pickupData.xpAmount);
                    }
                    else
                    {
                        playerHealth.UpdateSkills(pickupData.xpAmount / 2);
                    }
                    Destroy(gameObject);
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
                Destroy(gameObject);
                break;

            case "EMP":
                Collider[] colliders = Physics.OverlapSphere(transform.position, 10f);
                foreach (Collider collider in colliders)
                {
                    if (collider.CompareTag("Security"))
                    {
                        DroneHealth enemyDamageCrit = collider.GetComponent<DroneHealth>();
                        enemyDamageCrit.TakeDamage(200);
                    }
                    if (collider.CompareTag("Enemy") || collider.CompareTag("BossEnemy"))
                    {
                        FollowAI enemyDamageCrit = collider.GetComponent<FollowAI>();
                        enemyDamageCrit.TakeDamage(75);
                        enemyDamageCrit.EMPShock();
                    }
                }
                Destroy(gameObject);
                break;

            case "toxicDrop":
                playerHealth.Toxicity(pickupData.toxicAmount);
                Destroy(gameObject);
                break;

            case "bulletModifier":
                playerHealth.BulletImprove(pickupData.bulletModifierDamage, pickupData.bulletModifierCount);
                Destroy(gameObject);
                break;

            case "Shield":
                playerHealth.AddArmor(pickupData.armorAmount);
                Destroy(gameObject);
                break;

            default:
                Destroy(gameObject);
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

    public void rescale()
    {
        this.gameObject.transform.localScale = Vector3.one;
    }
}