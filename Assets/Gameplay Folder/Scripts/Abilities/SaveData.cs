using System.Collections;
using UnityEngine;

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

    string[] settings = { "BULLET_MODIFIER", "REACTOR_EXTRACTION", "TOXICITY_DAMAGE", "DAMAGAE_TAKEN", "PLAYER_SPEED", "PLAYER_HEALTH", "PLAYER_ARMOR", "PLAYER_DASH", "HEALTH_POWERUP", "DASH_COOLDOWN", "AMMO_OVERLOAD", "HEALTH_REGEN", "HEALTH_STIM", "LEECH",
    "ACTIVE_CAMO", "STEALTH","EXPLOSIVE_DEATH", "BERSERKER_FURY","AI_COMPANION", "DECOY_DEPLOYMENT","SAVING_GRACE", "HEALTH_STIM_SLOT","LEECH_SLOT", "ACTIVE_CAMO_SLOT","STEALTH_SLOT", "EXPLOSIVE_DEATH_SLOT","BERSERKER_FURY_SLOT", "AI_COMPANION_SLOT","DECOY_DEPLOYMENT_SLOT", "SAVING_GRACE_SLOT", "AVATAR_SELECTION_NUMBER"
    , "REACTOR_EXTRACTION", "EnemyKills", "PlayersKilled", "BUTTON_ASSIGN"};

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < settings.Length; i++)
        {
            if (PlayerPrefs.HasKey(settings[i]))
                PlayerPrefs.DeleteKey(settings[i]);
        }
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
        PlayerPrefs.SetInt("CINTS", SkillPoints);
        leaderboard.SubmitScore(SkillPoints);
        Save();
    }

    public void BossUpdateSkills(int skills)
    {
        bossesKilled += skills;
        PlayerPrefs.SetInt("BossKilled", bossesKilled);
        BossSave();
    }

    public void ArtifactUpdateSkills(int skills)
    {
        artifactsRecovered += skills;
        PlayerPrefs.SetInt("ArtifactFound", artifactsRecovered);
        ArtifactSave();
    }

    public void BombUpdateSkills(int skills)
    {
        bombsDestroyed += skills;
        PlayerPrefs.SetInt("BombDestroyed", bombsDestroyed);
        BombSave();
    }

    public void GuardianUpdateSkills(int skills)
    {
        guardiansDestroyed += skills;
        PlayerPrefs.SetInt("GuardianDestroyed", guardiansDestroyed);
        GuardianSave();
    }

    public void IntelUpdateSkills(int skills)
    {
        intelSecured += skills;
        PlayerPrefs.SetInt("IntelFound", intelSecured);
        IntelSave();
    }

    public void CollectorUpdateSkills(int skills)
    {
        collectorsDestroyed += skills;
        PlayerPrefs.SetInt("CollectorDestroyed", collectorsDestroyed);
        CollectorSave();
    }

    public void Save()
    {
        ES3.Save("SkillPoints", SkillPoints);
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
        if (PlayerPrefs.HasKey("CINTS"))
        {
            SkillPoints = PlayerPrefs.GetInt("CINTS");
            if (SkillPoints != leaderboard.Score)
                leaderboard.SubmitScore(SkillPoints);
        }
        else
            SkillPoints = leaderboard.Score;

        PlayerPrefs.SetInt("CINTS", SkillPoints);

        currentLevelInt = leaderboard.currentLevelInt;

        currentPrestigeLevel = (currentLevelInt / prestigeIncrement) + 1;


        if (PlayerPrefs.HasKey("PlayerLevel"))
        {
            playerLevelCurrent = PlayerPrefs.GetInt("PlayerLevel");
            if (playerLevelCurrent != currentLevelInt)
            {
                if(playerLevelCurrent > currentLevelInt)
                    leaderboard.AddProgression(playerLevelCurrent - currentLevelInt);
                else if(playerLevelCurrent < currentLevelInt)
                    playerLevelCurrent = currentLevelInt;
            }
        }
        else
            playerLevelCurrent = currentLevelInt;

        PlayerPrefs.SetInt("PlayerLevel", playerLevelCurrent);

        if (PlayerPrefs.HasKey("PlayerPrestige"))
        {
            playerPrestigeCurrent = PlayerPrefs.GetInt("PlayerPrestige");
            if (playerPrestigeCurrent != currentPrestigeLevel && playerPrestigeCurrent < currentPrestigeLevel)
            {
                awarded = true;
            }
            else
                awarded = false;
        }
        else
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
