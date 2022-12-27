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
    //public string[] SkillNames;
    //public string[] SkillDescriptions;

    public List<Skill> SkillList;
    public GameObject SkillHolder;

    public SaveData saveData;
    public TMP_Text EXPText;
    public TMP_Text purchasedText;

    public float powerUpCount = 0f;

    public AudioSource audioSource;
    public AudioClip[] audioClip;
    public bool initialLoad;

    private void Update()
    {
        if (saveData == null)
        {
            saveData = GameObject.FindGameObjectWithTag("SaveData").GetComponent<SaveData>();
        }

        EXPText.text = "CINTS: " + saveData.SkillPoints.ToString();
        purchasedText.text = $"Purchased: {powerUpCount}/2 Implants";

    }
    // Start is called before the first frame update
    void Start()
    {
        initialLoad = true;
        powerUpCount = 0f;

        SkillLevels = new int[6];
        SkillCaps = new[] { 7, 5, 5, 3, 3, 1, };

        foreach (var skill in SkillHolder.GetComponentsInChildren<Skill>()) 
            SkillList.Add(skill);

        for (var i = 0; i < SkillList.Count; i++)
            SkillList[i].id = i;

        SkillList[0].ConnectedSkills = new[] { 1, 2 };
        SkillList[1].ConnectedSkills = new[] { 3};
        SkillList[2].ConnectedSkills = new[] { 4};
        SkillList[3].ConnectedSkills = new[] { 5 };

        UpdateAllSkillsUI();
        if(saveData == null)
        {
            saveData = GameObject.FindGameObjectWithTag("SaveData").GetComponent<SaveData>();
        }
    }

    // Update is called once per frame
    public void UpdateAllSkillsUI()
    {
        foreach(var skill in SkillList)  
            skill.UpdateUI();

        int playAudio = Random.Range(0, 100);
        if (!audioSource.isPlaying && playAudio <= 50 && initialLoad == false)
            audioSource.PlayOneShot(audioClip[Random.Range(0, audioClip.Length)]);
        initialLoad = false;
    }
}
