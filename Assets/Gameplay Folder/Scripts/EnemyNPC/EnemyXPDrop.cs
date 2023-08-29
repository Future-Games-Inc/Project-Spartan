using UnityEngine;
using Photon.Pun;
using BNG;

public class EnemyXPDrop : MonoBehaviourPunCallbacks
{
    public Pickup pickupData; // Reference to the Pickup ScriptableObject
    public SpawnManager1 spawnManager;
    public LayerMask groundLayer;
    private Rigidbody rb;
    public NetworkedGrabbable grabbable;

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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PickupSlot"))
        {
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
                    PhotonNetwork.Destroy(gameObject);
                    break;

                case "Health":
                    spawnManager.photonView.RPC("RPC_UpdateHealthCount", RpcTarget.All);
                    playerHealth.AddHealth(pickupData.healthAmount);
                    PhotonNetwork.Destroy(gameObject);
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
                    PhotonNetwork.Destroy(gameObject);
                    break;

                case "toxicDrop":
                    playerHealth.Toxicity(pickupData.toxicAmount);
                    PhotonNetwork.Destroy(gameObject);
                    break;

                case "bulletModifier":
                    playerHealth.BulletImprove(pickupData.bulletModifierDamage, pickupData.bulletModifierCount);
                    PhotonNetwork.Destroy(gameObject);
                    break;

                case "Shield":
                    playerHealth.AddArmor(pickupData.armorAmount);
                    PhotonNetwork.Destroy(gameObject);
                    break;

                default:
                    PhotonNetwork.Destroy(gameObject);
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