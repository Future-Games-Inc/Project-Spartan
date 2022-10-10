using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Skill : MonoBehaviour
{
    public SkillTree skillTree;

    public int id;

    public TMP_Text TitleText;
    public TMP_Text BodyText;

    public int[] ConnectedSkills;



    void Update()
    {
        BodyText.text = $"Cost: {skillTree.saveData.SkillPoints}/1 SP";
        GetComponent<Image>().color = skillTree.saveData.SkillPoints >= 1 ? Color.white : Color.grey;
    }
    public void UpdateUI()
    {
        TitleText.text = $"{skillTree.SkillLevels[id]}/{skillTree.SkillCaps[id]}";

        foreach (var connectedSkill in ConnectedSkills)
        {
            skillTree.SkillList[connectedSkill].gameObject.SetActive(skillTree.SkillLevels[id] > 3);
        }
    }

    public void Buy()
    {
        if (skillTree.saveData.SkillPoints < 1 || skillTree.SkillLevels[id] >= skillTree.SkillCaps[id])
            return;
        skillTree.saveData.UpdateSkills(-1);
        skillTree.SkillLevels[id]++;
        skillTree.UpdateAllSkillsUI();
    }
}

