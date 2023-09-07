using UnityEngine;
using TMPro;

public class SkillUI : MonoBehaviour
{
    public TextMeshProUGUI skillText;
    public PlayerHealth playerHealth;

    private void Start()
    {
        
    }
    private void Update()
    {
        skillText.text = "Cints: " + playerHealth.playerCints.ToString();
    }
}
