using UnityEngine;
using Photon.Pun;

public class EnemyXPDrop : MonoBehaviourPunCallbacks
{
    public Pickup pickupData; // Reference to the Pickup ScriptableObject
    public SpawnManager1 spawnManager;
    // Start is called before the first frame update
    void OnEnable()
    {
        spawnManager = GameObject.FindGameObjectWithTag("spawnManager").GetComponent<SpawnManager1>();
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
                    playerHealth.toxicEffectActive = true;
                    playerHealth.Toxicity(pickupData.toxicAmount);
                    PhotonNetwork.Destroy(gameObject);
                    break;

                case "bulletModifier":
                    playerHealth.BulletImprove(pickupData.bulletModifierDamage, pickupData.bulletModifierCount);
                    playerHealth.bulletImproved = true;
                    PhotonNetwork.Destroy(gameObject);
                    break;

                case "Shield":
                    playerHealth.shieldActive = true;
                    playerHealth.Shield(pickupData.shieldDuration);
                    PhotonNetwork.Destroy(gameObject);
                    break;

                default:
                    PhotonNetwork.Destroy(gameObject);
                    break;
            }
        }
    }
}