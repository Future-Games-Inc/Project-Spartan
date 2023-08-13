using BNG;
using UnityEngine;

public class FabricatorAction : MonoBehaviour
{
    public SnapZone snapZone;
    public GameObject pistolUpgrader;
    public GameObject empUpgrader;
    public GameObject empPistol;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (snapZone.HeldItem == null)
        {
            pistolUpgrader.SetActive(false);
            empUpgrader.SetActive(false);
        }
    }

    public void EnterFabricator()
    {
        if (snapZone.HeldItem.gameObject.name == "Z_Pistol" || snapZone.HeldItem.gameObject.name == "Z_EMPPistol")
        {
            if (snapZone.HeldItem.gameObject.name == "Z_Pistol")
            {
                pistolUpgrader.SetActive(true);
                empUpgrader.SetActive(false);
            }
            else if (snapZone.HeldItem.gameObject.name == "Z_EMPPistol")
            {
                pistolUpgrader.SetActive(false);
                empUpgrader.SetActive(true);
            }
            snapZone.lastHeldItem = snapZone.HeldItem;
            EMPPistolNet pistol = snapZone.HeldItem.gameObject.GetComponent<EMPPistolNet>();
            pistol.reloadingScreen.SetActive(false);
            pistol.durabilityText.enabled = false;
            pistol.ammoText.enabled = false;
        }
    }

    public void ExitFabricator()
    {
        if (snapZone.lastHeldItem.gameObject.name == "Z_Pistol" || snapZone.lastHeldItem.gameObject.name == "Z_EMPPistol")
        {
            EMPPistolNet pistol = snapZone.lastHeldItem.gameObject.GetComponent<EMPPistolNet>();
            pistol.reloadingScreen.SetActive(true);
            pistol.durabilityText.enabled = true;
            pistol.ammoText.enabled = true;
        }
        Rescale();
    }

    public void UpgradePistol()
    {
        if (snapZone.HeldItem.gameObject.name == "Z_Pistol")
        {
            Destroy(snapZone.HeldItem.gameObject);
            GameObject newEmpPistol = Instantiate(empPistol, snapZone.gameObject.transform.position, snapZone.gameObject.transform.rotation);
            newEmpPistol.SetActive(true);
            Grabbable newGrabbable = empPistol.GetComponent<Grabbable>();
            snapZone.GrabGrabbable(newGrabbable);
        }
    }

    public void Rescale()
    {
        if (snapZone.lastHeldItem.gameObject.name == "Z_Pistol" || snapZone.lastHeldItem.gameObject.name == "Z_EMPPistol")
        {
            snapZone.lastHeldItem.gameObject.GetComponent<EMPPistolNet>().Rescale();
        }
    }
}
