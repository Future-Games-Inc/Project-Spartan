using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using TMPro;

public class SkillTree : MonoBehaviour
{

    public SkillTree skillTree;
    private void Awake() => skillTree = this;

    public int[] SkillLevels;
    public int[] SkillCaps;
    public string[] SkillNames;
    public string[] SkilLDescriptions;

    public List<Skill> SkillList;
    public GameObject SkillHolder;

    public SaveData saveData;
    public TMP_Text EXPText;

    private void Update()
    {
        EXPText.text = "EXP: " + saveData.SkillPoints.ToString();  
    }
    // Start is called before the first frame update
    void Start()
    {
        SkillLevels = new int[6];
        SkillCaps = new[] { 10, 7, 7, 5, 5, 4, };

        foreach (var skill in SkillHolder.GetComponentsInChildren<Skill>()) 
            SkillList.Add(skill);

        for (var i = 0; i < SkillList.Count; i++)
            SkillList[i].id = i;

        SkillList[0].ConnectedSkills = new[] { 1, 2 };
        SkillList[1].ConnectedSkills = new[] { 4};
        SkillList[2].ConnectedSkills = new[] { 3};
        SkillList[4].ConnectedSkills = new[] { 5 };

        UpdateAllSkillsUI();

    }

    // Update is called once per frame
    public void UpdateAllSkillsUI()
    {
        foreach(var skill in SkillList)  
            skill.UpdateUI();
    }
}
