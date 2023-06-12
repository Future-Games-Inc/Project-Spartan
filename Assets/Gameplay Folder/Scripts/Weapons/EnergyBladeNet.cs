using Photon.Pun;
using TMPro;
using UnityEngine;

public class EnergyBladeNet : MonoBehaviourPunCallbacks
{
    [Header("Blade Characteristics ---------------------------------------------------------------")]
    public TextMeshProUGUI bleedStackText;

    public GameObject bleedIcon;
    public GameObject hitEffectPrefab;
    public GameObject Blade;
    public PlayerHealth playerHealth;

    private Transform bladeTransform;

    private Vector3 previousPosition;

    private float bladeVelocity;

    public Material normal;
    public Material bleed;

    [Header("Blade Effects ---------------------------------------------------------------------")]
    public int baseDamage = 10;
    public int bleedDamage = 2;
    public int bleedIncrease = 2;
    private int bleedStacks = 0;

    private bool isBleeding = false;

    private float bleedTimer = 0.0f;
    public float bleedDuration = 5.0f;

    void OnEnable()
    {
        if (photonView.IsMine)
        {
            bladeTransform = transform;
            previousPosition = bladeTransform.localPosition;
        }
    }

    [System.Obsolete]
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (bladeVelocity >= 3f) // Check if blade is moving fast enough
            {
                // Calculate damage and apply to enemy
                int Damage = baseDamage + bleedStacks * bleedDamage;
                FollowAI enemyDamageReg = other.GetComponent<FollowAI>();
                if (enemyDamageReg.Health <= (Damage) && enemyDamageReg.alive == true && playerHealth != null)
                {
                    playerHealth.EnemyKilled("Normal");
                    enemyDamageReg.TakeDamage(Damage);
                }
                else if (enemyDamageReg.Health > (10) && enemyDamageReg.alive == true && playerHealth != null)
                {
                    enemyDamageReg.TakeDamage(Damage);
                }
                // Apply hit effect
                PhotonNetwork.InstantiateRoomObject(hitEffectPrefab.name, other.transform.position, Quaternion.identity, 0, null);

                // Apply bleed effect
                if (!isBleeding)
                {
                    isBleeding = true;
                    bleedStacks = 1;
                    bleedTimer = bleedDuration;
                }
                else
                {
                    bleedStacks++;
                    bleedTimer = bleedDuration;
                    if (bleedStacks > 3)
                    {
                        Damage += (bleedStacks - 1) * bleedIncrease;
                    }
                }
            }
        }

        else if (other.CompareTag("BossEnemy"))
        {
            if (bladeVelocity >= 3f) // Check if blade is moving fast enough
            {
                // Calculate damage and apply to enemy
                int Damage = baseDamage + bleedStacks * bleedDamage;
                FollowAI enemyDamageReg = other.GetComponent<FollowAI>();
                if (enemyDamageReg.Health <= (Damage) && enemyDamageReg.alive == true && playerHealth != null)
                {
                    playerHealth.EnemyKilled("Boss");
                    enemyDamageReg.TakeDamage(Damage);
                }
                else if (enemyDamageReg.Health > (10) && enemyDamageReg.alive == true && playerHealth != null)
                {
                    enemyDamageReg.TakeDamage(Damage);
                }
                // Apply hit effect
                PhotonNetwork.InstantiateRoomObject(hitEffectPrefab.name, other.transform.position, Quaternion.identity, 0, null);

                // Apply bleed effect
                if (!isBleeding)
                {
                    isBleeding = true;
                    bleedStacks = 1;
                    bleedTimer = bleedDuration;
                }
                else
                {
                    bleedStacks++;
                    bleedTimer = bleedDuration;
                    if (bleedStacks > 3)
                    {
                        Damage += (bleedStacks - 1) * bleedIncrease;
                    }
                }
            }
        }

        else if (other.CompareTag("Player"))
        {
            if (bladeVelocity >= 3f) // Check if blade is moving fast enough
            {
                // Calculate damage and apply to enemy
                int Damage = baseDamage + bleedStacks * bleedDamage;
                PlayerHealth enemyDamageReg = other.GetComponent<PlayerHealth>();
                if (enemyDamageReg.Health <= (Damage) && enemyDamageReg.alive == true && playerHealth != null)
                {
                    playerHealth.PlayersKilled();
                }
                enemyDamageReg.TakeDamage(Damage);

                // Apply hit effect
                PhotonNetwork.InstantiateRoomObject(hitEffectPrefab.name, other.transform.position, Quaternion.identity, 0, null);

                // Apply bleed effect
                if (!isBleeding)
                {
                    isBleeding = true;
                    bleedStacks = 1;
                    bleedTimer = bleedDuration;
                }
                else
                {
                    bleedStacks++;
                    bleedTimer = bleedDuration;
                    if (bleedStacks > 3)
                    {
                        Damage += (bleedStacks - 1) * bleedIncrease;
                    }
                }
            }
        }

        else if (other.CompareTag("Security"))
        {
            if (bladeVelocity >= 3f) // Check if blade is moving fast enough
            {
                // Calculate damage and apply to enemy
                int Damage = baseDamage + bleedStacks * bleedDamage;
                DroneHealth enemyDamageReg = other.GetComponent<DroneHealth>();
                if (enemyDamageReg != null)
                    enemyDamageReg.TakeDamage(Damage);
                else
                {
                    SentryDrone enemyDamageReg2 = other.GetComponent<SentryDrone>();
                    enemyDamageReg2.TakeDamage(Damage);
                }

                // Apply hit effect
                PhotonNetwork.InstantiateRoomObject(hitEffectPrefab.name, other.transform.position, Quaternion.identity, 0, null);

                // Apply bleed effect
                if (!isBleeding)
                {
                    isBleeding = true;
                    bleedStacks = 1;
                    bleedTimer = bleedDuration;
                }
                else
                {
                    bleedStacks++;
                    bleedTimer = bleedDuration;
                    if (bleedStacks > 3)
                    {
                        Damage += (bleedStacks - 1) * bleedIncrease;
                    }
                }
            }
        }
    }

    private void Update()
    {
        // Calculate blade velocity
        Vector3 displacement = bladeTransform.localPosition - previousPosition;
        bladeVelocity = displacement.magnitude / Time.deltaTime;
        previousPosition = bladeTransform.localPosition;

        // Update bleed effect timer
        if (isBleeding)
        {
            bleedTimer -= Time.deltaTime;
            if (bleedTimer <= 0.0f)
            {
                isBleeding = false;
                bleedStacks = 0;
            }
        }

        if (bleedStacks > 3)
        {
            bleedIcon.SetActive(true);
            photonView.RPC("RPC_BladeBleeding", RpcTarget.All);
        }
        else
        {
            bleedIcon.SetActive(false);
            photonView.RPC("RPC_BladeNormal", RpcTarget.All);
        }

        bleedStackText.text = bleedStacks.ToString();
    }

    [PunRPC]
    void RPC_BladeBleeding()
    {
        Blade.GetComponent<Renderer>().material = bleed;
    }

    [PunRPC]
    void RPC_BladeNormal()
    {
        Blade.GetComponent<Renderer>().material = normal;
    }
}

