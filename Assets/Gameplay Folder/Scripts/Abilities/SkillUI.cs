using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
