using System;
using UnityEngine;

public class FactionSelection : MonoBehaviour
{
    public bool cyberGang = false;
    public bool muerteGang = false;
    public bool chaosGang = false;
    public bool cintGang = false;

    public bool picked;

    public GameObject cyberBanner;
    public GameObject cintBanner;
    public GameObject muerteBanner;
    public GameObject chaosBanner;

    public Canvas factionDecision;
    public GameObject LevelSelector;

    const string factionSelected = "SelectedFaction";
    // Start is called before the first frame update
    void OnEnable()
    {
        if (PlayerPrefs.HasKey(factionSelected) && PlayerPrefs.GetString(factionSelected) == "Cyber SK Gang")
        {
            cyberGang = true;
            picked = true;
        }
        if (PlayerPrefs.HasKey(factionSelected) && PlayerPrefs.GetString(factionSelected) == "Muerte De Dios")
        {
            muerteGang = true;
            picked = true;
        }
        if (PlayerPrefs.HasKey(factionSelected) && PlayerPrefs.GetString(factionSelected) == "Chaos Cartel")
        {
            chaosGang = true;
            picked = true;
        }
        if (PlayerPrefs.HasKey(factionSelected) && PlayerPrefs.GetString(factionSelected) == "CintSix Cartel")
        {
            cintGang = true;
            picked = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (cyberGang == true || muerteGang == true || chaosGang == true || cintGang == true)
            factionDecision.enabled = false;

        if (cyberGang)
            cyberBanner.SetActive(true);
        if (muerteGang)
            muerteBanner.SetActive(true);
        if (chaosGang)
            chaosBanner.SetActive(true);
        if (cintGang)
            cintBanner.SetActive(true);

        LevelSelector.SetActive(picked);
    }

    public void CyberSelect()
    {
        PlayerPrefs.SetString(factionSelected, "Cyber SK Gang");

        cyberGang = true;
        muerteGang = false;
        chaosGang = false;
        cintGang = false;

        factionDecision.enabled = false;
    }

    public void MuerteSelect()
    {
        PlayerPrefs.SetString(factionSelected, "Muerte De Dios");

        cyberGang = false;
        muerteGang = true;
        chaosGang = false;
        cintGang = false;

        factionDecision.enabled = false;
    }

    public void ChaosSelect()
    {
        PlayerPrefs.SetString(factionSelected, "Chaos Cartel");

        cyberGang = false;
        muerteGang = false;
        chaosGang = true;
        cintGang = false;

        factionDecision.enabled = false;
    }

    public void CintSelect()
    {
        PlayerPrefs.SetString(factionSelected, "CintSix Cartel");

        cyberGang = false;
        muerteGang = false;
        chaosGang = false;
        cintGang = true;

        factionDecision.enabled = false;
    }
}
