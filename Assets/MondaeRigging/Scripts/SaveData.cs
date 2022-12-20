using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    public int SkillPoints;
    // Start is called before the first frame update
    void Start()
    {
        object savedCints;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.CINTS, out savedCints))
        {
            SkillPoints = (int)savedCints;
        }
        else
            SkillPoints = 30;

        ExitGames.Client.Photon.Hashtable cints = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.CINTS, SkillPoints } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(cints);
        Save();
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

    public void Save()
    {
        ES3AutoSaveMgr.Current.Save();
    }
}
