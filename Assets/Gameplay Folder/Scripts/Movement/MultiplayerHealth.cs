using UnityEngine;
using UnityEngine.UI;

public class MultiplayerHealth : MonoBehaviour
{
    public Slider healthSlider;
    public Slider armorSlider;

    public void SetMaxHealth(int maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
    }

    public void SetCurrentHealth(int currentHealth)
    {
        healthSlider.value = currentHealth;
    }

    public void SetMaxArmor(int maxArmor)
    {
        armorSlider.maxValue = maxArmor;
        armorSlider.value = maxArmor;
    }

    public void SetCurrentArmor(int currentArmor)
    {
        armorSlider.value = currentArmor;
    }
}
