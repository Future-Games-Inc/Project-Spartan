using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public TurretBluePrint standardTurret;
    public TurretBluePrint missleLauncher;
    public TurretBluePrint lazerTurret;
    //TODO: EMP and Extra Turrests


    BuildManager buildManager;

    void Start()
    {
        buildManager = BuildManager.instance;
    }



    public void SelectStandardTurret()
    {
        Debug.Log("Standard Turret Selected");
        buildManager.SelectTurretToBuild(standardTurret);
    }

    public void SelectMissleLauncher()
    {
        Debug.Log("Missel Turret Selected");
        buildManager.SelectTurretToBuild(missleLauncher);
    }

    public void SelectLazerTurret()
    {
        Debug.Log("Lazere Turret Selected");
        buildManager.SelectTurretToBuild(lazerTurret);
    }

}
