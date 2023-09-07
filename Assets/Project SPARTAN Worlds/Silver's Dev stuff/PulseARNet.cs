using System.Collections;
using UnityEngine;
using TMPro;
using BNG;
using PathologicalGames;
using Umbrace.Unity.PurePool;

public class PulseARNet : MonoBehaviour
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
    public GameObject playerBullet;
    public GameObject PulseBullet;
    [HideInInspector]
    public bool active = true;
    [HideInInspector]
    public bool isFiring = false;
    [HideInInspector]
    public bool reloadingWeapon = false;

    [Header("Keybinds ------------------------------------")]
    [HideInInspector]
    public bool isDual = false;
    [HideInInspector]
    public bool isTriggerSingle = false;

    public bool SecondaryTrigger = false;

    public GameObjectPoolManager PoolManager;


    // Start is called before the first frame update
    void OnEnable()
    {
        PoolManager = GameObject.FindGameObjectWithTag("Pool").GetComponent<GameObjectPoolManager>();

        durability = 5;
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
        if (SecondaryTrigger)
        {
            isFiring = true;
            StartCoroutine(FireBullet(true));
        }
        else
        {
            isFiring = true;
            StartCoroutine(FireBullet());
        }
    }

    public void StopFireBullet()
    {
        isFiring = false;
        StopCoroutine(FireBullet());
    }


    public void setHeld(bool state)
    {
        SecondaryTrigger = state;
    }

    IEnumerator FireBullet(bool pulse = false)
    {
        while (isFiring)
        {
            if (pulse)
            {
                // do the pulse fire thing
                if (ammoLeft >= 3 && reloadingWeapon == false)
                {
                    foreach (Transform t in spawnPoint)
                    {
                        GameObject spawnedBullet = this.PoolManager.Acquire(PulseBullet, t.position, Quaternion.identity);
                        spawnedBullet.GetComponent<BulletBehaviorNet>().audioSource.PlayOneShot(spawnedBullet.GetComponent<BulletBehaviorNet>().clip);
                        spawnedBullet.GetComponent<Rigidbody>().velocity = t.forward * spawnedBullet.GetComponent<BulletBehaviorNet>().TravelSpeed;
                        spawnedBullet.GetComponent<Bullet>().bulletModifier = player.GetComponentInParent<PlayerHealth>().bulletModifier;
                        spawnedBullet.gameObject.GetComponent<Bullet>().bulletOwner = player.gameObject;
                        spawnedBullet.gameObject.GetComponent<Bullet>().playerBullet = true;
                    }
                    ammoLeft -= 3;

                    if (ammoLeft <= 0 && reloadingWeapon == false)
                    {
                        reloadingWeapon = true;
                        StartCoroutine(Reload());
                    }
                }
                yield return new WaitForSeconds(0.5f);

            }

            // normal fire mode
            else
            {
                if (ammoLeft >= 1 && reloadingWeapon == false)
                {
                    foreach (Transform t in spawnPoint)
                    {
                        GameObject spawnedBullet = this.PoolManager.Acquire(playerBullet, spawnPoint[0].position, Quaternion.identity);
                        spawnedBullet.GetComponent<BulletBehaviorNet>().audioSource.PlayOneShot(spawnedBullet.GetComponent<BulletBehaviorNet>().clip);
                        spawnedBullet.GetComponent<Rigidbody>().velocity = spawnPoint[0].forward * spawnedBullet.GetComponent<BulletBehaviorNet>().TravelSpeed;
                        spawnedBullet.GetComponent<Bullet>().bulletModifier = player.GetComponentInParent<PlayerHealth>().bulletModifier;
                        spawnedBullet.gameObject.GetComponent<Bullet>().bulletOwner = player.gameObject;
                        spawnedBullet.gameObject.GetComponent<Bullet>().playerBullet = true;
                    }
                    ammoLeft--;

                    if (ammoLeft <= 0 && reloadingWeapon == false)
                    {
                        reloadingWeapon = true;
                        StartCoroutine(Reload());
                    }
                }
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    IEnumerator Reload()
    {
        StopCoroutine(FireBullet());
        reloadingScreen.SetActive(true);
        audioSource.PlayOneShot(reloadSFX);
        durability--;
        yield return new WaitForSeconds(ReloadDuration);
        ammoLeft = maxAmmo;
        reloadingScreen.SetActive(false);
        reloadingWeapon = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LeftHand") || other.CompareTag("RightHand"))
        {
            player = other.transform.parent.gameObject;
            var newMaxAmmo = player.GetComponentInParent<PlayerHealth>().maxAmmo + maxAmmo;
            maxAmmo = newMaxAmmo;
            rotatorScript.enabled = false;
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
