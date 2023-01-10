using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using ExitGames.Client.Photon;

public class FactionSelection : MonoBehaviourPunCallbacks
{
    public bool cyberGang = false;
    public bool muerteGang = false;
    public bool chaosGang = false;
    public bool cintGang = false;
    public bool fedGang = false;

    public GameObject cyberBanner;
    public GameObject cintBanner;
    public GameObject fedBanner;
    public GameObject muerteBanner;
    public GameObject chaosBanner;

    public Canvas factionDecision;

    const string factionSelected = "SelectedFaction";
    // Start is called before the first frame update
    void OnEnable()
    {
        object faction;
        if (PlayerPrefs.HasKey(factionSelected) && PlayerPrefs.GetString(factionSelected) == "Cyber SK Gang")
        {
            cyberGang = true;
            ExitGames.Client.Photon.Hashtable cyberGangJoin = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.CYBER_SK_GANG, Convert.ToInt32(cyberGang) } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(cyberGangJoin);
            PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.CYBER_SK_GANG, out faction);
        }
        if (PlayerPrefs.HasKey(factionSelected) && PlayerPrefs.GetString(factionSelected) == "Muerte De Dios")
        {
            muerteGang = true;
            ExitGames.Client.Photon.Hashtable muerteGangJoin = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.MUERTE_DE_DIOS, Convert.ToInt32(muerteGang) } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(muerteGangJoin);
            PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.MUERTE_DE_DIOS, out faction);
        }
        if (PlayerPrefs.HasKey(factionSelected) && PlayerPrefs.GetString(factionSelected) == "Chaos Cartel")
        {
            chaosGang = true;
            ExitGames.Client.Photon.Hashtable chaosGangJoin = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.CHAOS_CARTEL, Convert.ToInt32(chaosGang) } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(chaosGangJoin);
            PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.CHAOS_CARTEL, out faction);
        }
        if (PlayerPrefs.HasKey(factionSelected) && PlayerPrefs.GetString(factionSelected) == "CintSix Cartel")
        {
            cintGang = true;
            ExitGames.Client.Photon.Hashtable cintGangJoin = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.CINTSIX_CARTEL, Convert.ToInt32(cintGang) } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(cintGangJoin);
            PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.CINTSIX_CARTEL, out faction);
        }
        if (PlayerPrefs.HasKey(factionSelected) && PlayerPrefs.GetString(factionSelected) == "Federation Zone Authority")
        {
            fedGang = true;
            ExitGames.Client.Photon.Hashtable fedGangJoin = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.FEDZONE_AUTHORITY, Convert.ToInt32(fedGang) } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(fedGangJoin);
            PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.FEDZONE_AUTHORITY, out faction);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (cyberGang == true || muerteGang == true || chaosGang == true || cintGang == true || fedGang == true)
            factionDecision.enabled = false;

        if (cyberGang)
            cyberBanner.SetActive(true);
        if (muerteGang)
            muerteBanner.SetActive(true);
        if (chaosGang)
            chaosBanner.SetActive(true);
        if (cintGang)
            cintBanner.SetActive(true);
        if (fedGang)
            fedBanner.SetActive(true);
    }

    public void CyberSelect()
    {
        PlayerPrefs.SetString(factionSelected, "Cyber SK Gang");

        cyberGang = true;
        muerteGang = false;
        chaosGang = false;
        cintGang = false;
        fedGang = false;
        object faction;

        ExitGames.Client.Photon.Hashtable cyberGangJoin = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.CYBER_SK_GANG, Convert.ToInt32(cyberGang) } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(cyberGangJoin);
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.CYBER_SK_GANG, out faction);

        ExitGames.Client.Photon.Hashtable muerteGangJoin = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.MUERTE_DE_DIOS, Convert.ToInt32(muerteGang) } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(muerteGangJoin);
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.MUERTE_DE_DIOS, out faction);

        ExitGames.Client.Photon.Hashtable chaosGangJoin = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.CHAOS_CARTEL, Convert.ToInt32(chaosGang) } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(chaosGangJoin);
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.CHAOS_CARTEL, out faction);

        ExitGames.Client.Photon.Hashtable cintGangJoin = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.CINTSIX_CARTEL, Convert.ToInt32(cintGang) } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(cintGangJoin);
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.CINTSIX_CARTEL, out faction);

        ExitGames.Client.Photon.Hashtable fedGangJoin = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.FEDZONE_AUTHORITY, Convert.ToInt32(fedGang) } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(fedGangJoin);
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.FEDZONE_AUTHORITY, out faction);

        factionDecision.enabled = false;
    }

    public void MuerteSelect()
    {
        PlayerPrefs.SetString(factionSelected, "Muerte De Dios");

        cyberGang = false;
        muerteGang = true;
        chaosGang = false;
        cintGang = false;
        fedGang = false;
        object faction;

        ExitGames.Client.Photon.Hashtable cyberGangJoin = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.CYBER_SK_GANG, Convert.ToInt32(cyberGang) } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(cyberGangJoin);
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.CYBER_SK_GANG, out faction);

        ExitGames.Client.Photon.Hashtable muerteGangJoin = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.MUERTE_DE_DIOS, Convert.ToInt32(muerteGang) } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(muerteGangJoin);
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.MUERTE_DE_DIOS, out faction);

        ExitGames.Client.Photon.Hashtable chaosGangJoin = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.CHAOS_CARTEL, Convert.ToInt32(chaosGang) } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(chaosGangJoin);
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.CHAOS_CARTEL, out faction);

        ExitGames.Client.Photon.Hashtable cintGangJoin = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.CINTSIX_CARTEL, Convert.ToInt32(cintGang) } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(cintGangJoin);
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.CINTSIX_CARTEL, out faction);

        ExitGames.Client.Photon.Hashtable fedGangJoin = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.FEDZONE_AUTHORITY, Convert.ToInt32(fedGang) } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(fedGangJoin);
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.FEDZONE_AUTHORITY, out faction);

        factionDecision.enabled = false;
    }

    public void ChaosSelect()
    {
        PlayerPrefs.SetString(factionSelected, "Chaos Cartel");

        cyberGang = false;
        muerteGang = false;
        chaosGang = true;
        cintGang = false;
        fedGang = false;
        object faction;

        ExitGames.Client.Photon.Hashtable cyberGangJoin = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.CYBER_SK_GANG, Convert.ToInt32(cyberGang) } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(cyberGangJoin);
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.CYBER_SK_GANG, out faction);

        ExitGames.Client.Photon.Hashtable muerteGangJoin = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.MUERTE_DE_DIOS, Convert.ToInt32(muerteGang) } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(muerteGangJoin);
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.MUERTE_DE_DIOS, out faction);

        ExitGames.Client.Photon.Hashtable chaosGangJoin = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.CHAOS_CARTEL, Convert.ToInt32(chaosGang) } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(chaosGangJoin);
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.CHAOS_CARTEL, out faction);

        ExitGames.Client.Photon.Hashtable cintGangJoin = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.CINTSIX_CARTEL, Convert.ToInt32(cintGang) } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(cintGangJoin);
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.CINTSIX_CARTEL, out faction);

        ExitGames.Client.Photon.Hashtable fedGangJoin = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.FEDZONE_AUTHORITY, Convert.ToInt32(fedGang) } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(fedGangJoin);
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.FEDZONE_AUTHORITY, out faction);

        factionDecision.enabled = false;
    }

    public void CintSelect()
    {
        PlayerPrefs.SetString(factionSelected, "CintSix Cartel");

        cyberGang = false;
        muerteGang = false;
        chaosGang = false;
        cintGang = true;
        fedGang = false;
        object faction;

        ExitGames.Client.Photon.Hashtable cyberGangJoin = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.CYBER_SK_GANG, Convert.ToInt32(cyberGang) } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(cyberGangJoin);
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.CYBER_SK_GANG, out faction);

        ExitGames.Client.Photon.Hashtable muerteGangJoin = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.MUERTE_DE_DIOS, Convert.ToInt32(muerteGang) } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(muerteGangJoin);
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.MUERTE_DE_DIOS, out faction);

        ExitGames.Client.Photon.Hashtable chaosGangJoin = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.CHAOS_CARTEL, Convert.ToInt32(chaosGang) } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(chaosGangJoin);
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.CHAOS_CARTEL, out faction);

        ExitGames.Client.Photon.Hashtable cintGangJoin = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.CINTSIX_CARTEL, Convert.ToInt32(cintGang) } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(cintGangJoin);
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.CINTSIX_CARTEL, out faction);

        ExitGames.Client.Photon.Hashtable fedGangJoin = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.FEDZONE_AUTHORITY, Convert.ToInt32(fedGang) } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(fedGangJoin);
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.FEDZONE_AUTHORITY, out faction);

        factionDecision.enabled = false;
    }

    public void FedSelect()
    {
        PlayerPrefs.SetString(factionSelected, "Federation Zone Authority");

        cyberGang = false;
        muerteGang = false;
        chaosGang = false;
        cintGang = false;
        fedGang = true;
        object faction;

        ExitGames.Client.Photon.Hashtable cyberGangJoin = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.CYBER_SK_GANG, Convert.ToInt32(cyberGang) } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(cyberGangJoin);
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.CYBER_SK_GANG, out faction);

        ExitGames.Client.Photon.Hashtable muerteGangJoin = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.MUERTE_DE_DIOS, Convert.ToInt32(muerteGang) } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(muerteGangJoin);
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.MUERTE_DE_DIOS, out faction);

        ExitGames.Client.Photon.Hashtable chaosGangJoin = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.CHAOS_CARTEL, Convert.ToInt32(chaosGang) } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(chaosGangJoin);
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.CHAOS_CARTEL, out faction);

        ExitGames.Client.Photon.Hashtable cintGangJoin = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.CINTSIX_CARTEL, Convert.ToInt32(cintGang) } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(cintGangJoin);
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.CINTSIX_CARTEL, out faction);

        ExitGames.Client.Photon.Hashtable fedGangJoin = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.FEDZONE_AUTHORITY, Convert.ToInt32(fedGang) } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(fedGangJoin);
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.FEDZONE_AUTHORITY, out faction);

        factionDecision.enabled = false;
    }
}
