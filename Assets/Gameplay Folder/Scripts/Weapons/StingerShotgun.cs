using System.Collections;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class StingerShotgun : MonoBehaviourPunCallbacks
{
    [Header("Spawning ---------------------------------------")]
    public Transform[] spawnPoint;

    [Header("Audio ------------------------------------------")]
    public AudioSource audioSource;
    public AudioClip weaponFire;
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

    // Start is called before the first frame update
    void OnEnable()
    {
        durability = 5;
        photonView.RPC("RPC_StingerStart", RpcTarget.All);
        StartCoroutine(TextUpdate());
    }

    IEnumerator TextUpdate()
    {
        while (true)
        {
            photonView.RPC("RPC_StingerTextUpdate", RpcTarget.All);
            yield return new WaitForSeconds(.15f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ammoLeft <= 0)
            ammoLeft = 0;
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
                if (!audioSource.isPlaying)
                    audioSource.PlayOneShot(weaponFire);
                foreach (Transform t in spawnPoint)
                {
                    GameObject spawnedBullet = PhotonNetwork.Instantiate(bullet.name, t.position, Quaternion.identity, 0, null);
                    spawnedBullet.GetComponent<Rigidbody>().velocity = t.forward * fireSpeed;
                    spawnedBullet.GetComponent<StingerBulletNet>().bulletModifier = player.GetComponent<PlayerHealth>().bulletModifier;
                    spawnedBullet.gameObject.GetComponent<StingerBulletNet>().bulletOwner = player.gameObject;
                    spawnedBullet.gameObject.GetComponent<StingerBulletNet>().playerBullet = true;
                }
            }

            photonView.RPC("RPC_StingerFire", RpcTarget.All);
            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator ReloadWeapon()
    {
        photonView.RPC("RPC_StingerReload", RpcTarget.All);
        yield return new WaitForSeconds(2);
        photonView.RPC("RPC_StingerReload2", RpcTarget.All);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LeftHand") || other.CompareTag("RightHand"))
        {
            player = other.transform.root.gameObject;
            photonView.RPC("RPC_StingerTrigger", RpcTarget.All);
        }
    }

    IEnumerator DestroyWeapon()
    {
        yield return new WaitForSeconds(0.5f);
        photonView.RPC("RPC_StingerDestroy", RpcTarget.All);
        yield return new WaitForSeconds(0.5f);
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    void RPC_StingerStart()
    {
        reloadingScreen.SetActive(false);
        ammoLeft = maxAmmo;
        ammoText.text = ammoLeft.ToString();
    }

    [PunRPC]
    void RPC_StingerTextUpdate()
    {
        ammoText.text = ammoLeft.ToString();
        durabilityText.text = durability.ToString();
    }

    [PunRPC]
    void RPC_StingerFire()
    {
        ammoLeft--;

        if (ammoLeft <= 0 && reloadingWeapon == false)
        {
            reloadingWeapon = true;
            StartCoroutine(ReloadWeapon());
        }
    }

    [PunRPC]
    void RPC_StingerReload()
    {
        StopCoroutine(FireBullet());
        reloadingScreen.SetActive(true);
        audioSource.PlayOneShot(weaponReload);
        durability--;

        if (durability <= 0)
        {
            audioSource.PlayOneShot(weaponBreak);
            StartCoroutine(DestroyWeapon());
        }
    }

    [PunRPC]
    void RPC_StingerReload2()
    {
        ammoLeft = maxAmmo;
        reloadingScreen.SetActive(false);
        reloadingWeapon = false;
    }

    [PunRPC]
    void RPC_StingerTrigger()
    {
        var newMaxAmmo = player.GetComponentInParent<PlayerHealth>().maxAmmo + maxAmmo;
        maxAmmo = newMaxAmmo;
        rotatorScript.enabled = false;
    }

    [PunRPC]
    void RPC_StingerDestroy()
    {
        explosionObject.SetActive(true);
    }

    public void Rescale()
    {
        this.gameObject.transform.localScale = Vector3.one;
    }

}
