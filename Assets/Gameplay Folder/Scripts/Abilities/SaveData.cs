using Photon.Pun;
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

    public TopReactsLeaderboard leaderboard;
    public BlackMarketManager blackMarketManager;

    // Start is called before the first frame update
    [System.Obsolete]
    void Start()
    {
        object savedCints;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.CINTS, out savedCints))
        {
            SkillPoints = (int)savedCints;
        }
        else
            SkillPoints = ES3.Load<int>("SkillPoints", 50);

        ExitGames.Client.Photon.Hashtable cints = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.CINTS, SkillPoints } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(cints);

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

    [System.Obsolete]
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

    [System.Obsolete]
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

    [System.Obsolete]
    public void GuardianUpdateSkills(int skills)
    {
        guardiansDestroyed += skills;
        ExitGames.Client.Photon.Hashtable guardians = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.GuardianDestroyed, guardiansDestroyed } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(guardians);
        GuardianSave();
    }

    [System.Obsolete]
    public void IntelUpdateSkills(int skills)
    {
        intelSecured += skills;
        ExitGames.Client.Photon.Hashtable intel = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.IntelFound, intelSecured } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(intel);
        IntelSave();
    }

    [System.Obsolete]
    public void CollectorUpdateSkills(int skills)
    {
        collectorsDestroyed += skills;
        ExitGames.Client.Photon.Hashtable collector = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.CollectorDestroyed, collectorsDestroyed } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(collector);
        CollectorSave();
    }

    [System.Obsolete]
    public void Save()
    {
        ES3.Save("SkillPoints", SkillPoints);
        StartCoroutine(SubmitScore(SkillPoints));
    }

    [System.Obsolete]
    public void BossSave()
    {
        ES3.Save("bossesKilled", bossesKilled);
        StartCoroutine(SubmitScore(bossesKilled));
    }

    [System.Obsolete]
    public void ArtifactSave()
    {
        ES3.Save("artifactsRecovered", artifactsRecovered);
        StartCoroutine(SubmitScore(artifactsRecovered));
    }

    [System.Obsolete]
    public void BombSave()
    {
        ES3.Save("bombsDestroyed", bombsDestroyed);
        StartCoroutine(SubmitScore(bombsDestroyed));
    }

    [System.Obsolete]
    public void GuardianSave()
    {
        ES3.Save("guardiansDestroyed", guardiansDestroyed);
        StartCoroutine(SubmitScore(guardiansDestroyed));
    }

    [System.Obsolete]
    public void IntelSave()
    {
        ES3.Save("intelSecured", intelSecured);
        StartCoroutine(SubmitScore(intelSecured));
    }

    [System.Obsolete]
    public void CollectorSave()
    {
        ES3.Save("collectorsDestroyed", collectorsDestroyed);
        StartCoroutine(SubmitScore(collectorsDestroyed));
    }

    [System.Obsolete]
    IEnumerator SubmitScore(int score)
    {
        yield return new WaitForSeconds(0f);
        yield return leaderboard.SubmitScoreRoutine(score);
    }
}
