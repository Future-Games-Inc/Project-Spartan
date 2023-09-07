using BNG;
using UnityEngine;

public class FabricatorAction : MonoBehaviour
{
    public SnapZone snapZone;
    public GameObject pistolUpgrader;
    public GameObject empUpgrader;
    public GameObject empPistol;
    public GameObject shotgunUpgrader;
    public GameObject stingerUpgrader;
    public GameObject stingerShotgun;
    public GameObject rifleUpgrader;
    public GameObject pulseUpgrader;
    public GameObject pulseAR;
    public GameObject ZBowNormal;
    public GameObject ZBowBomb;
    public GameObject ZBowEMP;
    public GameObject bowUpgrade;

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
            shotgunUpgrader.SetActive(false);
            stingerUpgrader.SetActive(false);
            rifleUpgrader.SetActive(false);
            pulseUpgrader.SetActive(false);
            bowUpgrade.SetActive(false);
        }
    }

    public void EnterFabricator()
    {
        if (snapZone.HeldItem.gameObject.name == "Z_Pistol" || snapZone.HeldItem.gameObject.name == "Z_Pistol(Clone)" /*|| snapZone.HeldItem.gameObject.name == "Z_EMPPistol"*/)
        {
            //if (snapZone.HeldItem.gameObject.name == "Z_Pistol")
            //{
                pistolUpgrader.SetActive(true);
                empUpgrader.SetActive(false);
                PlayerWeapon pistol = snapZone.HeldItem.gameObject.GetComponent<PlayerWeapon>();
                pistol.reloadingScreen.SetActive(false);
                pistol.durabilityText.enabled = false;
                pistol.ammoText.enabled = false;
            //}
            //else if (snapZone.HeldItem.gameObject.name == "Z_EMPPistol")
            //{
            //    empUpgrader.SetActive(true);
            //    pistolUpgrader.SetActive(false);
            //    EMPPistolNet pistol = snapZone.HeldItem.gameObject.GetComponent<EMPPistolNet>();
            //    pistol.reloadingScreen.SetActive(false);
            //    pistol.durabilityText.enabled = false;
            //    pistol.ammoText.enabled = false;
            //}
        }

        else if (snapZone.HeldItem.gameObject.name == "Z_Shotgun" || snapZone.HeldItem.gameObject.name == "Z_Shotgun(Clone)" /*|| snapZone.HeldItem.gameObject.name == "Z_Stinger Shotgun"*/)
        {
            //if (snapZone.HeldItem.gameObject.name == "Z_Shotgun")
            //{
                shotgunUpgrader.SetActive(true);
                stingerUpgrader.SetActive(false);
                PlayerWeapon shotgun = snapZone.HeldItem.gameObject.GetComponent<PlayerWeapon>();
                shotgun.reloadingScreen.SetActive(false);
                shotgun.durabilityText.enabled = false;
                shotgun.ammoText.enabled = false;
            //}
            //else if (snapZone.HeldItem.gameObject.name == "Z_Stinger Shotgun")
            //{
            //    stingerUpgrader.SetActive(true);
            //    shotgunUpgrader.SetActive(false);
            //    StingerShotgun stinger = snapZone.HeldItem.gameObject.GetComponent<StingerShotgun>();
            //    stinger.reloadingScreen.SetActive(false);
            //    stinger.durabilityText.enabled = false;
            //    stinger.ammoText.enabled = false;
            //}
        }

        else if (snapZone.HeldItem.gameObject.name == "Z_Rifle" || snapZone.HeldItem.gameObject.name == "Z_Rifle(Clone)" /*|| snapZone.HeldItem.gameObject.name == "Z_PulseAR"*/)
        {
            //if (snapZone.HeldItem.gameObject.name == "Z_Rifle")
            //{
                rifleUpgrader.SetActive(true);
                pulseUpgrader.SetActive(false);
                PlayerWeapon rifle = snapZone.HeldItem.gameObject.GetComponent<PlayerWeapon>();
                rifle.reloadingScreen.SetActive(false);
                rifle.durabilityText.enabled = false;
                rifle.ammoText.enabled = false;
            //}
            //else if (snapZone.HeldItem.gameObject.name == "Z_PulseAR")
            //{
            //    pulseUpgrader.SetActive(true);
            //    rifleUpgrader.SetActive(false);
            //    PulseARNet pulseAR = snapZone.HeldItem.gameObject.GetComponent<PulseARNet>();
            //    pulseAR.reloadingScreen.SetActive(false);
            //    pulseAR.durabilityText.enabled = false;
            //    pulseAR.ammoText.enabled = false;
            //}
        }

        else if (snapZone.HeldItem.gameObject.name == "Z_BowNormal" || snapZone.HeldItem.gameObject.name == "Z_BowBomb" || snapZone.HeldItem.gameObject.name == "Z_BowEMP" || snapZone.HeldItem.gameObject.name == "Z_BowNormal(Clone)" || snapZone.HeldItem.gameObject.name == "Z_BowBomb(Clone)" || snapZone.HeldItem.gameObject.name == "Z_BowEMP(Clone)")
        {
            bowUpgrade.SetActive(true);
        }
        snapZone.lastHeldItem = snapZone.HeldItem;
    }

    public void ExitFabricator()
    {
        if (snapZone.lastHeldItem.gameObject.name == "Z_EMPPistol" || snapZone.lastHeldItem.gameObject.name == "Z_EMPPistol(Clone)")
        {
            EMPPistolNet pistol = snapZone.lastHeldItem.gameObject.GetComponent<EMPPistolNet>();
            pistol.durabilityText.enabled = true;
            pistol.ammoText.enabled = true;
        }
        else if (snapZone.lastHeldItem.gameObject.name == "Z_Pistol" || snapZone.lastHeldItem.gameObject.name == "Z_Pistol(Clone)")
        {
            PlayerWeapon pistol = snapZone.HeldItem.gameObject.GetComponent<PlayerWeapon>();
            pistol.durabilityText.enabled = true;
            pistol.ammoText.enabled = true;
        }
        else if (snapZone.lastHeldItem.gameObject.name == "Z_Shotgun" || snapZone.lastHeldItem.gameObject.name == "Z_Shotgun(Clone)")
        {
            PlayerWeapon shotgun = snapZone.HeldItem.gameObject.GetComponent<PlayerWeapon>();
            shotgun.durabilityText.enabled = true;
            shotgun.ammoText.enabled = true;
        }
        else if (snapZone.lastHeldItem.gameObject.name == "Z_Stinger Shotgun" || snapZone.lastHeldItem.gameObject.name == "Z_Stinger Shotgun(Clone)")
        {
            StingerShotgun stinger = snapZone.HeldItem.gameObject.GetComponent<StingerShotgun>();
            stinger.durabilityText.enabled = true;
            stinger.ammoText.enabled = true;
        }
        else if (snapZone.lastHeldItem.gameObject.name == "Z_Rifle" || snapZone.lastHeldItem.gameObject.name == "Z_Rifle(Clone)")
        {
            PlayerWeapon rifle = snapZone.HeldItem.gameObject.GetComponent<PlayerWeapon>();
            rifle.durabilityText.enabled = true;
            rifle.ammoText.enabled = true;
        }
        else if (snapZone.lastHeldItem.gameObject.name == "Z_PulseAR" || snapZone.lastHeldItem.gameObject.name == "Z_PulseAR(Clone)")
        {
            PulseARNet pulseAR = snapZone.HeldItem.gameObject.GetComponent<PulseARNet>();
            pulseAR.durabilityText.enabled = true;
            pulseAR.ammoText.enabled = true;
        }
        Rescale();
    }

    public void Upgrade()
    {
        if (snapZone.HeldItem.gameObject.name == "Z_Pistol" || snapZone.HeldItem.gameObject.name == "Z_Pistol(Clone)")
        {
            Destroy(snapZone.HeldItem.gameObject);
            GameObject newEmpPistol = Instantiate(empPistol, snapZone.gameObject.transform.position, snapZone.gameObject.transform.rotation);
            newEmpPistol.gameObject.name = "Z_EMPPistol";
            EMPPistolNet pistol = newEmpPistol.GetComponent<EMPPistolNet>();
            pistol.reloadingScreen.SetActive(false);
            pistol.durabilityText.enabled = false;
            pistol.ammoText.enabled = false;
            Grabbable newGrabbable = empPistol.GetComponent<Grabbable>();
            snapZone.GrabGrabbable(newGrabbable);
        }
        else if (snapZone.HeldItem.gameObject.name == "Z_Shotgun" || snapZone.HeldItem.gameObject.name == "Z_Shotgun(Clone)")
        {
            Destroy(snapZone.HeldItem.gameObject);
            GameObject newStingerShotgun = Instantiate(stingerShotgun, snapZone.gameObject.transform.position, snapZone.gameObject.transform.rotation);
            newStingerShotgun.gameObject.name = "Z_Stinger Shotgun";
            StingerShotgun stinger = newStingerShotgun.GetComponent<StingerShotgun>();
            stinger.reloadingScreen.SetActive(false);
            stinger.durabilityText.enabled = false;
            stinger.ammoText.enabled = false;
            Grabbable newGrabbable = newStingerShotgun.GetComponent<Grabbable>();
            snapZone.GrabGrabbable(newGrabbable);
        }
        else if (snapZone.HeldItem.gameObject.name == "Z_Rifle" || snapZone.HeldItem.gameObject.name == "Z_Rifle(Clone)")
        {
            Destroy(snapZone.HeldItem.gameObject);
            GameObject newPulseAR = Instantiate(pulseAR, snapZone.gameObject.transform.position, snapZone.gameObject.transform.rotation);
            newPulseAR.gameObject.name = "Z_PulseAR";
            PulseARNet AR = newPulseAR.GetComponent<PulseARNet>();
            AR.reloadingScreen.SetActive(false);
            AR.durabilityText.enabled = false;
            AR.ammoText.enabled = false;
            Grabbable newGrabbable = newPulseAR.GetComponent<Grabbable>();
            snapZone.GrabGrabbable(newGrabbable);
        }
    }

    public void UpgradeBowNormal()
    {

        Destroy(snapZone.HeldItem.gameObject);
        GameObject newBow = Instantiate(ZBowNormal, snapZone.gameObject.transform.position, snapZone.gameObject.transform.rotation);
        newBow.gameObject.name = "Z_BowNormal";
        Grabbable newGrabbable = newBow.GetComponent<Grabbable>();
        snapZone.GrabGrabbable(newGrabbable);
    }

    public void UpgradeBowBomb()
    {

        Destroy(snapZone.HeldItem.gameObject);
        GameObject newBow = Instantiate(ZBowBomb, snapZone.gameObject.transform.position, snapZone.gameObject.transform.rotation);
        newBow.gameObject.name = "Z_BowBomb";
        Grabbable newGrabbable = newBow.GetComponent<Grabbable>();
        snapZone.GrabGrabbable(newGrabbable);
    }

    public void UpgradeBowEMP()
    {

        Destroy(snapZone.HeldItem.gameObject);
        GameObject newBow = Instantiate(ZBowEMP, snapZone.gameObject.transform.position, snapZone.gameObject.transform.rotation);
        newBow.gameObject.name = "Z_BowEMP";
        Grabbable newGrabbable = newBow.GetComponent<Grabbable>();
        snapZone.GrabGrabbable(newGrabbable);
    }

    public void Rescale()
    {
        if (snapZone.lastHeldItem.gameObject.name == "Z_Pistol" || snapZone.lastHeldItem.gameObject.name == "Z_Shotgun" || snapZone.lastHeldItem.gameObject.name == "Z_Rifle" || snapZone.lastHeldItem.gameObject.name == "Z_Pistol(Clone)" || snapZone.lastHeldItem.gameObject.name == "Z_Shotgun(Clone)" || snapZone.lastHeldItem.gameObject.name == "Z_Rifle(Clone)")
        {
            snapZone.lastHeldItem.gameObject.GetComponent<PlayerWeapon>().Rescale();
        }
        else if (snapZone.lastHeldItem.gameObject.name == "Z_EMPPistol" || snapZone.lastHeldItem.gameObject.name == "Z_EMPPistol(Clone)")
        {
            snapZone.lastHeldItem.gameObject.GetComponent<EMPPistolNet>().Rescale();
        }
        else if (snapZone.lastHeldItem.gameObject.name == "Z_Stinger Shotgun" || snapZone.lastHeldItem.gameObject.name == "Z_Stinger Shotgun(Clone)")
        {
            snapZone.lastHeldItem.gameObject.GetComponent<StingerShotgun>().Rescale();
        }
        else if (snapZone.lastHeldItem.gameObject.name == "Z_PulseAR" || snapZone.lastHeldItem.gameObject.name == "Z_PulseAR(Clone)")
        {
            snapZone.lastHeldItem.gameObject.GetComponent<PulseARNet>().Rescale();
        }
        else if (snapZone.lastHeldItem.gameObject.name == "Z_BowNormal" || snapZone.lastHeldItem.gameObject.name == "Z_BowBomb" || snapZone.lastHeldItem.gameObject.name == "Z_BowEMP" || snapZone.lastHeldItem.gameObject.name == "Z_BowNormal(Clone)" || snapZone.lastHeldItem.gameObject.name == "Z_BowBomb(Clone)" || snapZone.lastHeldItem.gameObject.name == "Z_BowEMP(Clone)")
        {
            snapZone.lastHeldItem.gameObject.GetComponent<BowNet>().Rescale();
        }
    }
}
