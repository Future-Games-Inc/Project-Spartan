using UnityEngine;
using UnityEngine.UI;
public class PlayerHealthBar : MonoBehaviour
{
    public Slider[] slider;

    public void SetMaxHealth(int maxHealth)
    {
        foreach (Slider healthBar in slider)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = maxHealth;
        }
    }

    public void SetCurrentHealth(int currentHealth)
    {
        foreach (Slider healthBar in slider)
        {
            healthBar.value = currentHealth;
        }
    }
}
