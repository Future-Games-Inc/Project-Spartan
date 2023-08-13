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

    private bool isBleeding = false;

    private float bleedTimer = 0.0f;
    public float bleedDuration = 5.0f;

    private int _bleedStacks = 0;
    public int bleedStacks
    {
        get
        {
            return _bleedStacks;
        }
        set
        {
            if (_bleedStacks != value)
            {
                _bleedStacks = value;
                UpdateBleedStatus();
            }
        }
    }

    void OnEnable()
    {
        bladeTransform = transform;
        previousPosition = bladeTransform.localPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (bladeVelocity < 0.2f)
        {
            Debug.Log("Not fast enough");
            return;  // Check if blade is not moving fast enough
        }
        int Damage = baseDamage + bleedStacks * bleedDamage;

        if (other.CompareTag("Enemy") || other.CompareTag("BossEnemy"))
        {
            FollowAI enemyDamageReg = other.GetComponent<FollowAI>();
            if (enemyDamageReg != null)
                HandleFollowAIDamage(Damage, enemyDamageReg, other);

            ApplyEffects(Damage, other.transform.position);
            Debug.Log("Hit");
        }

        else if(other.CompareTag("Player"))
        {
            PlayerHealth playerDamage = other.GetComponent<PlayerHealth>();
            if (playerDamage != null)
                HandlePlayerDamage(Damage, playerDamage);

            ApplyEffects(Damage, other.transform.position);
            Debug.Log("Hit");
        }

        else if (other.CompareTag("Security"))
        {
            DroneHealth droneHealth = other.GetComponent<DroneHealth>();
            if (droneHealth != null)
                droneHealth.TakeDamage(Damage);
            else
            {
                SentryDrone sentryDrone = other.GetComponent<SentryDrone>();
                if (sentryDrone != null)
                    sentryDrone.TakeDamage(Damage);
            }
            ApplyEffects(Damage, other.transform.position);
            Debug.Log("Hit");
        }
    }

    private void HandleFollowAIDamage(int Damage, FollowAI enemyDamageReg, Collider other)
    {
        if (playerHealth == null) return;

        if (enemyDamageReg.Health <= Damage && enemyDamageReg.alive)
        {
            string type = other.CompareTag("BossEnemy") ? "Boss" : "Normal";
            playerHealth.EnemyKilled(type);
        }
        else if (enemyDamageReg.Health > 10 && enemyDamageReg.alive)
        {
            enemyDamageReg.TakeDamage(Damage);
        }
    }

    private void HandlePlayerDamage(int Damage, PlayerHealth enemyDamageReg)
    {
        if (playerHealth == null || playerHealth.gameObject == enemyDamageReg.gameObject) return;

        if (enemyDamageReg.Health <= Damage && enemyDamageReg.alive)
            playerHealth.PlayersKilled();

        enemyDamageReg.TakeDamage(Damage);
    }

    private void ApplyEffects(int Damage, Vector3 position)
    {
        // Apply hit effect
        PhotonNetwork.InstantiateRoomObject(hitEffectPrefab.name, position, Quaternion.identity, 0, null);

        // Apply bleed effect
        if (!isBleeding)
        {
            bleedStacks++;
            bleedTimer = bleedDuration;
        }
        else
        {
            bleedStacks++;
            bleedTimer = bleedDuration;
            if (bleedStacks > 3)
                Damage += (bleedStacks - 1) * bleedIncrease;
        }
    }

    private void Update()
    {
        // Calculate blade velocity
        Vector3 displacement = bladeTransform.localPosition - previousPosition;
        bladeVelocity = displacement.magnitude / Time.deltaTime;
        previousPosition = bladeTransform.localPosition;

        if (bleedStacks > 3)
            isBleeding = true;

        // Update bleed effect timer
        if (isBleeding)
        {
            bleedTimer -= Time.deltaTime;
            if (bleedTimer <= 0.0f)
            {
                isBleeding = false;
                bleedStacks = 0; // This triggers UpdateBleedStatus via the property setter
            }
        }

        bleedStackText.text = bleedStacks.ToString();
    }

    private void UpdateBleedStatus()
    {
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

    public void rescale()
    {
        this.gameObject.transform.localScale = Vector3.one;
    }
}