using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using JetBrains.Annotations;

public class PlayerWeapon : MonoBehaviourPunCallbacks
{
    public GameObject player;
    public Transform[] spawnPoint;
    public float fireSpeed = 20;
    public GameObject bullet;
    public Rotator rotatorScript;
    public GameObject explosionObject;

    public int maxAmmo;
    public int ammoLeft;
    public int durability;

    public GameObject reloadingScreen;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI durabilityText;

    public AudioSource audioSource;
    public AudioClip weaponFire;
    public AudioClip weaponReload;
    public AudioClip weaponBreak;

    public bool reloadingWeapon = false;

    // Start is called before the first frame update
    void Start()
    {
        rotatorScript = GetComponent<Rotator>();
        photonView.RPC("RPC_Start", RpcTarget.AllBuffered);
    }

    // Update is called once per frame
    void Update()
    {
        photonView.RPC("RPC_Update", RpcTarget.AllBuffered);
    }

    public void FireBullet(ActivateEventArgs arg)
    {
        photonView.RPC("RPC_Fire", RpcTarget.AllBuffered);
    }

    IEnumerator ReloadWeapon()
    {
        photonView.RPC("RPC_Reload", RpcTarget.AllBuffered);
        yield return new WaitForSeconds(2);
        photonView.RPC("RPC_Reload2", RpcTarget.AllBuffered);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LeftHand") || other.CompareTag("RightHand"))
        {
            player = other.transform.root.parent.gameObject;
            photonView.RPC("RPC_Trigger", RpcTarget.AllBuffered);
        }
    }

    IEnumerator DestroyWeapon()
    {
        yield return new WaitForSeconds(0.5f);
        photonView.RPC("RPC_Destroy", RpcTarget.AllBuffered);
        yield return new WaitForSeconds(0.5f);
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    void RPC_Start()
    {
        reloadingScreen.SetActive(false);
        ammoLeft = maxAmmo;
        ammoText.text = ammoLeft.ToString();
        XRGrabNetworkInteractable grabbable = GetComponent<XRGrabNetworkInteractable>();
        grabbable.activated.AddListener(FireBullet);
    }

    [PunRPC]
    void RPC_Update()
    {
        ammoText.text = ammoLeft.ToString();
        durabilityText.text = durability.ToString();
    }

    [PunRPC]
    void RPC_Fire()
    {
        if (ammoLeft >= 1 && reloadingWeapon == false)
        {
            foreach (Transform t in spawnPoint)
            {
                audioSource.PlayOneShot(weaponFire);
                GameObject spawnedBullet = PhotonNetwork.Instantiate(bullet.name, t.position, Quaternion.identity);
                spawnedBullet.GetComponent<Bullet>().bulletModifier = player.GetComponent<PlayerHealth>().bulletModifier;
                spawnedBullet.gameObject.GetComponent<Bullet>().bulletOwner = player.gameObject;
                spawnedBullet.gameObject.GetComponent<Bullet>().playerBullet = true;
                spawnedBullet.GetComponent<Rigidbody>().velocity = t.right * fireSpeed;
                ammoLeft -= 1;
            }
        }

        if (ammoLeft <= 0 && reloadingWeapon == false)
        {
            reloadingWeapon = true;
            StartCoroutine(ReloadWeapon());
        }
    }

    [PunRPC]
    void RPC_Reload()
    {
        reloadingScreen.SetActive(true);
        audioSource.PlayOneShot(weaponReload);
        durability--;

        if (durability <= 0)
        {
            audioSource.PlayOneShot(weaponBreak);
            GetComponent<XRGrabNetworkInteractable>().enabled = false;
            StartCoroutine(DestroyWeapon());
        }
    }

    [PunRPC]
    void RPC_Reload2()
    {
        ammoLeft = maxAmmo;
        reloadingScreen.SetActive(false);
        reloadingWeapon = false;
    }

    [PunRPC]
    void RPC_Trigger()
    {
        var newMaxAmmo = player.GetComponentInParent<PlayerHealth>().maxAmmo + maxAmmo;
        maxAmmo = newMaxAmmo;
        GetComponent<Rigidbody>().isKinematic = false;
        rotatorScript.enabled = false;
    }

    [PunRPC]
    void RPC_Destroy(Collider other)
    {
        explosionObject.SetActive(true);
    }
}
