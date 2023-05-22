using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using Photon.Pun;
using TMPro;

public class EMPPistolNet : MonoBehaviourPunCallbacks
{
    [Header("Spawning ---------------------------------------")]
    public Transform[] spawnPoint;

    [Header("Audio ---------------------------------------")]
    public AudioSource audioSource;
    public AudioClip reloadSFX;
    public AudioClip weaponBreak;

    [Header("Gun property --------------------------------")]
    public int maxAmmo;
    [HideInInspector]
    public int ammoLeft;
    public int durability;
    public float ReloadDuration;
    public Rotator rotatorScript;
    public GameObject reloadingScreen;
    public GameObject player;
    public GameObject explosionObject;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI durabilityText;

    [Header("Bullet Types --------------------------------")]
    public GameObject Bullet;
    [HideInInspector]
    public bool active = true;
    [HideInInspector]
    public bool isFiring = false;
    public bool reloadingWeapon = false;

    [Header("Keybinds ------------------------------------")]
    public InputActionProperty triggerL;
    public InputActionProperty triggerR;
    [HideInInspector]
    public bool isDual = false;
    [HideInInspector]
    public bool isTriggerSingle = false;
    // Start is called before the first frame update
    void OnEnbale()
    {
        durability = 5;
        rotatorScript = GetComponent<Rotator>();
        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
        photonView.RPC("RPC_EMPStart", RpcTarget.All);
        StartCoroutine(TextUpdate());
    }

    IEnumerator TextUpdate()
    {
        while (true)
        {
            photonView.RPC("RPC_EMPText", RpcTarget.All);
            yield return new WaitForSeconds(.15f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        checkForInputs();
        // check for durablity
        if (durability <= 0)
            StartCoroutine(DestroyWeapon());
    }

    private void checkForInputs()
    {
        // ignore if the gun is not being held
        if (!isTriggerSingle) return;

        // hold down trigger R for fire (works on left hand as well)
        if ((triggerR.action.ReadValue<float>() > 0.8f || triggerL.action.ReadValue<float>() > 0.8f))
        {
            if (!isFiring)
            {
                isFiring = true;
                StartCoroutine(StartFireBullet());
            }
        }

        if ((triggerR.action.ReadValue<float>() < 0.8f || triggerL.action.ReadValue<float>() < 0.8f))
        {
            if (isFiring)
                isFiring = false;
        }
    }

    public void setDual(bool state)
    {
        isDual = state;
    }

    public void setHeld(bool state)
    {
        isTriggerSingle = state;
    }

    IEnumerator StartFireBullet()
    {
        while (isFiring)
        {
            if (ammoLeft >= 1 && reloadingWeapon == false)
            {
                foreach (Transform t in spawnPoint)
                {
                    GameObject spawnedBullet = PhotonNetwork.InstantiateRoomObject(Bullet.name, t.position, Quaternion.identity, 0, null);
                    audioSource.PlayOneShot(spawnedBullet.GetComponent<BulletBehaviorNet>().clip);
                    spawnedBullet.GetComponent<Rigidbody>().velocity = spawnPoint[0].forward * spawnedBullet.GetComponent<BulletBehaviorNet>().TravelSpeed;
                    spawnedBullet.GetComponent<Bullet>().bulletModifier = player.GetComponent<PlayerHealth>().bulletModifier;
                    spawnedBullet.gameObject.GetComponent<Bullet>().bulletOwner = player.gameObject;
                    spawnedBullet.gameObject.GetComponent<Bullet>().playerBullet = true;
                }
                photonView.RPC("RPC_EMPFire", RpcTarget.All);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    IEnumerator Reload()
    {
        photonView.RPC("RPC_EMPReload", RpcTarget.All);
        yield return new WaitForSeconds(2);
        photonView.RPC("RPC_EMPReload2", RpcTarget.All);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LeftHand") || other.CompareTag("RightHand"))
        {
            player = other.transform.root.gameObject;
            photonView.RPC("RPC_EMPTrigger", RpcTarget.All);
        }
    }

    IEnumerator DestroyWeapon()
    {
        yield return new WaitForSeconds(0.5f);
        photonView.RPC("RPC_EMPDestroy", RpcTarget.All);
        yield return new WaitForSeconds(0.5f);
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    void RPC_EMPStart()
    {
        reloadingScreen.SetActive(false);
        ammoLeft = maxAmmo;
        ammoText.text = ammoLeft.ToString();
    }

    [PunRPC]
    void RPC_EMPText()
    {
        ammoText.text = ammoLeft.ToString();
        durabilityText.text = durability.ToString();
    }

    [PunRPC]
    void RPC_RPC_EMPFire()
    {
        ammoLeft--;

        if (ammoLeft <= 0 && reloadingWeapon == false)
        {
            reloadingWeapon = true;
            StartCoroutine(Reload());
        }
    }

    [PunRPC]
    void RPC_EMPReload()
    {
        reloadingScreen.SetActive(true);
        audioSource.PlayOneShot(reloadSFX);
        durability--;

        if (durability <= 0)
        {
            audioSource.PlayOneShot(weaponBreak);
            GetComponent<XRGrabNetworkInteractable>().enabled = false;
            StartCoroutine(DestroyWeapon());
        }
    }

    [PunRPC]
    void RPC_EMPReload2()
    {
        ammoLeft = maxAmmo;
        reloadingScreen.SetActive(false);
        reloadingWeapon = false;
    }

    [PunRPC]
    void RPC_EMPTrigger()
    {
        var newMaxAmmo = player.GetComponentInParent<PlayerHealth>().maxAmmo + maxAmmo;
        maxAmmo = newMaxAmmo;
        GetComponent<Rigidbody>().isKinematic = false;
        rotatorScript.enabled = false;
    }

    [PunRPC]
    void RPC_EMPDestroy()
    {
        explosionObject.SetActive(true);
    }
}
