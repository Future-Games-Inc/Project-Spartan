using UnityEngine;
using System;

public class TimeTracker : MonoBehaviour
{
    public float elapsedTime;
    private DateTime startTime;
    public bool rewarded;

    private readonly int oneWeekInSeconds = 1800;  //604800
    private readonly int extraTimeInSeconds = 2700; //864000

    void Start()
    {
        if (PlayerPrefs.HasKey("LastPlayTime"))
        {
            startTime = Convert.ToDateTime(PlayerPrefs.GetString("LastPlayTime"));
        }
        else
        {
            startTime = DateTime.Now;
        }
    }

    void Update()
    {
        elapsedTime = (float)(DateTime.Now - startTime).TotalSeconds;

        if (elapsedTime > oneWeekInSeconds)
        {
            rewarded = true;
        }

        if (elapsedTime > extraTimeInSeconds)
        {
            startTime = DateTime.Now;
            elapsedTime = 0f;
            rewarded = false;
        }
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.SetString("LastPlayTime", startTime.ToString());
        PlayerPrefs.Save();
    }
}
