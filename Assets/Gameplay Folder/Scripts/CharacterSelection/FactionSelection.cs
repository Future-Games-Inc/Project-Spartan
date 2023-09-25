using LootLocker.Requests;
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

    public GameObject factionDecision;
    public GameObject LevelSelector;
    public GameObject leaveButton;

    public float factionTimer;
    public float depositTimer;

    const string factionSelected = "SelectedFaction";
    const string factionSelectionDate = "FactionSelectionDate";
    const string factionDepositDate = "FactionDepositDate";
    public string progressionKey = "cent_prog";


    public SaveData saveData;

    // Start is called before the first frame update
    void OnEnable()
    {
        if (PlayerPrefs.HasKey(factionSelected) && PlayerPrefs.GetString(factionSelected) == "Cyber SK Gang")
        {
            cyberGang = true;
            picked = true;
        }
        else if (PlayerPrefs.HasKey(factionSelected) && PlayerPrefs.GetString(factionSelected) == "Muerte De Dios")
        {
            muerteGang = true;
            picked = true;
        }
        else if (PlayerPrefs.HasKey(factionSelected) && PlayerPrefs.GetString(factionSelected) == "Chaos Cartel")
        {
            chaosGang = true;
            picked = true;
        }

        else if (PlayerPrefs.HasKey(factionSelected) && PlayerPrefs.GetString(factionSelected) == "CintSix Cartel")
        {
            cintGang = true;
            picked = true;
        }
        else
        {
            picked = false;
            factionDecision.SetActive(true);
        }
        if (PlayerPrefs.HasKey(factionSelectionDate))
        {
            DateTime savedDate = DateTime.Parse(PlayerPrefs.GetString(factionSelectionDate));
            DateTime currentDate = DateTime.Now;
            TimeSpan difference = currentDate - savedDate;

            if (difference.TotalDays < 7)
            {
                leaveButton.SetActive(false);
            }
            else if (difference.TotalDays >= 7)
            {
                leaveButton.SetActive(true);
            }
        }
        else if(!PlayerPrefs.HasKey(factionSelectionDate))
            leaveButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (cyberGang == true || muerteGang == true || chaosGang == true || cintGang == true)
            factionDecision.SetActive(false);
        else
            factionDecision.SetActive(true);

        if (PlayerPrefs.HasKey(factionSelected))
        {
            cyberBanner.SetActive(cyberGang);
            muerteBanner.SetActive(muerteGang);
            chaosBanner.SetActive(chaosGang);
            cintBanner.SetActive(cintGang);
        }

        LevelSelector.SetActive(picked);
    }

    public void CyberSelect()
    {
        PlayerPrefs.SetString(factionSelected, "Cyber SK Gang");

        cyberGang = true;
        muerteGang = false;
        chaosGang = false;
        cintGang = false;

        factionDecision.SetActive(false);
        SaveCurrentDate();
    }

    public void MuerteSelect()
    {
        PlayerPrefs.SetString(factionSelected, "Muerte De Dios");

        cyberGang = false;
        muerteGang = true;
        chaosGang = false;
        cintGang = false;

        factionDecision.SetActive(false);
        SaveCurrentDate();
    }

    public void ChaosSelect()
    {
        PlayerPrefs.SetString(factionSelected, "Chaos Cartel");

        cyberGang = false;
        muerteGang = false;
        chaosGang = true;
        cintGang = false;

        factionDecision.SetActive(false);
        SaveCurrentDate();
    }

    void SaveCurrentDate()
    {
        PlayerPrefs.SetString(factionSelectionDate, DateTime.Now.ToString());
        if (PlayerPrefs.HasKey(factionSelectionDate))
        {
            DateTime savedDate = DateTime.Parse(PlayerPrefs.GetString(factionSelectionDate));
            DateTime currentDate = DateTime.Now;
            TimeSpan difference = currentDate - savedDate;

            if (difference.TotalDays < 7)
            {
                leaveButton.SetActive(false);
            }
        }
    }

    public void LeaveFaction()
    {
        if (PlayerPrefs.HasKey(factionSelected))
        {
            PlayerPrefs.DeleteKey(factionSelected);
            PlayerPrefs.DeleteKey(factionSelectionDate); // Clear the saved date as well
            picked = false;
            cyberGang = false;
            cintGang = false;
            muerteGang = false;
            chaosGang = false;
            cyberBanner.SetActive(false);
            muerteBanner.SetActive(false);
            chaosBanner.SetActive(false);
            cintBanner.SetActive(false);
            LootLockerSDKManager.ResetPlayerProgression(progressionKey, response =>
            {
            });
        }
    }
}
