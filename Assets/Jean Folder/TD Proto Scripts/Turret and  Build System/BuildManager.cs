using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    //singleton pattern to ensure one instance on the scene. Easy to access this instance
    public static BuildManager instance;

    void Awake()
    {
        if(instance != null)
        {
            Debug.Log("More than one build manager in scene");
            return;
        }
        instance = this;    
    }

    [Header("Build VFX")]
    public GameObject buildEffect;
    [Header("Sell VFX")]
    public GameObject sellEffect;


    private TurretBluePrint turretToBuild;
    private Nodo selecetedNode;
    public NodoUI nodeUI;

    // this is a property, we want a public bool variable, that will check if we can build or not. 
    public bool CanBuild { get { return turretToBuild != null; } }
    // public bool variable to check if we have money for the turret
    public bool HasMoney { get { return PlayerStats.Money >= turretToBuild.cost; } }



    public void SelectedNode(Nodo node)
    {
        if(selecetedNode == node)
        {
            DeselectNode();
            return;
        }
        selecetedNode = node;
        turretToBuild = null;

        nodeUI.SetTarget(node);
    }


    public void DeselectNode()
    {
        selecetedNode = null;
        nodeUI.Hide();

    }

    public void SelectTurretToBuild(TurretBluePrint turret)
    {
        turretToBuild = turret;
        DeselectNode();

    }

    public TurretBluePrint GetTurretToBuild()
    {
        return turretToBuild;
    }



}
