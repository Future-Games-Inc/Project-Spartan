using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpBuy : MonoBehaviour
{
    public SkillTree skillTree;

    public int cost;

    public TMP_Text BodyText;

    public bool powerUpBought;

    private void Start()
    {
        powerUpBought = false;
    }

    void Update()
    {
        BodyText.text = $"Cost: {skillTree.saveData.SkillPoints}/{cost} Cs";
        GetComponent<Image>().color = skillTree.saveData.SkillPoints >= 1 ? Color.white : Color.grey;
        GetComponent<Image>().color = powerUpBought == false ? Color.white : Color.grey;
    }

    public void Buy()
    {
        if (skillTree.saveData.SkillPoints < cost || powerUpBought == true || skillTree.powerUpCount == 2)
            return;
        skillTree.saveData.UpdateSkills(-cost);
        powerUpBought = true;
        skillTree.powerUpCount ++;
    }
}
