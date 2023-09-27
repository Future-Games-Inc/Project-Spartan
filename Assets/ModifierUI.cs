using TMPro;
using UnityEngine;

public class ModifierUI : MonoBehaviour
{
    public TextMeshProUGUI modifierText;
    public PlayerHealth playerHealth;
    public int BulletModifier;

    private void Start()
    {
        modifierText.text = "Bullet Modifier: " + playerHealth.bulletModifier.ToString();
    }
    private void Update()
    {
        if (BulletModifier != playerHealth.bulletModifier)
        {
            modifierText.text = "Bullet Modifier: " + playerHealth.bulletModifier.ToString();
            BulletModifier = playerHealth.bulletModifier;
        }
    }
}


