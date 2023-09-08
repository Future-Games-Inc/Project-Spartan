using System.Collections;
using UnityEngine;
using TMPro;
using PathologicalGames;
using Umbrace.Unity.PurePool;

public class StingerShotgun : MonoBehaviour
{
    [Header("Spawning ---------------------------------------")]
    public Transform[] spawnPoint;

    [Header("Audio ------------------------------------------")]
    public AudioSource audioSource;
    public AudioClip weaponReload;
    public AudioClip weaponBreak;

    [Header("Gun Mechanics ----------------------------------")]
    public GameObject player;
    public Rotator rotatorScript;
    public GameObject explosionObject;

    public int maxAmmo;
    public int ammoLeft;
    public int durability;

    public GameObject reloadingScreen;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI durabilityText;

    public bool reloadingWeapon = false;
    public bool isFiring = false;

    [Header("Bullet Mechanics -------------------------------")]
    public float fireSpeed = 20;
    public GameObject bullet;

    public GameObjectPoolManager PoolManager;

    // Start is called before the first frame update
    void OnEnable()
    {
        PoolManager = GameObject.FindGameObjectWithTag("Pool").GetComponent<GameObjectPoolManager>();
        durability = 5;
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
                    GameObject spawnedBullet = this.PoolManager.Acquire(bullet, t.position, Quaternion.identity);
                    spawnedBullet.GetComponent<StingerBulletNet>().audioSource.PlayOneShot(spawnedBullet.GetComponent<StingerBulletNet>().clip);
                    spawnedBullet.GetComponent<Rigidbody>().velocity = t.forward * fireSpeed;
                    spawnedBullet.GetComponent<StingerBulletNet>().bulletModifier = player.GetComponent<PlayerHealth>().bulletModifier;
                    spawnedBullet.gameObject.GetComponent<StingerBulletNet>().bulletOwner = player.gameObject;
                    spawnedBullet.gameObject.GetComponent<StingerBulletNet>().playerBullet = true;
                }
            }
            ammoLeft--;

            if (ammoLeft <= 0 && reloadingWeapon == false)
            {
                reloadingWeapon = true;
                StartCoroutine(ReloadWeapon());
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator ReloadWeapon()
    {
        StopCoroutine(FireBullet());
        reloadingScreen.SetActive(true);
        audioSource.PlayOneShot(weaponReload);
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
