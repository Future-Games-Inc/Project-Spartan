using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;

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
    public GameObject stickyBulletIcon;
    public GameObject gravityBulletIcon;
    public GameObject blackoutBulletIcon;

    // Start is called before the first frame update
    void OnEnable()
    {
        rotatorScript = GetComponent<Rotator>();
        reloadingScreen.SetActive(false);
        ammoLeft = maxAmmo;
        ammoText.text = ammoLeft.ToString();
        StartCoroutine(TextUpdate());
    }

    IEnumerator TextUpdate()
    {
        while (true)
        {
            ammoText.text = ammoLeft.ToString();
            durabilityText.text = durability.ToString();
            yield return new WaitForSeconds(.15f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ammoLeft <= 0) // Maybe change this in the future
            ammoLeft = 0;

        if (durability <= 0)
        {
            audioSource.PlayOneShot(weaponBreak);
            DestroyWeapon();
        }
    }

    public void StartFireBullet()
    {
        isFiring = true;
        CheckForLauncher();
    }

    public void StopFireBullet()
    {
        isFiring = false;
    }

    public void StickyActive()
    {
        activeLauncher = Launcher.Sticky;
        stickyBulletIcon.SetActive(true);
        gravityBulletIcon.SetActive(false);
        blackoutBulletIcon.SetActive(false);
    }

    public void GravityActive()
    {
        activeLauncher = Launcher.Gravity;
        gravityBulletIcon.SetActive(true);
        stickyBulletIcon.SetActive(false);
        blackoutBulletIcon.SetActive(false);
    }

    public void BlackOutActive()
    {
        activeLauncher = Launcher.Blackout;
        blackoutBulletIcon.SetActive(true);
        gravityBulletIcon.SetActive(false);
        stickyBulletIcon.SetActive(false);
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
        explosionObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        PhotonNetwork.Destroy(gameObject);
    }

    IEnumerator Sticky()
    {
        while (isFiring)
        {
            if (ammoLeft >= 1 && reloadingWeapon == false)
            {
                foreach (Transform t in spawnPoint)
                {
                    GameObject spawnedBullet = PhotonNetwork.InstantiateRoomObject(stickyBullet.name, t.position, Quaternion.identity, 0, null);
                    spawnedBullet.GetComponent<StickyBullet>().audioSource.PlayOneShot(spawnedBullet.GetComponent<StickyBullet>().clip);
                    spawnedBullet.GetComponent<Rigidbody>().velocity = t.forward * fireSpeed;
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
                foreach (Transform t in spawnPoint)
                {
                    GameObject spawnedBullet = PhotonNetwork.InstantiateRoomObject(gravityBullet.name, t.position, Quaternion.identity, 0, null);
                    spawnedBullet.GetComponent<GravityBullet>().audioSource.PlayOneShot(spawnedBullet.GetComponent<GravityBullet>().clip);
                    spawnedBullet.GetComponent<Rigidbody>().velocity = t.forward * fireSpeed;
                    spawnedBullet.gameObject.GetComponent<GravityBullet>().bulletOwner = player.gameObject;
                    spawnedBullet.gameObject.GetComponent<GravityBullet>().playerBullet = true;
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
                foreach (Transform t in spawnPoint)
                {
                    GameObject spawnedBullet = PhotonNetwork.InstantiateRoomObject(blackoutBullet.name, t.position, Quaternion.identity, 0, null);
                    spawnedBullet.GetComponent<BlackoutBullet>().audioSource.PlayOneShot(spawnedBullet.GetComponent<BlackoutBullet>().clip);
                    spawnedBullet.GetComponent<Rigidbody>().velocity = t.forward * fireSpeed;
                    spawnedBullet.gameObject.GetComponent<BlackoutBullet>().bulletOwner = player.gameObject;
                    spawnedBullet.gameObject.GetComponent<BlackoutBullet>().playerBullet = true;
                }

                photonView.RPC("RPC_MultiLauncherFire", RpcTarget.All);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    [PunRPC]
    void RPC_MultiLauncherFire()
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
    }
}
