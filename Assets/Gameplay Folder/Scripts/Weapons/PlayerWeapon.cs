using System.Collections;
using UnityEngine;
using TMPro;
using Umbrace.Unity.PurePool;

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
    public AudioClip weaponReload;
    public AudioClip weaponBreak;

    public bool reloadingWeapon = false;
    public bool isFiring = false;

    public GameObjectPoolManager PoolManager;


    // Start is called before the first frame update
    public void OnEnable()
    {
        PoolManager = GameObject.FindGameObjectWithTag("Pool").GetComponent<GameObjectPoolManager>();

        durability = 5;
        rotatorScript = GetComponent<Rotator>();
        reloadingScreen.SetActive(false);
        ammoLeft = maxAmmo;
        ammoText.text = ammoLeft.ToString();
        UpdateText();
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
        StartCoroutine(FireBullet());
    }

    public void StopFireBullet()
    {
        isFiring = false;
    }

   IEnumerator FireBullet()
    {
        while (isFiring)
        {
            if (ammoLeft >= 1 && !reloadingWeapon)
            {
                foreach (Transform t in spawnPoint)
                {
                    GameObject spawnedBullet = this.PoolManager.Acquire(bullet, t.position, Quaternion.identity);
                    spawnedBullet.GetComponent<Bullet>().audioSource.PlayOneShot(spawnedBullet.GetComponent<Bullet>().clip);
                    spawnedBullet.GetComponent<Rigidbody>().velocity = t.forward * fireSpeed;
                    spawnedBullet.GetComponent<Bullet>().bulletModifier = player.GetComponentInParent<PlayerHealth>().bulletModifier;
                    spawnedBullet.gameObject.GetComponent<Bullet>().bulletOwner = player.gameObject;
                    spawnedBullet.gameObject.GetComponent<Bullet>().playerBullet = true;
                }

                Fire();
            }
            yield return new WaitForSeconds(.2f);
        }
    }

    IEnumerator ReloadWeapon()
    {
        StopFireBullet();
        ammoText.gameObject.SetActive(false);
        reloadingScreen.SetActive(true);
        audioSource.PlayOneShot(weaponReload);

        yield return new WaitForSeconds(2f);

        durability--;

        ammoLeft = maxAmmo;
        UpdateText();

        reloadingScreen.SetActive(false);
        ammoText.gameObject.SetActive(true);
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
        yield return new WaitForSeconds(.5f);
        explosionObject.SetActive(true);
        yield return new WaitForSeconds(.5f);
        this.PoolManager.Release(gameObject);
    }

    void UpdateText()
    {
        ammoText.text = ammoLeft.ToString();
        durabilityText.text = durability.ToString();
    }


    void Fire()
    {
        ammoLeft--;

        if (ammoLeft <= 0 && !reloadingWeapon)
        {
            reloadingWeapon = true;
            StartCoroutine(ReloadWeapon());
        }
        UpdateText();
    }

    public void Rescale()
    {
        this.gameObject.transform.localScale = Vector3.one;
    }

    // ----------------------- OLD RPCs ----------------
    //[PunRPC]
    //void RPC_Update()
    //{
    //    ammoText.text = ammoLeft.ToString();
    //    durabilityText.text = durability.ToString();
    //}
    //[PunRPC]
    

    //[PunRPC]
    //void RPC_Reload2()
    //{
    //    if (!photonView.IsMine)
    //        return;

    //    ammoLeft = maxAmmo;
    //    reloadingScreen.SetActive(false);
    //    reloadingWeapon = false;
    //}

}
