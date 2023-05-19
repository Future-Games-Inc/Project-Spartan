using System;

[System.Serializable]
public class Contract
{
    public int ID;
    public string description;
    public string objectives;
    public bool isCompleted;
    public float duration; // Duration in seconds
    public DateTime completionTime;

    public Contract(int id, string desc, string obj, float dur)
    {
        ID = id;
        description = desc;
        objectives = obj;
        duration = dur;
        isCompleted = false;
        completionTime = DateTime.MinValue;
    }
}
