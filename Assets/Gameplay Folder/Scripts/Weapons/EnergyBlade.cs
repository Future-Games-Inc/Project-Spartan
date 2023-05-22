using TMPro;
using UnityEngine;

public class EnergyBlade : MonoBehaviour
{
    public int baseDamage = 10;
    public int bleedDamage = 2;
    public float bleedDuration = 5.0f;
    public int bleedIncrease = 2;
    public GameObject hitEffectPrefab;
    //public GameObject bladeObject;

    private bool isBleeding = false;
    private float bleedTimer = 0.0f;
    private int bleedStacks = 0;

    public TextMeshProUGUI bleedStackText;
    public GameObject bleedIcon;

    private Transform bladeTransform;
    private Vector3 previousPosition;
    private float bladeVelocity;

    public Material normal;
    public Material bleed;

    private void Start()
    {
        bladeTransform = transform;
        previousPosition = bladeTransform.localPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("BossEnemy"))
        {
            if (bladeVelocity >= 3f) // Check if blade is moving fast enough
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
                    if (bleedStacks > 3)
                    {
                        damage += (bleedStacks - 1) * bleedIncrease;
                    }
                }
            }
        }

        if (other.CompareTag("Player"))
        {
            if (bladeVelocity >= 3f) // Check if blade is moving fast enough
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
                    if (bleedStacks > 3)
                    {
                        damage += (bleedStacks - 1) * bleedIncrease;
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
            this.GetComponent<Renderer>().material = bleed;
        }
        else
        {
            bleedIcon.SetActive(false);
            this.GetComponent<Renderer>().material = normal;
        }

        bleedStackText.text = bleedStacks.ToString();
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

