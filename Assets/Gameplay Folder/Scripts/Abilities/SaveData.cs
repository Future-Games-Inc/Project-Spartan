using System.Collections;
using UnityEngine;
using LootLocker.Requests;
using UnityEngine.SocialPlatforms.Impl;

public class SaveData : MonoBehaviour
{
    public int SkillPoints;

    public int bossesKilled;
    public int artifactsRecovered;
    public int bombsDestroyed;
    public int guardiansDestroyed;
    public int intelSecured;
    public int collectorsDestroyed;

    public int playerLevelCurrent;
    public int playerPrestigeCurrent;

    public int currentLevelInt;
    public int currentPrestigeLevel;
    public int prestigeIncrement = 10;

    public bool awarded;

    public TopReactsLeaderboard leaderboard;
    public BlackMarketManager blackMarketManager;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("BossKilled"))
        {
            bossesKilled = PlayerPrefs.GetInt("BossKilled");
        }
        else
            bossesKilled = ES3.Load<int>("BossKilled", 0);

        PlayerPrefs.SetInt("BossKilled", bossesKilled);

        if (PlayerPrefs.HasKey("ArtifactFound"))
        {
            artifactsRecovered = PlayerPrefs.GetInt("ArtifactFound");
        }
        else
            artifactsRecovered = ES3.Load<int>("ArtifactFound", 0);

        PlayerPrefs.SetInt("ArtifactFound", artifactsRecovered);

        if (PlayerPrefs.HasKey("BombDestroyed"))
        {
            bombsDestroyed = PlayerPrefs.GetInt("BombDestroyed");
        }
        else
            bombsDestroyed = ES3.Load<int>("BombDestroyed", 0);

        PlayerPrefs.SetInt("BombDestroyed", bombsDestroyed);

        if (PlayerPrefs.HasKey("GuardianDestroyed"))
        {
            guardiansDestroyed = PlayerPrefs.GetInt("GuardianDestroyed");
        }
        else
            guardiansDestroyed = ES3.Load<int>("GuardianDestroyed", 0);

        PlayerPrefs.SetInt("GuardianDestroyed", guardiansDestroyed);

        if (PlayerPrefs.HasKey("IntelFound"))
        {
            intelSecured = PlayerPrefs.GetInt("IntelFound");
        }
        else
            intelSecured = ES3.Load<int>("IntelFound", 0);

        PlayerPrefs.SetInt("IntelFound", intelSecured);

        if (PlayerPrefs.HasKey("CollectorDestroyed"))
        {
            collectorsDestroyed = PlayerPrefs.GetInt("CollectorDestroyed");
        }
        else
            collectorsDestroyed = ES3.Load<int>("CollectorDestroyed", 0);

        PlayerPrefs.SetInt("CollectorDestroyed", collectorsDestroyed);

        BossSave();
        ArtifactSave();
        BombSave();
        GuardianSave();
        IntelSave();
        CollectorSave();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateSkills(int skills)
    {
        SkillPoints += skills;
        PlayerPrefs.SetInt("BossKilled", bossesKilled);
        Save();
    }

    [System.Obsolete]
    public void BossUpdateSkills(int skills)
    {
        bossesKilled += skills;
        PlayerPrefs.SetInt("BossKilled", bossesKilled);
        BossSave();
    }

    public void ArtifactUpdateSkills(int skills)
    {
        artifactsRecovered += skills;
        PlayerPrefs.SetInt("BossKilled", bossesKilled);
        ArtifactSave();
    }

    [System.Obsolete]
    public void BombUpdateSkills(int skills)
    {
        bombsDestroyed += skills;
        PlayerPrefs.SetInt("BossKilled", bossesKilled);
        BombSave();
    }

    public void GuardianUpdateSkills(int skills)
    {
        guardiansDestroyed += skills;
        PlayerPrefs.SetInt("BossKilled", bossesKilled);
        GuardianSave();
    }

    public void IntelUpdateSkills(int skills)
    {
        intelSecured += skills;
        PlayerPrefs.SetInt("BossKilled", bossesKilled);
        IntelSave();
    }

    public void CollectorUpdateSkills(int skills)
    {
        collectorsDestroyed += skills;
        PlayerPrefs.SetInt("BossKilled", bossesKilled);
        CollectorSave();
    }

    public void Save()
    {
        ES3.Save("SkillPoints", SkillPoints);
        StartCoroutine(leaderboard.SubmitScoreRoutine(SkillPoints));
    }

    public void BossSave()
    {
        ES3.Save("bossesKilled", bossesKilled);
    }

    public void ArtifactSave()
    {
        ES3.Save("artifactsRecovered", artifactsRecovered);
    }

    public void BombSave()
    {
        ES3.Save("bombsDestroyed", bombsDestroyed);
    }

    public void GuardianSave()
    {
        ES3.Save("guardiansDestroyed", guardiansDestroyed);
    }

    public void IntelSave()
    {
        ES3.Save("intelSecured", intelSecured);
    }

    public void CollectorSave()
    {
        ES3.Save("collectorsDestroyed", collectorsDestroyed);
    }

    public void PlayerLevelSave()
    {
        ES3.Save("PlayerLevel", playerLevelCurrent);
    }

    public void PlayerPrestigeSave()
    {
        ES3.Save("PlayerPrestige", playerPrestigeCurrent);
    }

    public IEnumerator PlayerLevelRoutine()
    {
        yield return new WaitForSeconds(.75f);
        if (PlayerPrefs.HasKey("Cints"))
        {
            SkillPoints = PlayerPrefs.GetInt("CINTS");
            if (SkillPoints != leaderboard.Score && SkillPoints < leaderboard.Score)
                SkillPoints = leaderboard.Score;
        }
        SkillPoints = leaderboard.Score;

        PlayerPrefs.SetInt("CINTS", SkillPoints);

        currentLevelInt = leaderboard.currentLevelInt;

        currentPrestigeLevel = (currentLevelInt / prestigeIncrement) + 1;


        if (PlayerPrefs.HasKey("PlayerLevel"))
        {
            playerLevelCurrent = PlayerPrefs.GetInt("PlayerLevel");
            if (playerLevelCurrent != currentLevelInt && playerLevelCurrent < currentLevelInt)
                playerLevelCurrent = currentLevelInt;

        }
        playerLevelCurrent = leaderboard.currentLevelInt;

        PlayerPrefs.SetInt("PlayerLevel", playerLevelCurrent);

        if (PlayerPrefs.HasKey("PlayerPrestige"))
        {
            playerPrestigeCurrent = PlayerPrefs.GetInt("PlayerPrestige");
            if (playerPrestigeCurrent != currentPrestigeLevel && playerPrestigeCurrent < currentPrestigeLevel)
            {
                awarded = true;
            }
        }
        playerPrestigeCurrent = playerPrestigeCurrent / prestigeIncrement + 1;

        PlayerLevelSave();
        Save();
    }

    public void PlayerPrestige()
    {
        if (PlayerPrefs.HasKey("PlayerPrestige"))
        {
            playerPrestigeCurrent = PlayerPrefs.GetInt("PlayerPrestige");
            playerPrestigeCurrent = currentPrestigeLevel;
        }

        PlayerPrefs.SetInt("PlayerPrestige", playerPrestigeCurrent);

        PlayerPrestigeSave();
    }
}
