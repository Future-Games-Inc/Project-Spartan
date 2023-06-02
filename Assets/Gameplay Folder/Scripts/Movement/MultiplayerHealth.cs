using UnityEngine;
using UnityEngine.UI;

public class MultiplayerHealth : MonoBehaviour
{
    public Slider slider;
    public void SetMaxHealth(int maxHealth)
    {
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
    }

    public void SetCurrentHealth(int currentHealth)
    {
        slider.value = currentHealth;
    }
}
