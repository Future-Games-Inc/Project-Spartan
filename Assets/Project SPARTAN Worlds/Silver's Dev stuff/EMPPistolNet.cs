using System.Collections;
using UnityEngine;
using TMPro;
using Umbrace.Unity.PurePool;

public class EMPPistolNet : MonoBehaviour
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
    public bool isFiring = false;
    public bool reloadingWeapon = false;

    public bool contact;
    public GameObjectPoolManager PoolManager;

    void OnEnable()
    {
        // Find the manager if one hasn't been specified.
        if (this.PoolManager == null)
        {
            this.PoolManager = Object.FindObjectOfType<GameObjectPoolManager>();
        }



        durability = 5;
        rotatorScript = GetComponent<Rotator>();
        reloadingScreen.SetActive(false);
        ammoLeft = maxAmmo;
        ammoText.text = ammoLeft.ToString();
        StartCoroutine(TextUpdate());
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
        if (ammoLeft <= 0)
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
        StartCoroutine(FireBullet());
    }

    public void StopFireBullet()
    {
        isFiring = false;
        StopCoroutine(FireBullet());
    }

    IEnumerator FireBullet()
    {
        while (isFiring)
        {
            if (ammoLeft >= 1 && reloadingWeapon == false)
            {
                foreach (Transform t in spawnPoint)
                {
                    GameObject spawnedBullet = this.PoolManager.Acquire(Bullet, t.position, Quaternion.identity);
                    spawnedBullet.GetComponent<BulletBehaviorNet>().audioSource.PlayOneShot(spawnedBullet.GetComponent<BulletBehaviorNet>().clip);
                    spawnedBullet.GetComponent<Rigidbody>().velocity = spawnPoint[0].forward * spawnedBullet.GetComponent<BulletBehaviorNet>().TravelSpeed;
                    spawnedBullet.gameObject.GetComponent<BulletBehaviorNet>().bulletOwner = player.gameObject;
                    spawnedBullet.gameObject.GetComponent<BulletBehaviorNet>().playerBullet = true;
                }
            }
            ammoLeft--;

            if (ammoLeft <= 0 && reloadingWeapon == false)
            {
                reloadingWeapon = true;
                StartCoroutine(Reload());
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator Reload()
    {
        StopCoroutine(FireBullet());
        reloadingScreen.SetActive(true);
        audioSource.PlayOneShot(reloadSFX);
        durability--;
        yield return new WaitForSeconds(2);
        ammoLeft = maxAmmo;
        reloadingScreen.SetActive(false);
        reloadingWeapon = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LeftHand") || other.CompareTag("RightHand"))
        {
            player = other.transform.root.gameObject;
            var newMaxAmmo = player.GetComponent<PlayerHealth>().maxAmmo + maxAmmo;
            maxAmmo = newMaxAmmo;
            rotatorScript.enabled = false;
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

    public void Rescale()
    {
        this.gameObject.transform.localScale = Vector3.one;
    }
}
