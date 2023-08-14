using System.Collections;
using UnityEngine;
using Photon.Pun;
using TMPro;
using BNG;

public class PulseARNet : MonoBehaviourPunCallbacks
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

    public Grabbable grabbable;
    public GrabbableUnityEvents grabbableEvents;

    // Start is called before the first frame update
    void OnEnable()
    {
        durability = 5;
        rotatorScript = GetComponent<Rotator>();
        photonView.RPC("RPC_PulseStart", RpcTarget.All);
        StartCoroutine(TextUpdate());
    }
    IEnumerator TextUpdate()
    {
        while (true)
        {
            photonView.RPC("RPC_PulseText", RpcTarget.All);
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
        if (grabbable.BeingHeldWithTwoHands)
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
        isTriggerSingle = state;
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
                        GameObject spawnedBullet = PhotonNetwork.InstantiateRoomObject(PulseBullet.name, t.position, Quaternion.identity, 0, null);
                        audioSource.PlayOneShot(spawnedBullet.GetComponent<BulletBehaviorNet>().clip);
                        spawnedBullet.GetComponent<Rigidbody>().velocity = t.forward * spawnedBullet.GetComponent<BulletBehaviorNet>().TravelSpeed;
                        spawnedBullet.GetComponent<Bullet>().bulletModifier = player.GetComponent<PlayerHealth>().bulletModifier;
                        spawnedBullet.gameObject.GetComponent<Bullet>().bulletOwner = player.gameObject;
                        spawnedBullet.gameObject.GetComponent<Bullet>().playerBullet = true;
                    }
                    photonView.RPC("RPC_PulseDualFire", RpcTarget.All);
                    yield return new WaitForSeconds(0.5f);
                }

            }

            // normal fire mode
            else
            {
                if (ammoLeft >= 1 && reloadingWeapon == false)
                {
                    foreach (Transform t in spawnPoint)
                    {
                        GameObject spawnedBullet = PhotonNetwork.InstantiateRoomObject(playerBullet.name, spawnPoint[0].position, Quaternion.identity, 0, null);
                        audioSource.PlayOneShot(spawnedBullet.GetComponent<BulletBehaviorNet>().clip);
                        spawnedBullet.GetComponent<Rigidbody>().velocity = spawnPoint[0].forward * spawnedBullet.GetComponent<BulletBehaviorNet>().TravelSpeed;
                        spawnedBullet.GetComponent<Bullet>().bulletModifier = player.GetComponent<PlayerHealth>().bulletModifier;
                        spawnedBullet.gameObject.GetComponent<Bullet>().bulletOwner = player.gameObject;
                        spawnedBullet.gameObject.GetComponent<Bullet>().playerBullet = true;
                    }
                    photonView.RPC("RPC_PulseSingleFire", RpcTarget.All);
                    yield return new WaitForSeconds(0.2f);
                }
            }
        }
    }

    IEnumerator Reload()
    {
        photonView.RPC("RPC_PulseReload", RpcTarget.All);
        yield return new WaitForSeconds(ReloadDuration);
        photonView.RPC("RPC_PulseReload2", RpcTarget.All);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LeftHand") || other.CompareTag("RightHand"))
        {
            player = other.transform.parent.gameObject;
            photonView.RPC("RPC_PulseTrigger", RpcTarget.All);
        }
    }

    IEnumerator DestroyWeapon()
    {
        yield return new WaitForSeconds(0.5f);
        photonView.RPC("RPC_PulseDestroy", RpcTarget.All);
        yield return new WaitForSeconds(0.5f);
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    void RPC_PulseStart()
    {
        reloadingScreen.SetActive(false);
        ammoLeft = maxAmmo;
        ammoText.text = ammoLeft.ToString();
    }

    [PunRPC]
    void RPC_PulseText()
    {
        ammoText.text = ammoLeft.ToString();
        durabilityText.text = durability.ToString();
    }

    [PunRPC]
    void RPC_PulseDualFire()
    {
        ammoLeft -= 3;

        if (ammoLeft <= 0 && reloadingWeapon == false)
        {
            reloadingWeapon = true;
            StartCoroutine(Reload());
        }
    }

    [PunRPC]
    void RPC_PulseSingleFire()
    {
        ammoLeft--;

        if (ammoLeft <= 0 && reloadingWeapon == false)
        {
            reloadingWeapon = true;
            StartCoroutine(Reload());
        }
    }

    [PunRPC]
    void RPC_PulseReload()
    {
        StopCoroutine(FireBullet());
        reloadingScreen.SetActive(true);
        audioSource.PlayOneShot(reloadSFX);
        durability--;

        if (durability <= 0)
        {
            audioSource.PlayOneShot(weaponBreak);
            StartCoroutine(DestroyWeapon());
        }
    }

    [PunRPC]
    void RPC_PulseReload2()
    {
        ammoLeft = maxAmmo;
        reloadingScreen.SetActive(false);
        reloadingWeapon = false;
    }

    [PunRPC]
    void RPC_PulseTrigger()
    {
        var newMaxAmmo = player.GetComponentInParent<PlayerHealth>().maxAmmo + maxAmmo;
        maxAmmo = newMaxAmmo;
        rotatorScript.enabled = false;
    }

    [PunRPC]
    void RPC_PulseDestroy()
    {
        explosionObject.SetActive(true);
    }

    public void Rescale()
    {
        this.gameObject.transform.localScale = Vector3.one;
    }
}
