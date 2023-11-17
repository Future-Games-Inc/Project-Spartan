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
        BodyText.text = $"{cost} Cints";
        GetComponent<Image>().color = skillTree.saveData.SkillPoints >= cost ? Color.white : Color.grey;
        GetComponent<Image>().color = powerUpBought == false ? Color.white : Color.grey;
    }

    public void Buy()
    {
        if (skillTree.saveData.SkillPoints < cost || powerUpBought == true || skillTree.powerUpCount == 2)
            return;
        skillTree.saveData.UpdateSkills(-cost);
        powerUpBought = true;
        skillTree.powerUpCount++;

        UpdatePowerups();
    }

    public void UpdatePowerups()
    {
        if (tag == "healthStim")
        {
            PlayerPrefs.SetInt("HEALTH_STIM", 1);
            PlayerPrefs.SetInt("HEALTH_STIM_SLOT", (int)skillTree.powerUpCount);
            PlayerPrefs.SetInt("BUTTON_ASSIGN", (int)skillTree.powerUpCount);
        }

        else if (tag == "leech")
        {
            PlayerPrefs.SetInt("LEECH", 1);
            PlayerPrefs.SetInt("LEECH_SLOT", (int)skillTree.powerUpCount);
            PlayerPrefs.SetInt("BUTTON_ASSIGN", (int)skillTree.powerUpCount);
        }

        else if (tag == "savingGrace")
        {
            PlayerPrefs.SetInt("SAVING_GRACE", 1);
            PlayerPrefs.SetInt("SAVING_GRACE_SLOT", (int)skillTree.powerUpCount);
            PlayerPrefs.SetInt("BUTTON_ASSIGN", (int)skillTree.powerUpCount);
        }

        else if (tag == "activeCamo")
        {
            PlayerPrefs.SetInt("ACTIVE_CAMO", 1);
            PlayerPrefs.SetInt("ACTIVE_CAMO_SLOT", (int)skillTree.powerUpCount);
            PlayerPrefs.SetInt("BUTTON_ASSIGN", (int)skillTree.powerUpCount);
        }

        else if (tag == "stealth")
        {
            PlayerPrefs.SetInt("STEALTH", 1);
            PlayerPrefs.SetInt("STEALTH_SLOT", (int)skillTree.powerUpCount);
            PlayerPrefs.SetInt("BUTTON_ASSIGN", (int)skillTree.powerUpCount);
        }

        //else if (tag == "doubleAgent")
        //{
        //    ExitGames.Client.Photon.Hashtable doublePurchase = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.DOUBLE_AGENT, Convert.ToInt32(powerUpBought) } };
        //    PhotonNetwork.LocalPlayer.SetCustomProperties(doublePurchase);

        //    ExitGames.Client.Photon.Hashtable doubleSlotPurchase = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.DOUBLE_AGENT_SLOT, powerupButtonAssign } };
        //    PhotonNetwork.LocalPlayer.SetCustomProperties(doubleSlotPurchase);
        //}

        //else if (tag == "proxBomb")
        //{
        //    ExitGames.Client.Photon.Hashtable proxPurchase = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.PROXIMITY_BOMB, Convert.ToInt32(powerUpBought) } };
        //    PhotonNetwork.LocalPlayer.SetCustomProperties(proxPurchase);

        //    ExitGames.Client.Photon.Hashtable proxSlotPurchase = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.PROXIMITY_BOMB_SLOT, powerupButtonAssign } };
        //    PhotonNetwork.LocalPlayer.SetCustomProperties(proxSlotPurchase);
        //}

        //else if (tag == "smokeBomb")
        //{
        //    ExitGames.Client.Photon.Hashtable smokePurchase = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.SMOKE_BOMB, Convert.ToInt32(powerUpBought) } };
        //    PhotonNetwork.LocalPlayer.SetCustomProperties(smokePurchase);

        //    ExitGames.Client.Photon.Hashtable smokeSlotPurchase = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.SMOKE_BOMB_SLOT, powerupButtonAssign } };
        //    PhotonNetwork.LocalPlayer.SetCustomProperties(smokeSlotPurchase);
        //}

        else if (tag == "exploBomb")
        {
            PlayerPrefs.SetInt("EXPLOSIVE_DEATH", 1);
            PlayerPrefs.SetInt("EXPLOSIVE_DEATH_SLOT", (int)skillTree.powerUpCount);
            PlayerPrefs.SetInt("BUTTON_ASSIGN", (int)skillTree.powerUpCount);
        }

        else if (tag == "BerserkFury")
        {
            PlayerPrefs.SetInt("BERSERKER_FURY", 1);
            PlayerPrefs.SetInt("BERSERKER_FURY_SLOT", (int)skillTree.powerUpCount);
            PlayerPrefs.SetInt("BUTTON_ASSIGN", (int)skillTree.powerUpCount);
        }

        else if (tag == "aiComp")
        {
            PlayerPrefs.SetInt("AI_COMPANION", 1);
            PlayerPrefs.SetInt("AI_COMPANION_SLOT", (int)skillTree.powerUpCount);
            PlayerPrefs.SetInt("BUTTON_ASSIGN", (int)skillTree.powerUpCount);
        }

        else if (tag == "decoyDeplo")
        {
            PlayerPrefs.SetInt("DECOY_DEPLOYMENT", 1);
            PlayerPrefs.SetInt("DECOY_DEPLOYMENT_SLOT", (int)skillTree.powerUpCount);
            PlayerPrefs.SetInt("BUTTON_ASSIGN", (int)skillTree.powerUpCount);
        }
    }
}
