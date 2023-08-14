using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class PlayerWeapon : MonoBehaviourPunCallbacks
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
    public bool isFiring = false;

    // Start is called before the first frame update
    void OnEnable()
    {
        durability = 5;
        rotatorScript = GetComponent<Rotator>();
        photonView.RPC("RPC_Start", RpcTarget.All);
        StartCoroutine(TextUpdate());
    }

    IEnumerator TextUpdate()
    {
        while (true)
        {
            photonView.RPC("RPC_Update", RpcTarget.All);
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
                    spawnedBullet.GetComponent<Bullet>().bulletModifier = player.GetComponentInParent<PlayerHealth>().bulletModifier;
                    spawnedBullet.gameObject.GetComponent<Bullet>().bulletOwner = player.gameObject;
                    spawnedBullet.gameObject.GetComponent<Bullet>().playerBullet = true;
                }
            }

            photonView.RPC("RPC_Fire", RpcTarget.All);
            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator ReloadWeapon()
    {
        photonView.RPC("RPC_Reload", RpcTarget.All);
        yield return new WaitForSeconds(2);
        photonView.RPC("RPC_Reload2", RpcTarget.All);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LeftHand") || other.CompareTag("RightHand"))
        {
            player = other.transform.root.gameObject;
            photonView.RPC("RPC_Trigger", RpcTarget.All);
        }
    }

    IEnumerator DestroyWeapon()
    {
        yield return new WaitForSeconds(0.5f);
        photonView.RPC("RPC_Destroy", RpcTarget.All);
        yield return new WaitForSeconds(0.5f);
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    void RPC_Start()
    {
        reloadingScreen.SetActive(false);
        ammoLeft = maxAmmo;
        ammoText.text = ammoLeft.ToString();
    }

    [PunRPC]
    void RPC_Update()
    {
        ammoText.text = ammoLeft.ToString();
        durabilityText.text = durability.ToString();
    }

    [PunRPC]
    void RPC_Fire()
    {
        ammoLeft--;

        if (ammoLeft <= 0 && reloadingWeapon == false)
        {
            reloadingWeapon = true;
            StartCoroutine(ReloadWeapon());
        }
    }

    [PunRPC]
    void RPC_Reload()
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
    void RPC_Reload2()
    {
        ammoLeft = maxAmmo;
        reloadingScreen.SetActive(false);
        reloadingWeapon = false;
    }

    [PunRPC]
    void RPC_Trigger()
    {
        var newMaxAmmo = player.GetComponentInParent<PlayerHealth>().maxAmmo + maxAmmo;
        maxAmmo = newMaxAmmo;
        rotatorScript.enabled = false;
    }

    [PunRPC]
    void RPC_Destroy()
    {
        explosionObject.SetActive(true);
    }

    public void Rescale()
    {
        this.gameObject.transform.localScale = Vector3.one;
    }
}
