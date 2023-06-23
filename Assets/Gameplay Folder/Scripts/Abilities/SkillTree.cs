using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class SkillTree : MonoBehaviour
{

    public SkillTree skillTree;
    private void Awake() => skillTree = this;

    public int[] SkillLevels;
    public int[] SkillCaps;
    public int powerupButtonAssign;
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
        EXPText.text = "CINTS: " + saveData.SkillPoints.ToString();
        if (purchasedText != null)
            purchasedText.text = $"Purchased: {powerUpCount}/2 Implants";

    }
    // Start is called before the first frame update
    void Start()
    {
        initialLoad = true;
        powerUpCount = 0f;
        powerupButtonAssign = 0;

        SkillLevels = new int[6];
        SkillCaps = new[] { 7, 5, 5, 3, 3, 1, };

        foreach (var skill in SkillHolder.GetComponentsInChildren<Skill>())
            SkillList.Add(skill);

        for (var i = 0; i < SkillList.Count; i++)
            SkillList[i].id = i;

        SkillList[0].ConnectedSkills = new[] { 1, 2 };
        SkillList[1].ConnectedSkills = new[] { 3 };
        SkillList[2].ConnectedSkills = new[] { 4 };
        SkillList[3].ConnectedSkills = new[] { 5 };

        UpdateAllSkillsUI();
        if (saveData == null)
        {
            saveData = GameObject.FindGameObjectWithTag("SaveData").GetComponent<SaveData>();
        }
    }

    // Update is called once per frame
    public void UpdateAllSkillsUI()
    {
        foreach (var skill in SkillList)
            skill.UpdateUI();

        int playAudio = (int)Random.Range(0, 100);
        if (!audioSource.isPlaying && playAudio <= 70 && initialLoad == false)
            audioSource.PlayOneShot(audioClip[(int)Random.Range(0, audioClip.Length)]);
        initialLoad = false;
    }

    //public void SetPowerUp()
    //{
    //    ExitGames.Client.Photon.Hashtable buttonAssign = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.BUTTON_ASSIGN, powerupButtonAssign } };
    //    PhotonNetwork.LocalPlayer.SetCustomProperties(buttonAssign);
    //}
}
