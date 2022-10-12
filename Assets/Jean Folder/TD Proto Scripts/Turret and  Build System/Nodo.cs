using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;


public class Nodo : MonoBehaviour
{
    [Header("Hover Color")]
    public Color hoverColor;
    public Color notEnoughMoneyColor;
    public Material startMaterial;
    public Material emptyMaterial;
    public Material highLightedMaterial;
    // on NODO Deactivated 
    public Material deactivatedMaterial;

    public Vector3 positionOffset;

    [HideInInspector]
    public GameObject turret;
    [HideInInspector]
    public TurretBluePrint turretBluePrint;
    [HideInInspector]
    public bool isUpgraded = false;


    private Renderer rend;
    private Color startColor;

    BuildManager buildManager;

    XRSimpleInteractable xRSimpleInteractable;

    //public InputActionProperty OnBButtonSelect;
    public InputActionReference onBButtonPressed;

    // todo: link XR Simple Intractable in order to trigger and activate with selected button of choosing. 
    public XRSimpleInteractable nodoSimpleInteractable;

    private void Awake()
    {
        onBButtonPressed.action.started += rightbButtonPressed;

    }



    private void OnDestroy()
    {
        onBButtonPressed.action.started += rightbButtonPressed;
    }

    private void rightbButtonPressed(InputAction.CallbackContext context)
    {

        //if(context.action.IsPressed() && nodoSimpleInteractable.isHovered)
        //{
        //    OnButtonPressActivation();

        //}

    }

    void Start()
    {
        buildManager = BuildManager.instance;
        rend = GetComponent<Renderer>();
        startColor = rend.material.color;
        startMaterial = rend.material;


        nodoSimpleInteractable = GetComponent<XRSimpleInteractable>();
    }

    public Vector3 GetBuildPosition()
    {
        return transform.position + positionOffset;
    }








    //
    // Replacement for mousebutton presss on the interactable
    //
    public void OnButtonPressActivation()
    {

        if (turret != null)
        {

            buildManager.SelectedNode(this);
            return;
        }

        if (!buildManager.CanBuild)
        {
            return;
        }
        //buildManager.BuildTurretOn(this);
        BuildTurret(buildManager.GetTurretToBuild());

    }


    public void SellTurret()
    {
        PlayerStats.Money += turretBluePrint.GetSellAmount();
        //TODO: Spawn cool VFX on the Tear down of the tower
        GameObject effect = (GameObject)Instantiate(buildManager.sellEffect, GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 4f);

        Destroy(turret);
        turretBluePrint = null;

    }





    public void UpgradeTurret()
    {
        if (PlayerStats.Money < turretBluePrint.upgradeCost)
        {
            Debug.Log("your too poor, not enough to upgrade fool!");
            return;
        }

        PlayerStats.Money -= turretBluePrint.upgradeCost;

        //getting rid of the old turret
        Destroy(turret);


        //building a new turret. 
        //casting to a gameobject
        GameObject _turret = (GameObject)Instantiate(turretBluePrint.upgradedPrefab, GetBuildPosition(), Quaternion.identity);
        turret = _turret;



        //casting the VFX build into a GameObject and waiting 4 seconds to delet the build. 
        // TODO: Possible can add and upgrade effect here
        GameObject effect = (GameObject)Instantiate(buildManager.buildEffect, GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 4f);


        isUpgraded = true;



    }




    void BuildTurret(TurretBluePrint blueprint)
    {
        if (PlayerStats.Money < blueprint.cost)
        {
            Debug.Log("Not enough Money");
            return;
        }

        PlayerStats.Money -= blueprint.cost;

        //casting to a gameobject
        GameObject _turret = (GameObject)Instantiate(blueprint.prefab, GetBuildPosition(), Quaternion.identity);
        turret = _turret;

        turretBluePrint = blueprint;

        //casting the VFX build into a GameObject and waiting 4 seconds to delet the build. 
        GameObject effect = (GameObject)Instantiate(buildManager.buildEffect, GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 4f);


    }

    //*
    //      WHen the XR Ray Hovers onto the Node
    //*

    public void OnXRRayEnter()
    {
        //only highlight where to build a turret when we have a turret selected


        if (!buildManager.CanBuild)
        {
            return;
        }

        if (buildManager.HasMoney)
        {
            GetComponent<Renderer>().material = highLightedMaterial;
            GetComponent<Renderer>().material.color = hoverColor;
        }
        else
        {
            GetComponent<Renderer>().material.color = notEnoughMoneyColor;
        }


    }

    //*
    //          WHen the XR Ray Hovers OUT of the Node
    //*
    public void OnXRRayExit()
    {
        GetComponent<Renderer>().material = startMaterial;
        rend.material.color = startColor;
    }

    //added the two methods to activate and deactive the Nodo Script

    public IEnumerator OnReactorPlaced()
    {
        WaitForSeconds Wait = new WaitForSeconds(2);
        GetComponent<Renderer>().material = startMaterial;
        nodoSimpleInteractable.enabled = true;
        this.enabled = true;
        //gameObject.SetActive(true);
        yield return Wait;
    }

    public IEnumerator OnReactorRemoved()
    {
        WaitForSeconds Wait = new WaitForSeconds(2);
        GetComponent<Renderer>().material = deactivatedMaterial;
        nodoSimpleInteractable.enabled = false;
        this.enabled = false;
        yield return Wait;
        //gameObject.SetActive(false);
    }

}