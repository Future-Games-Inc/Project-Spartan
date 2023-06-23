using Photon.Pun;
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
        BodyText.text = $"Cost: {skillTree.saveData.SkillPoints}/{cost} Cs";
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

    [System.Obsolete]
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
        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();

        switch (tag)
        {
            case "bulletMod":
                properties[MultiplayerVRConstants.BULLET_MODIFIER] = skillLevel;
                break;

            case "armorLevel":
                properties[MultiplayerVRConstants.PLAYER_ARMOR] = skillLevel;
                break;

            case "toxDam":
                properties[MultiplayerVRConstants.TOXICITY_DAMAGE] = skillLevel;
                break;

            case "damTake":
                properties[MultiplayerVRConstants.DAMAGAE_TAKEN] = skillLevel;
                break;

            case "reactExtra":
                properties[MultiplayerVRConstants.REACTOR_EXTRACTION] = skillLevel;
                break;

            case "ammoOver":
                properties[MultiplayerVRConstants.AMMO_OVERLOAD] = skillLevel;
                break;

            case "playSpeed":
                properties[MultiplayerVRConstants.PLAYER_SPEED] = skillLevel;
                break;

            case "playHealth":
                properties[MultiplayerVRConstants.PLAYER_HEALTH] = skillLevel;
                break;

            case "playDash":
                properties[MultiplayerVRConstants.PLAYER_DASH] = skillLevel;
                break;

            case "healthPow":
                properties[MultiplayerVRConstants.HEALTH_POWERUP] = skillLevel;
                break;

            case "dashCool":
                properties[MultiplayerVRConstants.DASH_COOLDOWN] = skillLevel;
                break;

            case "healthRegen":
                properties[MultiplayerVRConstants.HEALTH_REGEN] = skillLevel;
                break;

            default:
                break;
        }

        PhotonNetwork.LocalPlayer.SetCustomProperties(properties);
    }
}

