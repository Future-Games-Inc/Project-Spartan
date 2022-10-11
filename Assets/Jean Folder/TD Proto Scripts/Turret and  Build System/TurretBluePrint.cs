using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TurretBluePrint 
{
    public GameObject prefab;
    public int cost;

    public GameObject upgradedPrefab;
    public int upgradeCost;

    public int GetSellAmount()
    {
        return cost / 2;

    }

    //TODO: Add in the 2nd Final Upgrage Turretprefab


}
