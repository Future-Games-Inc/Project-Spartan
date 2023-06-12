using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ContractGoal
{
    public ContractType ContractType;

    public int requriedAmount;
    public int currentAmount;

    public bool IsReached()
    {
        return (currentAmount >= requriedAmount);
    }
}

public enum ContractType
{
    BossEnemy,
    Artifact,
    Bombs,
    Guardian,
    Intel,
    Collector
}
