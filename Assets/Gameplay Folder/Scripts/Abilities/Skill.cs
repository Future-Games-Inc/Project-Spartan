using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Skill : MonoBehaviour
{
    public SkillTree skillTree;

    public int id;
    public int skillLevel;

    public int cost;

    public TMP_Text TitleText;
    public TMP_Text BodyText;

    public int[] ConnectedSkills;

    private Image image;

    void Start()
    {
        skillLevel = 0;
        UpdateLoadout();

        image = GetComponent<Image>();
    }

    void Update()
    {
        BodyText.text = $"{cost} Cints";
        image.color = skillTree.saveData.SkillPoints >= cost ? Color.white : Color.grey;
    }
    public void UpdateUI()
    {
        TitleText.text = $"{skillTree.SkillLevels[id]}/{skillTree.SkillCaps[id]}";

        foreach (var connectedSkill in ConnectedSkills)
        {
            skillTree.SkillList[connectedSkill].gameObject.SetActive(skillTree.SkillLevels[id] > 2);
        }
    }

    public void Buy()
    {
        if (skillTree.saveData.SkillPoints < cost || skillTree.SkillLevels[id] >= skillTree.SkillCaps[id])
            return;
        skillTree.saveData.UpdateSkills(-cost);
        skillTree.SkillLevels[id]++;
        skillLevel++;
        skillTree.UpdateAllSkillsUI();
        UpdateLoadout();
    }

    public void UpdateLoadout()
    {
        switch (tag)
        {
            case "bulletMod":
                PlayerPrefs.SetInt("BULLET_MODIFIER",  skillLevel);
                break;

            case "armorLevel":
                PlayerPrefs.SetInt("PLAYER_ARMOR", skillLevel);
                break;

            case "toxDam":
                PlayerPrefs.SetInt("TOXICITY_DAMAGE", skillLevel);
                break;

            case "damTake":
                PlayerPrefs.SetInt("DAMAGAE_TAKEN", skillLevel);
                break;

            case "reactExtra":
                PlayerPrefs.SetInt("REACTOR_EXTRACTION", skillLevel);
                break;

            case "ammoOver":
                PlayerPrefs.SetInt("AMMO_OVERLOAD", skillLevel);
                break;

            case "playSpeed":
                PlayerPrefs.SetInt("PLAYER_SPEED", skillLevel);
                break;

            case "playHealth":
                PlayerPrefs.SetInt("PLAYER_HEALTH", skillLevel);
                break;

            case "playDash":
                PlayerPrefs.SetInt("PLAYER_DASH", skillLevel);
                break;

            case "healthPow":
                PlayerPrefs.SetInt("HEALTH_POWERUP", skillLevel);
                break;

            case "dashCool":
                PlayerPrefs.SetInt("DASH_COOLDOWN", skillLevel);
                break;

            case "healthRegen":
                PlayerPrefs.SetInt("HEALTH_REGEN", skillLevel);
                break;

            default:
                break;
        }
    }
}

