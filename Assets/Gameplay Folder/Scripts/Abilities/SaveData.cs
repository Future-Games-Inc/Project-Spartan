using Photon.Pun;
using System.Collections;
using UnityEngine;
using LootLocker.Requests;

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
        object savedBosses;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.BossKilled, out savedBosses))
        {
            bossesKilled = (int)savedBosses;
        }
        else
            bossesKilled = ES3.Load<int>("BossKilled", 0);

        ExitGames.Client.Photon.Hashtable bosses = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.BossKilled, bossesKilled } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(bosses);

        object savedArtifacts;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.ArtifactFound, out savedArtifacts))
        {
            artifactsRecovered = (int)savedArtifacts;
        }
        else
            artifactsRecovered = ES3.Load<int>("ArtifactFound", 0);

        ExitGames.Client.Photon.Hashtable artifacts = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.ArtifactFound, artifactsRecovered } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(artifacts);

        object savedBomb;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.BombDestroyed, out savedBomb))
        {
            bombsDestroyed = (int)savedBomb;
        }
        else
            bombsDestroyed = ES3.Load<int>("BombDestroyed", 0);

        ExitGames.Client.Photon.Hashtable bombs = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.BombDestroyed, bombsDestroyed } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(bombs);

        object savedGuardian;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.GuardianDestroyed, out savedGuardian))
        {
            guardiansDestroyed = (int)savedGuardian;
        }
        else
            guardiansDestroyed = ES3.Load<int>("GuardianDestroyed", 0);

        ExitGames.Client.Photon.Hashtable guardians = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.GuardianDestroyed, guardiansDestroyed } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(guardians);

        object savedIntel;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.IntelFound, out savedIntel))
        {
            intelSecured = (int)savedIntel;
        }
        else
            intelSecured = ES3.Load<int>("IntelFound", 0);

        ExitGames.Client.Photon.Hashtable intel = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.IntelFound, intelSecured } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(intel);

        object savedCollectors;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.CollectorDestroyed, out savedCollectors))
        {
            collectorsDestroyed = (int)savedCollectors;
        }
        else
            collectorsDestroyed = ES3.Load<int>("CollectorDestroyed", 0);

        ExitGames.Client.Photon.Hashtable collectors = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.CollectorDestroyed, collectorsDestroyed } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(collectors);

        Save();
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
        ExitGames.Client.Photon.Hashtable cints = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.CINTS, SkillPoints } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(cints);
        Save();
    }

    [System.Obsolete]
    public void BossUpdateSkills(int skills)
    {
        bossesKilled += skills;
        ExitGames.Client.Photon.Hashtable bosses = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.BossKilled, bossesKilled } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(bosses);
        BossSave();
    }

    public void ArtifactUpdateSkills(int skills)
    {
        artifactsRecovered += skills;
        ExitGames.Client.Photon.Hashtable artifacts = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.ArtifactFound, artifactsRecovered } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(artifacts);
        ArtifactSave();
    }

    [System.Obsolete]
    public void BombUpdateSkills(int skills)
    {
        bombsDestroyed += skills;
        ExitGames.Client.Photon.Hashtable bombs = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.BombDestroyed, bombsDestroyed } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(bombs);
        BombSave();
    }

    public void GuardianUpdateSkills(int skills)
    {
        guardiansDestroyed += skills;
        ExitGames.Client.Photon.Hashtable guardians = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.GuardianDestroyed, guardiansDestroyed } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(guardians);
        GuardianSave();
    }

    public void IntelUpdateSkills(int skills)
    {
        intelSecured += skills;
        ExitGames.Client.Photon.Hashtable intel = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.IntelFound, intelSecured } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(intel);
        IntelSave();
    }

    public void CollectorUpdateSkills(int skills)
    {
        collectorsDestroyed += skills;
        ExitGames.Client.Photon.Hashtable collector = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.CollectorDestroyed, collectorsDestroyed } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(collector);
        CollectorSave();
    }

    public void Save()
    {
        ES3.Save("SkillPoints", SkillPoints);
        StartCoroutine(SubmitScore(SkillPoints));
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

    IEnumerator SubmitScore(int score)
    {
        yield return new WaitForSeconds(0f);
        StartCoroutine(leaderboard.SubmitScoreRoutine(score));
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
        object savedCints;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.CINTS, out savedCints))
        {
            SkillPoints = (int)savedCints;
            if (SkillPoints != leaderboard.Score)
                SkillPoints = leaderboard.Score;
        }
        else
            SkillPoints = ES3.Load<int>("SkillPoints", 50);

        ExitGames.Client.Photon.Hashtable cints = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.CINTS, SkillPoints } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(cints);

        yield return new WaitForSeconds(.75f);
        LootLockerSDKManager.GetPlayerInfo((response) =>
        {
            currentLevelInt = (int)response.level;
        });

        currentPrestigeLevel = (currentLevelInt / prestigeIncrement) + 1;


        object playerLevel;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.PlayerLevel, out playerLevel))
        {
            playerLevelCurrent = (int)playerLevel;
            if (playerLevelCurrent != currentLevelInt)
                playerLevelCurrent = currentLevelInt;

        }
        else
            playerLevelCurrent = ES3.Load<int>("PlayerLevel", 0);

        ExitGames.Client.Photon.Hashtable playerLevelSaved = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.PlayerLevel, playerLevelCurrent } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerLevelSaved);

        object playerPrestige;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.PlayerPrestige, out playerPrestige))
        {
            playerPrestigeCurrent = (int)playerPrestige;
            if (playerPrestigeCurrent != currentPrestigeLevel)
            {
                playerPrestigeCurrent = currentPrestigeLevel;
                awarded = true;
            }
        }
        else
            playerPrestigeCurrent = ES3.Load<int>("PlayerPrestige", 0);

        ExitGames.Client.Photon.Hashtable playerPrestigeSaved = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.PlayerPrestige, playerPrestigeCurrent } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerPrestigeSaved);

        PlayerLevelSave();
        PlayerPrestigeSave();
    }
}
