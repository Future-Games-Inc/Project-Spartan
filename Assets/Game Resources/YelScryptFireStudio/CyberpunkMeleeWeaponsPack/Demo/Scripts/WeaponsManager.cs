using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//This script is made to be used for the Demo only. It is not part of this assets pack.

public class WeaponsManager : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject weaponGallery;

    [Header ("Weapon Game Object")]
    public GameObject[] weaponGOArray;
    Vector3 weaponPos;
    Vector3[] origWeaponPosArray;
    Quaternion[] origWeaponRotArray;
    string weaponStr;
    bool clickWeapon = false;


    [Header ("Display Graph Text")]
    public GameObject displayGTextGO;
    public MeshRenderer displayGTextMR;
    Vector3 displayGTextPos;
    Quaternion displayGTextRot;
    Vector3 displayGTextSca;
    Vector3 oriDisplayGTextPos;
    Vector3 oriDisplayGTextRot;
    Vector3 oriDisplayGTextSca;
    public Material[] displayGTextMaterial;

    [Header ("Display Weapon")]
    public GameObject displayWeaponGO;
    public MeshRenderer displayWeaponMR;
    Vector3 displayWeaponPos;
    Quaternion displayWeaponRot;
    Vector3 displayWeaponSca;
    Vector3 oriDisplayWeaponPos;
    Vector3 oriDisplayWeaponRot;
    Vector3 oriDisplayWeaponSca;
    public Material[] displayWeaponMaterial;


    [Header ("Display Text")]
    public GameObject displayTextGO;
    public MeshRenderer displayTextMR;
    public Material[] displayTextMaterial;


    [Header ("Light")]
    public GameObject spotLightGO;
    Vector3 spotLightPos;  
    Vector3 origSpotLightPos;
    

    [Header ("Weapon Material")]
    public Material bladeMaterial;
    public Material greatSwordMaterial;
    public Material hammerMaterial;
    public Material knifeMaterial;
    public Material tonfaMaterial;
    string nameBladeMaterial;
    string nameGreatSwordMaterial;
    string nameHammerMaterial;
    string nameKnifeMaterial;
    string nameTonfaMaterial;

    // Start is called before the first frame update
    void Start()
    {       
        weaponGallery.SetActive(false);

        //Get Component Displays
        //Display Graph Text
        displayGTextMR = displayGTextGO.GetComponent<MeshRenderer>();
        oriDisplayGTextPos = displayGTextGO.transform.position;
        oriDisplayGTextRot = displayGTextGO.transform.localEulerAngles;
        oriDisplayGTextSca = displayGTextGO.transform.localScale;

        //Display Weapon
        displayWeaponMR = displayWeaponGO.GetComponent<MeshRenderer>();
        oriDisplayWeaponPos = displayWeaponGO.transform.position;
        oriDisplayWeaponRot = displayWeaponGO.transform.eulerAngles;
        oriDisplayWeaponSca = displayWeaponGO.transform.localScale;

        //Display Text
        displayTextMR = displayTextGO.GetComponent<MeshRenderer>();
        //--------------------------------------------------------------

        //Get Weapons and Components
        weaponGOArray = GameObject.FindGameObjectsWithTag("Weapon");
        origWeaponPosArray = new Vector3[weaponGOArray.Length];
        origWeaponRotArray = new Quaternion[weaponGOArray.Length];
        for(int i = 0; i < weaponGOArray.Length; i++)
        {
            origWeaponPosArray[i] = weaponGOArray[i].transform.position;
            origWeaponRotArray[i] = weaponGOArray[i].transform.rotation;
        }
        //------------------------------------------------------------------------------------------------------------
        
        //Get Spotlight
        spotLightGO = GameObject.Find("SpotLights");
        origSpotLightPos = spotLightGO.transform.position;
        //----------------------------------------------------------------------------------------------------------
    }

    // Update is called once per frame
    void Update()
    {
        ContentDisplayColor();
        ChangeWeaponColor();
        WeaponRayCastHit();
    }

    //Weapon Ray Cast Hit (Mouse Over)
    private void WeaponRayCastHit()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.name == "Blade")
            {
                weaponStr = hit.collider.gameObject.name;
                WeaponTransf(mainCamera.transform.position.x,mainCamera.transform.position.y,-0.6f);
                SpotLightTransf(origSpotLightPos.x,-0.49f,-0.8f);
                DisplayGTextTransf( oriDisplayGTextPos.x,oriDisplayGTextPos.y,oriDisplayGTextPos.z,
                                    oriDisplayGTextRot.x,oriDisplayGTextRot.y,oriDisplayGTextRot.z,
                                    oriDisplayGTextSca.x,oriDisplayGTextSca.y,oriDisplayGTextSca.z);
                DisplayWeaponTransf(oriDisplayWeaponPos.x,oriDisplayWeaponPos.y,oriDisplayWeaponPos.z,
                                    oriDisplayWeaponRot.x,oriDisplayWeaponRot.y,oriDisplayWeaponRot.z,
                                    oriDisplayWeaponSca.x,oriDisplayWeaponSca.y,oriDisplayWeaponSca.z);
                WeaponDisplayPreview();
            }
            else if (hit.collider.gameObject.name == "GreatSword")
            {
                weaponStr = hit.collider.gameObject.name;
                WeaponTransf(mainCamera.transform.position.x,1.20f,-0.6f);
                SpotLightTransf(origSpotLightPos.x,origSpotLightPos.y,-1.44f);
                DisplayGTextTransf( oriDisplayGTextPos.x,oriDisplayGTextPos.y,oriDisplayGTextPos.z,
                                    oriDisplayGTextRot.x,oriDisplayGTextRot.y,oriDisplayGTextRot.z,
                                    0.06f,oriDisplayGTextSca.y,oriDisplayGTextSca.z);
                DisplayWeaponTransf(oriDisplayWeaponPos.x,oriDisplayWeaponPos.y,oriDisplayWeaponPos.z,
                                    oriDisplayWeaponRot.x,oriDisplayWeaponRot.y,oriDisplayWeaponRot.z,
                                    oriDisplayWeaponSca.x,oriDisplayWeaponSca.y,oriDisplayWeaponSca.z);
                WeaponDisplayPreview();
            }
            else if (hit.collider.gameObject.name == "Hammer")
            {
                weaponStr = hit.collider.gameObject.name;
                WeaponTransf(mainCamera.transform.position.x,mainCamera.transform.position.y,-0.6f);
                SpotLightTransf(origSpotLightPos.x,0.05f,-0.8f);
                DisplayGTextTransf( oriDisplayGTextPos.x,oriDisplayGTextPos.y,oriDisplayGTextPos.z,
                                    oriDisplayGTextRot.x,oriDisplayGTextRot.y,oriDisplayGTextRot.z,
                                    oriDisplayGTextSca.x,oriDisplayGTextSca.y,oriDisplayGTextSca.z);
                DisplayWeaponTransf(-1.53f,1.12f,-0.0111585f,
                                    75.311f,oriDisplayWeaponRot.y,oriDisplayWeaponRot.z,
                                    oriDisplayWeaponSca.x,oriDisplayWeaponSca.y,oriDisplayWeaponSca.z);
                WeaponDisplayPreview();
            }
            else if (hit.collider.gameObject.name == "Knife")
            {
                weaponStr = hit.collider.gameObject.name;
                WeaponTransf(mainCamera.transform.position.x,1.3f,-1.47f);
                SpotLightTransf(origSpotLightPos.x,-0.89f,-0.8f);
                DisplayGTextTransf( oriDisplayGTextPos.x,oriDisplayGTextPos.y,oriDisplayGTextPos.z,
                                    oriDisplayGTextRot.x,oriDisplayGTextRot.y,oriDisplayGTextRot.z,
                                    oriDisplayGTextSca.x,oriDisplayGTextSca.y,oriDisplayGTextSca.z);
                DisplayWeaponTransf(oriDisplayWeaponPos.x,oriDisplayWeaponPos.y,oriDisplayWeaponPos.z,
                                    oriDisplayWeaponRot.x,oriDisplayWeaponRot.y,oriDisplayWeaponRot.z,
                                    oriDisplayWeaponSca.x,oriDisplayWeaponSca.y,oriDisplayWeaponSca.z);
                WeaponDisplayPreview();
            }
            else if (hit.collider.gameObject.name == "Tonfa")
            {
                weaponStr = hit.collider.gameObject.name;
                WeaponTransf(mainCamera.transform.position.x,1.20f,-1.34f);
                SpotLightTransf(origSpotLightPos.x,-0.62f,-0.8f);
                DisplayGTextTransf( oriDisplayGTextPos.x,oriDisplayGTextPos.y,oriDisplayGTextPos.z,
                                    oriDisplayGTextRot.x,oriDisplayGTextRot.y,oriDisplayGTextRot.z,
                                    oriDisplayGTextSca.x,oriDisplayGTextSca.y,oriDisplayGTextSca.z);
                DisplayWeaponTransf(oriDisplayWeaponPos.x,oriDisplayWeaponPos.y,oriDisplayWeaponPos.z,
                                    75.593f,70.677f,oriDisplayWeaponRot.z,
                                    oriDisplayWeaponSca.x,oriDisplayWeaponSca.y,oriDisplayWeaponSca.z);
                WeaponDisplayPreview();
            }
        }
        else
        {
            if(clickWeapon == false)
                UnLoadDisplay();
        }
    }
    //End of Weapon Ray Cast Hit (Mouse Over)

    //Change Weapon Material Color
    private void ChangeWeaponColor()
    {
        if(UIManager.effectModeStr == "Single")
        {
            nameBladeMaterial = "Blade-" + UIManager.primaryColorStr + " (Instance)";
            nameGreatSwordMaterial = "GreatSword-" + UIManager.primaryColorStr + " (Instance)";
            nameHammerMaterial = "Hammer-" + UIManager.primaryColorStr + " (Instance)";
            nameKnifeMaterial = "Knife-" + UIManager.primaryColorStr + " (Instance)";
            nameTonfaMaterial = "Tonfa-" + UIManager.primaryColorStr + " (Instance)";
        }
        else
        {
            nameBladeMaterial = "Blade-" + UIManager.primaryColorStr + "_" + UIManager.secondaryColorStr.ToLower() + "-" + UIManager.effectModeStr.ToUpper() + " (Instance)";
            nameGreatSwordMaterial = "GreatSword-" + UIManager.primaryColorStr + "_" + UIManager.secondaryColorStr.ToLower() + "-" + UIManager.effectModeStr.ToUpper() + " (Instance)";
            nameHammerMaterial = "Hammer-" + UIManager.primaryColorStr + "_" + UIManager.secondaryColorStr.ToLower() + "-" + UIManager.effectModeStr.ToUpper() + " (Instance)";
            nameKnifeMaterial = "Knife-" + UIManager.primaryColorStr + "_" + UIManager.secondaryColorStr.ToLower() + "-" + UIManager.effectModeStr.ToUpper() + " (Instance)";
            nameTonfaMaterial = "Tonfa-" + UIManager.primaryColorStr + "_" + UIManager.secondaryColorStr.ToLower() + "-" + UIManager.effectModeStr.ToUpper() + " (Instance)";
        }   

        for(int i = 0; i < weaponGOArray.Length; i++)
        {
            for(int j = 0; j < weaponGOArray[i].GetComponent<MeshRenderer>().materials.Length; j++)
            {
                if(weaponGOArray[i].GetComponent<MeshRenderer>().materials[j].name == nameBladeMaterial)
                {
                    bladeMaterial = weaponGOArray[i].GetComponent<MeshRenderer>().materials[j];
                    GameObject.Find("Blade").GetComponent<MeshRenderer>().material = bladeMaterial;
                }
                else if(weaponGOArray[i].GetComponent<MeshRenderer>().materials[j].name == nameGreatSwordMaterial)
                {
                    greatSwordMaterial = weaponGOArray[i].GetComponent<MeshRenderer>().materials[j];
                    GameObject.Find("GreatSword").GetComponent<MeshRenderer>().material = greatSwordMaterial;
                }
                else if(weaponGOArray[i].GetComponent<MeshRenderer>().materials[j].name == nameHammerMaterial)
                {
                    hammerMaterial = weaponGOArray[i].GetComponent<MeshRenderer>().materials[j];
                    GameObject.Find("Hammer").GetComponent<MeshRenderer>().material = hammerMaterial;
                }
                else if(weaponGOArray[i].GetComponent<MeshRenderer>().materials[j].name == nameKnifeMaterial)
                {
                    knifeMaterial = weaponGOArray[i].GetComponent<MeshRenderer>().materials[j];
                    GameObject.Find("Knife").GetComponent<MeshRenderer>().material = knifeMaterial;
                }
                else if(weaponGOArray[i].GetComponent<MeshRenderer>().materials[j].name == nameTonfaMaterial)
                {
                    tonfaMaterial = weaponGOArray[i].GetComponent<MeshRenderer>().materials[j];
                    GameObject.Find("Tonfa").GetComponent<MeshRenderer>().material = tonfaMaterial;
                }
            }
        }
    }
    //End of Change Weapon Material Color

    //Weapon Preview
    private void WeaponDisplayPreview()
    {     
        for(int i = 0; i < weaponGOArray.Length; i++){
            
            if(weaponGOArray[i].ToString().Contains(weaponStr) == true){
                if(clickWeapon == false)
                {
                    LoadDisplay();
                }

                //Click Weapons
                if(Input.GetMouseButtonDown(0) && clickWeapon == false)
                {
                    clickWeapon = true;
                    for(int j = 0; j < weaponGOArray.Length; j++)
                    {
                        if(weaponGOArray[j].ToString().Contains(weaponStr) == false)
                        {
                            weaponGOArray[j].GetComponentInChildren<Collider>().enabled = false;
                        }
                    }
                    weaponGOArray[i].transform.position = weaponPos;
                    spotLightGO.transform.position = spotLightPos;
                    LoadDisplay();
                    weaponGOArray[i].GetComponent<Animator>().enabled = true;
                    weaponGOArray[i].GetComponent<Animator>().Play("WeaponAnim",-1,0f);
                }
                else if(Input.GetMouseButtonDown(0) && clickWeapon == true)
                {
                    clickWeapon = false;
                    for(int j = 0; j < weaponGOArray.Length; j++)
                    {
                        if(weaponGOArray[j].ToString().Contains(weaponStr) == false)
                        {
                            weaponGOArray[j].GetComponentInChildren<Collider>().enabled = true;
                        }
                    }
                    weaponGOArray[i].transform.position = origWeaponPosArray[i];
                    weaponGOArray[i].transform.rotation = origWeaponRotArray[i];
                    spotLightGO.transform.position = origSpotLightPos;
                    UnLoadDisplay();
                    weaponGOArray[i].GetComponent<Animator>().enabled = false;
                }
                //End of Click Weapons
            }
        }
    }
    //End of Weapon Preview

    //GameObject DisplayGraphText Transform
    private void DisplayGTextTransf(float posx, float posy, float posz, float qrotx, float qroty, float qrotz, float scax, float scay, float scaz)
    {
        displayGTextPos = new Vector3(posx,posy,posz);
        displayGTextRot = Quaternion.Euler(qrotx,qroty,qrotz);
        displayGTextSca = new Vector3(scax,scay,scaz);
    }
    //End of GameObject DisplayGraphText Transform

    //GameObject DisplayWeapon Transform
    private void DisplayWeaponTransf(float posx, float posy, float posz, float qrotx, float qroty, float qrotz, float scax, float scay, float scaz)
    {
        displayWeaponPos = new Vector3(posx,posy,posz);
        displayWeaponRot = Quaternion.Euler(qrotx,qroty,qrotz);
        displayWeaponSca = new Vector3(scax,scay,scaz);
    }
    //End of GameObject DisplayWeapon Transform

    //GameObject Weapon Transform
    private void WeaponTransf(float posx, float posy, float posz)
    {
        weaponPos = new Vector3(posx,posy,posz);
    }
    //End of GameObject Weapon Transform

    //SpotLight Transform
    private void SpotLightTransf(float posx, float posy, float posz)
    {
        spotLightPos = new Vector3(posx,posy,posz);
    }
    //End of SpotLight Transform

    //Load Display Manager
    private void LoadDisplay(){
        displayGTextMR.enabled = true;
        displayWeaponMR.enabled = true;
        displayTextMR.enabled = true;

        displayGTextGO.transform.position = displayGTextPos;
        displayGTextGO.transform.rotation = displayGTextRot;
        displayGTextGO.transform.localScale = displayGTextSca;

        displayWeaponGO.transform.position = displayWeaponPos;
        displayWeaponGO.transform.rotation = displayWeaponRot;
        displayWeaponGO.transform.localScale = displayWeaponSca;

        for(int i = 0; i < displayGTextMaterial.Length; i++){
            if(displayGTextMaterial[i].ToString().Contains(weaponStr) == true){
                displayGTextMR.material = displayGTextMaterial[i];
            }
        }
        for(int i = 0; i < displayWeaponMaterial.Length; i++){
            if(displayWeaponMaterial[i].ToString().Contains(weaponStr) == true){
                displayWeaponMR.material = displayWeaponMaterial[i];
            }
        }
        for(int i = 0; i < displayTextMaterial.Length; i++){
            if(displayTextMaterial[i] .ToString().Contains(weaponStr) == true){
                displayTextMR.material = displayTextMaterial[i];
            }
        }

        ContentDisplayColor();
    }

    //Disable Content Display
    private void UnLoadDisplay(){
        displayGTextMR.enabled = false;
        displayWeaponMR.enabled = false;
        displayTextMR.enabled = false;
    }
    //End of Disable Content Display

    //Content Display Color Manager
    private void ContentDisplayColor()
    {

        displayGTextMR.material.SetColor("_Color", UIManager.colorDisplay);
        displayWeaponMR.material.SetColor("_Color", UIManager.colorDisplay);
        displayTextMR.material.SetColor("_Color", UIManager.colorDisplay);
    }
    //End of Content Display Color Manager
}
