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
                    GameObject spawnedBullet = PhotonNetwork.Instantiate(bullet.name, t.position, Quaternion.identity, 0, null);
                    spawnedBullet.GetComponent<StingerBulletNet>().audioSource.PlayOneShot(spawnedBullet.GetComponent<StingerBulletNet>().clip);
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
        explosionObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    void RPC_StingerFire()
    {
        if (!photonView.IsMine)
            return;
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
        if (!photonView.IsMine)
            return;
        StopCoroutine(FireBullet());
        reloadingScreen.SetActive(true);
        audioSource.PlayOneShot(weaponReload);
        durability--;
    }

    [PunRPC]
    void RPC_StingerReload2()
    {
        if (!photonView.IsMine)
            return;
        ammoLeft = maxAmmo;
        reloadingScreen.SetActive(false);
        reloadingWeapon = false;
    }

    [PunRPC]
    void RPC_StingerTrigger()
    {
        if (!photonView.IsMine)
            return;
        var newMaxAmmo = player.GetComponentInParent<PlayerHealth>().maxAmmo + maxAmmo;
        maxAmmo = newMaxAmmo;
        rotatorScript.enabled = false;
    }

    public void Rescale()
    {
        this.gameObject.transform.localScale = Vector3.one;
    }

}
