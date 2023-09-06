using System.Collections;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class EMPPistolNet : MonoBehaviourPunCallbacks
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

    void OnEnable()
    {
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
                    GameObject spawnedBullet = PhotonNetwork.Instantiate(Bullet.name, t.position, Quaternion.identity, 0, null);
                    spawnedBullet.GetComponent<BulletBehaviorNet>().audioSource.PlayOneShot(spawnedBullet.GetComponent<BulletBehaviorNet>().clip);
                    spawnedBullet.GetComponent<Rigidbody>().velocity = spawnPoint[0].forward * spawnedBullet.GetComponent<BulletBehaviorNet>().TravelSpeed;
                    spawnedBullet.gameObject.GetComponent<BulletBehaviorNet>().bulletOwner = player.gameObject;
                    spawnedBullet.gameObject.GetComponent<BulletBehaviorNet>().playerBullet = true;
                }
            }
            photonView.RPC("RPC_EMPFire", RpcTarget.All);
            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator Reload()
    {
        photonView.RPC("RPC_EMPReload", RpcTarget.All);
        yield return new WaitForSeconds(2);
        photonView.RPC("RPC_EMPReload2", RpcTarget.All);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LeftHand") || other.CompareTag("RightHand"))
        {
            player = other.transform.root.gameObject;
            photonView.RPC("RPC_EMPTrigger", RpcTarget.All);
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
    void RPC_EMPFire()
    {
        if (!photonView.IsMine)
            return;
        ammoLeft--;

        if (ammoLeft <= 0 && reloadingWeapon == false)
        {
            reloadingWeapon = true;
            StartCoroutine(Reload());
        }
    }

    [PunRPC]
    void RPC_EMPReload()
    {
        if (!photonView.IsMine)
            return;
        StopCoroutine(FireBullet());
        reloadingScreen.SetActive(true);
        audioSource.PlayOneShot(reloadSFX);
        durability--;
    }

    [PunRPC]
    void RPC_EMPReload2()
    {
        if (!photonView.IsMine)
            return;
        ammoLeft = maxAmmo;
        reloadingScreen.SetActive(false);
        reloadingWeapon = false;
    }

    [PunRPC]
    void RPC_EMPTrigger()
    {
        if (!photonView.IsMine)
            return;
        var newMaxAmmo = player.GetComponentInParent<PlayerHealth>().maxAmmo + maxAmmo;
        maxAmmo = newMaxAmmo;
        rotatorScript.enabled = false;
    }

    public void Rescale()
    {
        if (!photonView.IsMine)
            return;
        this.gameObject.transform.localScale = Vector3.one;
    }
}
