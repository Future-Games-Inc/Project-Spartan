using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
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

    void Start()
    {
        skillLevel = 0;
        UpdateLoadout();
    }

    void Update()
    {
        BodyText.text = $"Cost: {skillTree.saveData.SkillPoints}/{cost} Cs";
        GetComponent<Image>().color = skillTree.saveData.SkillPoints >= cost ? Color.white : Color.grey;
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
        if (tag == "bulletMod")
        {
            ExitGames.Client.Photon.Hashtable bulletModifierPurchase = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.BULLET_MODIFIER, skillLevel } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(bulletModifierPurchase);
        }

        else if (tag == "shieldDur")
        {
            ExitGames.Client.Photon.Hashtable shieldDurationPurchase = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.SHIELD_DURATION, skillLevel } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(shieldDurationPurchase);
        }

        else if (tag == "toxDam")
        {
            ExitGames.Client.Photon.Hashtable toxicDamagePurchase = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.TOXICITY_DAMAGE, skillLevel } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(toxicDamagePurchase);
        }

        else if (tag == "damTake")
        {

            ExitGames.Client.Photon.Hashtable damageTakenPurchase = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.DAMAGAE_TAKEN, skillLevel } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(damageTakenPurchase);
        }

        else if (tag == "reactExtra")
        {
            ExitGames.Client.Photon.Hashtable reactorExtractionPurchase = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.REACTOR_EXTRACTION, skillLevel } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(reactorExtractionPurchase);
        }

        else if (tag == "ammoOver")
        {
            ExitGames.Client.Photon.Hashtable ammoOverloadPurchase = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.AMMO_OVERLOAD, skillLevel } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(ammoOverloadPurchase);
        }

        else if (tag == "playSpeed")
        {
            ExitGames.Client.Photon.Hashtable playerSpeedPurchase = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.PLAYER_SPEED, skillLevel } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerSpeedPurchase);
        }

        else if (tag == "playHealth")
        {
            ExitGames.Client.Photon.Hashtable playerHealthPurchase = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.PLAYER_HEALTH, skillLevel } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerHealthPurchase);
        }

        else if (tag == "playDash")
        {
            ExitGames.Client.Photon.Hashtable playerDashPurchase = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.PLAYER_DASH, skillLevel } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerDashPurchase);
        }

        else if (tag == "healthPow")
        {
            ExitGames.Client.Photon.Hashtable healthPowerupPurchase = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.HEALTH_POWERUP, skillLevel } };
        }

        else if (tag == "dashCool")
        {
            ExitGames.Client.Photon.Hashtable dashCooldownPurchase = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.DASH_COOLDOWN, skillLevel } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(dashCooldownPurchase);
        }

        else if (tag == "healthRegen")
        {
            ExitGames.Client.Photon.Hashtable healthRegenPurchase = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.HEALTH_REGEN, skillLevel } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(healthRegenPurchase);
        }
    }
}

