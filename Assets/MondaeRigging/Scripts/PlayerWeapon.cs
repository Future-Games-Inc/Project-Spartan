using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using JetBrains.Annotations;

public class PlayerWeapon : MonoBehaviour
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

    PhotonView pV;

    // Start is called before the first frame update
    void Start()
    {
        pV = GetComponent<PhotonView>();
        rotatorScript = GetComponent<Rotator>();
        pV.RPC("RPC_Start", RpcTarget.AllBuffered);
    }

    // Update is called once per frame
    void Update()
    {
        pV.RPC("RPC_Update", RpcTarget.AllBuffered);
    }

    public void FireBullet(ActivateEventArgs arg)
    {
        pV.RPC("RPC_Fire", RpcTarget.AllBuffered, arg);
    }

    IEnumerator ReloadWeapon()
    {
        pV.RPC("RPC_Reload",RpcTarget.AllBuffered);
        yield return new WaitForSeconds(2);
        pV.RPC("RPC_Reload2", RpcTarget.AllBuffered);
    }

    private void OnTriggerEnter(Collider other)
    {
        pV.RPC("RPC_Trigger", RpcTarget.AllBuffered, other);
    }

    IEnumerator DestroyWeapon()
    {
        yield return new WaitForSeconds(0.5f);
        pV.RPC("RPC_Destroy", RpcTarget.AllBuffered);
        yield return new WaitForSeconds(0.5f);
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    void RPC_Start()
    {
        if (!pV.IsMine)
        { return; }

        reloadingScreen.SetActive(false);
        ammoLeft = maxAmmo;
        ammoText.text = ammoLeft.ToString();
        XRGrabNetworkInteractable grabbable = GetComponent<XRGrabNetworkInteractable>();
        grabbable.activated.AddListener(FireBullet);
    }

    [PunRPC]
    void RPC_Update()
    {
        if (!pV.IsMine)
        { return; }

        ammoText.text = ammoLeft.ToString();
        durabilityText.text = durability.ToString();

        if (durability <= 0)
        {
            audioSource.PlayOneShot(weaponBreak);
            GetComponent<XRGrabNetworkInteractable>().enabled = false;
            StartCoroutine(DestroyWeapon());
        }
    }

    [PunRPC]
    void RPC_Fire(ActivateEventArgs args)
    {
        if (!pV.IsMine)
        { return; }

        if (ammoLeft >= 1)
        {
            foreach (Transform t in spawnPoint)
            {
                audioSource.PlayOneShot(weaponFire);
                GameObject spawnedBullet = PhotonNetwork.Instantiate(bullet.name, t.position, Quaternion.identity);
                spawnedBullet.GetComponent<Bullet>().bulletModifier = player.GetComponentInParent<PlayerHealth>().bulletModifier;
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
        if (!pV.IsMine)
        { return; }

        reloadingScreen.SetActive(true);
        audioSource.PlayOneShot(weaponReload);
        durability--;
    }

    [PunRPC]
    void RPC_Reload2()
    {
        if (!pV.IsMine)
        { return; }

        ammoLeft = maxAmmo;
        reloadingScreen.SetActive(false);
        reloadingWeapon = false;
    }

    [PunRPC]
    void RPC_Trigger(Collider other)
    {
        if (!pV.IsMine)
        { return; }

        if (other.CompareTag("LeftHand") || other.CompareTag("RightHand"))
        {
            player = other.transform.parent.gameObject;
            var newMaxAmmo = player.GetComponentInParent<PlayerHealth>().maxAmmo + maxAmmo;
            maxAmmo = newMaxAmmo;
            GetComponent<Rigidbody>().isKinematic = false;
            rotatorScript.enabled = false;
        }
    }

    [PunRPC]
    void RPC_Destroy(Collider other)
    {
        if (!pV.IsMine)
        { return; }

        explosionObject.SetActive(true);
    }
}
