using System.Collections;
using TMPro;
using Umbrace.Unity.PurePool;
using UnityEngine;

public class MultiLauncher : MonoBehaviour
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

    public GameObject stickyBulletSmoke;
    public GameObject gravityBulletSmoke;
    public GameObject blackoutBulletSmoke;

    public GameObject reactors;

    public Material stickyMaterial;
    public Material gravityMaterial;
    public Material blackoutMaterial;


    public bool contact;
    public GameObjectPoolManager PoolManager;

    // Start is called before the first frame update
    void OnEnable()
    {
        // Find the manager if one hasn't been specified.
        if (this.PoolManager == null)
        {
            this.PoolManager = Object.FindObjectOfType<GameObjectPoolManager>();
        }



        rotatorScript = GetComponent<Rotator>();
        reloadingScreen.SetActive(false);
        ammoLeft = maxAmmo;
        ammoText.text = ammoLeft.ToString();
        CheckForLauncher();
        UpdateText();

        StartCoroutine(NoContact());
    }
    IEnumerator NoContact()
    {
        yield return new WaitForSeconds(10);
        if (contact == false)
        {
            this.PoolManager.Release(gameObject);
        }
    }

    void UpdateText()
    {
        ammoText.text = ammoLeft.ToString();
        durabilityText.text = durability.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (ammoLeft <= 0) // Maybe change this in the future
            ammoLeft = 0;

        if (durability <= 0)
        {
            audioSource.PlayOneShot(weaponBreak);
            StartCoroutine(DestroyWeapon());
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
        reactors.GetComponent<MeshRenderer>().material = stickyMaterial;
        stickyBulletIcon.SetActive(true);
        gravityBulletIcon.SetActive(false);
        blackoutBulletIcon.SetActive(false);
        stickyBulletSmoke.SetActive(true);
        gravityBulletSmoke.SetActive(false);
        blackoutBulletSmoke.SetActive(false);
    }

    public void GravityActive()
    {
        activeLauncher = Launcher.Gravity;
        reactors.GetComponent<MeshRenderer>().material = gravityMaterial;
        gravityBulletIcon.SetActive(true);
        stickyBulletIcon.SetActive(false);
        blackoutBulletIcon.SetActive(false);
        stickyBulletSmoke.SetActive(false);
        gravityBulletSmoke.SetActive(true);
        blackoutBulletSmoke.SetActive(false);
    }

    public void BlackOutActive()
    {
        activeLauncher = Launcher.Blackout;
        reactors.GetComponent<MeshRenderer>().material = blackoutMaterial;
        blackoutBulletIcon.SetActive(true);
        gravityBulletIcon.SetActive(false);
        stickyBulletIcon.SetActive(false);
        stickyBulletSmoke.SetActive(false);
        gravityBulletSmoke.SetActive(false);
        blackoutBulletSmoke.SetActive(true);
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
        StopFireBullet();
        reloadingScreen.SetActive(true);
        audioSource.PlayOneShot(reloadSFX);
        durability--;
        yield return new WaitForSeconds(2);
        ammoLeft = maxAmmo;
        reloadingScreen.SetActive(false);
        reloadingWeapon = false;
        UpdateText();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LeftHand") || other.CompareTag("RightHand"))
        {
            player = other.transform.root.gameObject;
            var newMaxAmmo = player.GetComponentInParent<PlayerHealth>().maxAmmo + maxAmmo;
            maxAmmo = newMaxAmmo;
            contact = true;
        }
    }

    IEnumerator DestroyWeapon()
    {
        yield return new WaitForSeconds(0.5f);
        explosionObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        this.PoolManager.Release(gameObject);
    }

    IEnumerator Sticky()
    {
        while (isFiring)
        {
            if (ammoLeft >= 1 && reloadingWeapon == false)
            {
                    foreach (Transform t in spawnPoint)
                    {
                        GameObject spawnedBullet = this.PoolManager.Acquire(stickyBullet, t.position, Quaternion.identity);
                        spawnedBullet.GetComponent<StickyBullet>().audioSource.PlayOneShot(spawnedBullet.GetComponent<StickyBullet>().clip);
                        spawnedBullet.GetComponent<Rigidbody>().velocity = t.forward * fireSpeed;
                    }
                    Fire();
                }
            yield return new WaitForSeconds(0.2f);
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
                        GameObject spawnedBullet = this.PoolManager.Acquire(gravityBullet, t.position, Quaternion.identity);
                        spawnedBullet.GetComponent<GravityBullet>().audioSource.PlayOneShot(spawnedBullet.GetComponent<GravityBullet>().clip);
                        spawnedBullet.GetComponent<Rigidbody>().velocity = t.forward * fireSpeed;
                        spawnedBullet.gameObject.GetComponent<GravityBullet>().bulletOwner = player.gameObject;
                        spawnedBullet.gameObject.GetComponent<GravityBullet>().playerBullet = true;
                    }
                    Fire();
                }
            yield return new WaitForSeconds(0.2f);
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
                        GameObject spawnedBullet = this.PoolManager.Acquire(blackoutBullet, t.position, Quaternion.identity);
                        spawnedBullet.GetComponent<BlackoutBullet>().audioSource.PlayOneShot(spawnedBullet.GetComponent<BlackoutBullet>().clip);
                        spawnedBullet.GetComponent<Rigidbody>().velocity = t.forward * fireSpeed;
                        spawnedBullet.gameObject.GetComponent<BlackoutBullet>().bulletOwner = player.gameObject;
                        spawnedBullet.gameObject.GetComponent<BlackoutBullet>().playerBullet = true;
                    }
                    Fire();
                }
            yield return new WaitForSeconds(0.2f);
        }
    }

    void Fire()
    {
        ammoLeft--;

        if (ammoLeft <= 0 && reloadingWeapon == false)
        {
            reloadingWeapon = true;
            StartCoroutine(Reload());
        }

        UpdateText();
    }
}
