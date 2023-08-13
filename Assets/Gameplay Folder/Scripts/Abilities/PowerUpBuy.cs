using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;

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
        BodyText.text = $"{cost} Cs";
        GetComponent<Image>().color = skillTree.saveData.SkillPoints >= cost ? Color.white : Color.grey;
        GetComponent<Image>().color = powerUpBought == false ? Color.white : Color.grey;
    }

    [System.Obsolete]
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
            ExitGames.Client.Photon.Hashtable healthStimPurchase = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.HEALTH_STIM, Convert.ToInt32(powerUpBought) } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(healthStimPurchase);

            ExitGames.Client.Photon.Hashtable healthStimSlotPurchase = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.HEALTH_STIM_SLOT, (int)skillTree.powerUpCount } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(healthStimSlotPurchase);

            ExitGames.Client.Photon.Hashtable buttonAssign = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.BUTTON_ASSIGN, (int)skillTree.powerUpCount } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(buttonAssign);
        }

        else if (tag == "leech")
        {
            ExitGames.Client.Photon.Hashtable leechPurchase = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.LEECH, Convert.ToInt32(powerUpBought) } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(leechPurchase);

            ExitGames.Client.Photon.Hashtable leechSlotPurchase = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.LEECH_SLOT, (int)skillTree.powerUpCount } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(leechSlotPurchase);

            ExitGames.Client.Photon.Hashtable buttonAssign = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.BUTTON_ASSIGN, (int)skillTree.powerUpCount } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(buttonAssign);
        }

        else if (tag == "savingGrace")
        {
            ExitGames.Client.Photon.Hashtable savingPurchase = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.SAVING_GRACE, Convert.ToInt32(powerUpBought) } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(savingPurchase);

            ExitGames.Client.Photon.Hashtable savingSlotPurchase = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.SAVING_GRACE_SLOT, (int)skillTree.powerUpCount } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(savingSlotPurchase);

            ExitGames.Client.Photon.Hashtable buttonAssign = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.BUTTON_ASSIGN, (int)skillTree.powerUpCount } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(buttonAssign);
        }

        else if (tag == "activeCamo")
        {
            ExitGames.Client.Photon.Hashtable activeCamoPurchase = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.ACTIVE_CAMO, Convert.ToInt32(powerUpBought) } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(activeCamoPurchase);

            ExitGames.Client.Photon.Hashtable activeCamoSlotPurchase = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.ACTIVE_CAMO_SLOT, (int)skillTree.powerUpCount } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(activeCamoSlotPurchase);

            ExitGames.Client.Photon.Hashtable buttonAssign = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.BUTTON_ASSIGN, (int)skillTree.powerUpCount } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(buttonAssign);
        }

        else if (tag == "stealth")
        {
            ExitGames.Client.Photon.Hashtable stealthPurchase = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.STEALTH, Convert.ToInt32(powerUpBought) } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(stealthPurchase);

            ExitGames.Client.Photon.Hashtable stealthSlotPurchase = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.STEALTH_SLOT, (int)skillTree.powerUpCount } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(stealthSlotPurchase);

            ExitGames.Client.Photon.Hashtable buttonAssign = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.BUTTON_ASSIGN, (int)skillTree.powerUpCount } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(buttonAssign);
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
            ExitGames.Client.Photon.Hashtable exploPurchase = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.EXPLOSIVE_DEATH, Convert.ToInt32(powerUpBought) } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(exploPurchase);

            ExitGames.Client.Photon.Hashtable exploSlotPurchase = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.EXPLOSIVE_DEATH_SLOT, (int)skillTree.powerUpCount } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(exploSlotPurchase);

            ExitGames.Client.Photon.Hashtable buttonAssign = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.BUTTON_ASSIGN, (int)skillTree.powerUpCount } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(buttonAssign);
        }

        else if (tag == "BerserkFury")
        {
            ExitGames.Client.Photon.Hashtable berserkPurchase = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.BERSERKER_FURY, Convert.ToInt32(powerUpBought) } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(berserkPurchase);

            ExitGames.Client.Photon.Hashtable berserkSlotPurchase = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.BERSERKER_FURY_SLOT, (int)skillTree.powerUpCount } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(berserkSlotPurchase);

            ExitGames.Client.Photon.Hashtable buttonAssign = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.BUTTON_ASSIGN, (int)skillTree.powerUpCount } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(buttonAssign);
        }

        else if (tag == "aiComp")
        {
            ExitGames.Client.Photon.Hashtable aiPurchase = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.AI_COMPANION, Convert.ToInt32(powerUpBought) } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(aiPurchase);

            ExitGames.Client.Photon.Hashtable aiSlotPurchase = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.AI_COMPANION_SLOT, (int)skillTree.powerUpCount } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(aiSlotPurchase);

            ExitGames.Client.Photon.Hashtable buttonAssign = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.BUTTON_ASSIGN, (int)skillTree.powerUpCount } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(buttonAssign);
        }

        else if (tag == "decoyDeplo")
        {
            ExitGames.Client.Photon.Hashtable decoyPurchase = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.DECOY_DEPLOYMENT, Convert.ToInt32(powerUpBought) } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(decoyPurchase);

            ExitGames.Client.Photon.Hashtable decoySlotPurchase = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.DECOY_DEPLOYMENT_SLOT, (int)skillTree.powerUpCount } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(decoySlotPurchase);

            ExitGames.Client.Photon.Hashtable buttonAssign = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.BUTTON_ASSIGN, (int)skillTree.powerUpCount } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(buttonAssign);
        }
    }
}
