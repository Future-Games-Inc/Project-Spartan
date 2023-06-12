using System;

[System.Serializable]
public class Contract
{
    public int ID;
    public int requiredLevel;
    public string description;
    public string objectives;
    public bool isCompleted;
    public int expRewards;
    public int cintRewards;
    public bool isActive;

    public ContractGoal goal;

    public Contract(int id, int level, string desc, string obj, int exp, int cint)
    {
        ID = id;
        requiredLevel = level;
        description = desc;
        objectives = obj;
        isCompleted = false;
        isActive = false;
        expRewards = exp;
        cintRewards = cint;
    }
}
