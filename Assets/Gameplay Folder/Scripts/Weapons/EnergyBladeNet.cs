using PathologicalGames;
using TMPro;
using Umbrace.Unity.PurePool;
using UnityEngine;

public class EnergyBladeNet : MonoBehaviour
{
    [Header("Blade Characteristics ---------------------------------------------------------------")]
    public TextMeshProUGUI bleedStackText;

    public GameObject bleedIcon;
    public GameObject hitEffectPrefab;
    public GameObject Blade;
    public PlayerHealth playerHealth;

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

    public GameObjectPoolManager PoolManager;
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
        PoolManager = GameObject.FindGameObjectWithTag("Pool").GetComponent<GameObjectPoolManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        int Damage = baseDamage + bleedStacks * bleedDamage;

        if (other.CompareTag("Enemy") || other.CompareTag("BossEnemy"))
        {
            FollowAI enemyDamageReg = other.GetComponent<FollowAI>();
            if (enemyDamageReg != null)
                HandleFollowAIDamage(Damage, enemyDamageReg, other);

            ApplyEffects(Damage, other.transform.position);
        }

        else if(other.CompareTag("Player"))
        {
            PlayerHealth playerDamage = other.GetComponent<PlayerHealth>();
            if (playerDamage != null)
                HandlePlayerDamage(Damage, playerDamage);

            ApplyEffects(Damage, other.transform.position);
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
        GameObject hit = this.PoolManager.Acquire(hitEffectPrefab, position, Quaternion.identity);

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
            RPC_BladeMaterial(bleed);
        }
        else
        {
            bleedIcon.SetActive(false);
            RPC_BladeMaterial(normal);
        }
    }

    void RPC_BladeMaterial(Material material)
    {
        Blade.GetComponent<Renderer>().material = material;
    }

    public void rescale()
    {
        this.transform.localScale = Vector3.one;
    }
}