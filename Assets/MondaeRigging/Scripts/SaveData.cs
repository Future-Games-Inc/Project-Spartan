using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    public int SkillPoints;
    // Start is called before the first frame update
    void Start()
    {
        SkillPoints = 20;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateSkills(int skills)
    {
        SkillPoints += skills;
        ES3AutoSaveMgr.Current.Save();
    }
}
