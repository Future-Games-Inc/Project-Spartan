using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MultiLauncher : MonoBehaviourPunCallbacks
{
    public enum Launcher
    {
        Sticky,
        Blackout,
        Gravity,
    }

    [Header("Spawning ---------------------------------------")]
    public Transform[] spawnPoint;

    [Header("Audio ------------------------------------------")]
    public AudioSource audioSource;

    public AudioClip shootSFX;
    public AudioClip reloadSFX;
    public AudioClip weaponBreak;

    [Header("Gun Mechanics ----------------------------------")]
    public float fireSpeed = 20;

    public int maxAmmo;
    public int ammoLeft;
    public int durability;

    public bool reloadingWeapon = false;
    public bool isFiring = false;

    public GameObject reloadingScreen;
    public GameObject player;
    public GameObject explosionObject;

    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI durabilityText;

    public Launcher activeLauncher = Launcher.Sticky;

    public Rotator rotatorScript;

    [Header("Bullet Mechanics -------------------------------")]
    public GameObject stickyBullet;
    public GameObject gravityBullet;
    public GameObject blackoutBullet;

    // Start is called before the first frame update
    void OnEnable()
    {
        rotatorScript = GetComponent<Rotator>();
        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
        grabbable.activated.AddListener(StartFireBullet);
        grabbable.deactivated.AddListener(StopFireBullet);
        photonView.RPC("RPC_MultiStart", RpcTarget.All);
        StartCoroutine(TextUpdate());
    }

    IEnumerator TextUpdate()
    {
        while (true)
        {
            photonView.RPC("RPC_MultiLauncherText", RpcTarget.All);
            yield return new WaitForSeconds(.15f);
        }
    }

    public void StartFireBullet(ActivateEventArgs arg)
    {
        isFiring = true;
        CheckForLauncher();
    }

    public void StopFireBullet(DeactivateEventArgs arg)
    {
        isFiring = false;
    }

    public void StickyActive()
    {
        activeLauncher = Launcher.Sticky;
    }

    public void GravityActive()
    {
        activeLauncher = Launcher.Gravity;
    }

    public void BlackOutActive()
    {
        activeLauncher = Launcher.Blackout;
    }

    private void CheckForLauncher()
    {
        switch (activeLauncher)
        {
            case Launcher.Sticky:
                StartCoroutine(Sticky());
                break;
            case Launcher.Gravity:
                StartCoroutine(Gravity());
                break;
            case Launcher.Blackout:
                StartCoroutine(Blackout());
                break;
        }
    }

    IEnumerator Reload()
    {
        photonView.RPC("RPC_MultiReload", RpcTarget.All);
        yield return new WaitForSeconds(2);
        photonView.RPC("RPC_MultiReload2", RpcTarget.All);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LeftHand") || other.CompareTag("RightHand"))
        {
            player = other.transform.root.gameObject;
            photonView.RPC("RPC_MultiTrigger", RpcTarget.All);
        }
    }

    IEnumerator DestroyWeapon()
    {
        yield return new WaitForSeconds(0.5f);
        photonView.RPC("RPC_MultiDestroy", RpcTarget.All);
        yield return new WaitForSeconds(0.5f);
        PhotonNetwork.Destroy(gameObject);
    }

    IEnumerator Sticky()
    {
        while (isFiring)
        {
            if (ammoLeft >= 1 && reloadingWeapon == false)
            {
                if (!audioSource.isPlaying)
                    audioSource.PlayOneShot(shootSFX);
                foreach (Transform t in spawnPoint)
                {
                    GameObject spawnedBullet = PhotonNetwork.InstantiateRoomObject(stickyBullet.name, t.position, Quaternion.identity, 0, null);
                    spawnedBullet.GetComponent<Rigidbody>().velocity = t.forward * fireSpeed;
                    spawnedBullet.GetComponent<Bullet>().bulletModifier = player.GetComponent<PlayerHealth>().bulletModifier;
                    spawnedBullet.gameObject.GetComponent<Bullet>().bulletOwner = player.gameObject;
                    spawnedBullet.gameObject.GetComponent<Bullet>().playerBullet = true;
                }

                photonView.RPC("RPC_MultiLauncherFire", RpcTarget.All);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    IEnumerator Gravity()
    {
        while (isFiring)
        {
            if (ammoLeft >= 1 && reloadingWeapon == false)
            {
                if (!audioSource.isPlaying)
                    audioSource.PlayOneShot(shootSFX);
                foreach (Transform t in spawnPoint)
                {
                    GameObject spawnedBullet = PhotonNetwork.InstantiateRoomObject(gravityBullet.name, t.position, Quaternion.identity, 0, null);
                    spawnedBullet.GetComponent<Rigidbody>().velocity = t.forward * fireSpeed;
                    spawnedBullet.GetComponent<Bullet>().bulletModifier = player.GetComponent<PlayerHealth>().bulletModifier;
                    spawnedBullet.gameObject.GetComponent<Bullet>().bulletOwner = player.gameObject;
                    spawnedBullet.gameObject.GetComponent<Bullet>().playerBullet = true;
                }

                photonView.RPC("RPC_MultiLauncherFire", RpcTarget.All);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    IEnumerator Blackout()
    {
        while (isFiring)
        {
            if (ammoLeft >= 1 && reloadingWeapon == false)
            {
                if (!audioSource.isPlaying)
                    audioSource.PlayOneShot(shootSFX);
                foreach (Transform t in spawnPoint)
                {
                    GameObject spawnedBullet = PhotonNetwork.InstantiateRoomObject(blackoutBullet.name, t.position, Quaternion.identity, 0, null);
                    spawnedBullet.GetComponent<Rigidbody>().velocity = t.forward * fireSpeed;
                    spawnedBullet.GetComponent<Bullet>().bulletModifier = player.GetComponent<PlayerHealth>().bulletModifier;
                    spawnedBullet.gameObject.GetComponent<Bullet>().bulletOwner = player.gameObject;
                    spawnedBullet.gameObject.GetComponent<Bullet>().playerBullet = true;
                }

                photonView.RPC("RPC_MultiLauncherFire", RpcTarget.All);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    [PunRPC]
    void RPC_MultiStart()
    {
        reloadingScreen.SetActive(false);
        ammoLeft = maxAmmo;
        ammoText.text = ammoLeft.ToString();
    }

    [PunRPC]
    void RPC_MultiLauncherText()
    {
        ammoText.text = ammoLeft.ToString();
        durabilityText.text = durability.ToString();
    }

    [PunRPC]
    void RPC_RPC_MultiLauncherFire()
    {
        ammoLeft--;

        if (ammoLeft <= 0 && reloadingWeapon == false)
        {
            reloadingWeapon = true;
            StartCoroutine(Reload());
        }
    }

    [PunRPC]
    void RPC_MultiReload()
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
    void RPC_MultiReload2()
    {
        ammoLeft = maxAmmo;
        reloadingScreen.SetActive(false);
        reloadingWeapon = false;
    }

    [PunRPC]
    void RPC_MultiTrigger()
    {
        var newMaxAmmo = player.GetComponentInParent<PlayerHealth>().maxAmmo + maxAmmo;
        maxAmmo = newMaxAmmo;
        GetComponent<Rigidbody>().isKinematic = false;
        rotatorScript.enabled = false;
    }

    [PunRPC]
    void RPC_MultiDestroy()
    {
        explosionObject.SetActive(true);
    }
}
