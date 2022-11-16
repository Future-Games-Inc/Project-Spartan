using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillUI : MonoBehaviour
{
    public TextMeshProUGUI skillText;
    public SaveData saveData;

    private void Start()
    {
        saveData = GameObject.FindGameObjectWithTag("SaveData").GetComponent<SaveData>();  
    }
    private void Update()
    {
        skillText.text = "Skill Points: " + saveData.SkillPoints.ToString();
    }
}
