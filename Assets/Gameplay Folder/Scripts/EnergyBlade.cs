using UnityEngine;

public class EnergyBlade : MonoBehaviour
{
    public int baseDamage = 10;
    public int bleedDamage = 2;
    public float bleedDuration = 10.0f;
    public int bleedIncrease = 2;
    public GameObject hitEffectPrefab;
    //public GameObject bladeObject;

    private bool isBleeding = false;
    private float bleedTimer = 0.0f;
    private int bleedStacks = 0;

    private void Start()
    {
        //bladeObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("BossEnemy"))
        {
            // Calculate damage and apply to enemy
            int damage = baseDamage + bleedStacks * bleedDamage;
            other.GetComponent<FiringRangeAI>().TakeDamage(damage);

            // Apply hit effect
            Instantiate(hitEffectPrefab, other.transform.position, Quaternion.identity);

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
                if (bleedStacks > 1)
                {
                    damage += (bleedStacks - 1) * bleedIncrease;
                }
            }
        }

        if (other.CompareTag("Player"))
        {
            // Calculate damage and apply to enemy
            int damage = baseDamage + bleedStacks * bleedDamage;
            other.GetComponent<PlayerHealth>().TakeDamage(damage);

            // Apply hit effect
            Instantiate(hitEffectPrefab, other.transform.position, Quaternion.identity);

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
                if (bleedStacks > 1)
                {
                    damage += (bleedStacks - 1) * bleedIncrease;
                }
            }
        }
    }

    private void Update()
    {
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
    }

    //public void ActivateBlade()
    //{
    //    // Play audio and visual effects on blade activation
    //    // ...

    //    // Show blade mesh
    //    bladeObject.SetActive(true);
    //}

    //public void DeactivateBlade()
    //{
    //    // Hide blade mesh
    //    bladeObject.SetActive(false);
    //}
}

