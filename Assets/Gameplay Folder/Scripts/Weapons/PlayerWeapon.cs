using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using System.Threading.Tasks;

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
    public AudioClip weaponReload;
    public AudioClip weaponBreak;

    public bool reloadingWeapon = false;
    public bool isFiring = false;

    // Start is called before the first frame update
    public override void OnEnable()
    {
        base.OnEnable();
        durability = 5;
        rotatorScript = GetComponent<Rotator>();
        TextUpdate();
        reloadingScreen.SetActive(false);
        ammoLeft = maxAmmo;
        ammoText.text = ammoLeft.ToString();
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.AddCallbackTarget(this);
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    async void TextUpdate()
    {
        while (true)
        {
            UpdateText();
            //photonView.RPC("RPC_Update", RpcTarget.All);
            await Task.Delay(150);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ammoLeft <= 0) // Maybe change this in the future
            ammoLeft = 0;

        if (durability <= 0)
        {
            audioSource.PlayOneShot(weaponBreak);
            DestroyWeapon();
        }
    }

    public void StartFireBullet()
    {
        isFiring = true;
        FireBullet();
    }

    public void StopFireBullet()
    {
        isFiring = false;
        photonView.RPC("RPC_UpdateAmmoAndDurability", RpcTarget.All, photonView.ViewID, ammoLeft, durability);

    }

    async void FireBullet()
    {
        while (isFiring)
        {
            if (ammoLeft >= 1 && !reloadingWeapon)
            {
                foreach (Transform t in spawnPoint)
                {
                    GameObject spawnedBullet = PhotonNetwork.Instantiate(bullet.name, t.position, Quaternion.identity, 0, null);
                    spawnedBullet.GetComponent<Bullet>().audioSource.PlayOneShot(spawnedBullet.GetComponent<Bullet>().clip);
                    spawnedBullet.GetComponent<Rigidbody>().velocity = t.forward * fireSpeed;
                    spawnedBullet.GetComponent<Bullet>().bulletModifier = player.GetComponentInParent<PlayerHealth>().bulletModifier;
                    spawnedBullet.gameObject.GetComponent<Bullet>().bulletOwner = player.gameObject;
                    spawnedBullet.gameObject.GetComponent<Bullet>().playerBullet = true;
                }
            }

            Fire();
            await Task.Delay(200);
        }
    }

    async void ReloadWeapon()
    {
        StopFireBullet();
        ammoText.gameObject.SetActive(false);
        reloadingScreen.SetActive(true);
        audioSource.PlayOneShot(weaponReload);

        await Task.Delay(2000);

        photonView.RPC("RPC_Reload", RpcTarget.All, photonView.ViewID);

        reloadingScreen.SetActive(false);
        ammoText.gameObject.SetActive(true);
        reloadingWeapon = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LeftHand") || other.CompareTag("RightHand"))
        {
            player = other.transform.root.gameObject;
            photonView.RPC("RPC_Trigger", RpcTarget.All);
        }
    }

    async void DestroyWeapon()
    {
        await Task.Delay(500);
        explosionObject.SetActive(true);
        await Task.Delay(500);
        PhotonNetwork.Destroy(gameObject);
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
            ReloadWeapon();
        }
    }

    [PunRPC]
    void RPC_UpdateAmmoAndDurability(int callingPhotonView, int ammoLeft, int durability)
    {
        if (photonView.ViewID != callingPhotonView)
        {
            return;
        }

        this.ammoLeft = ammoLeft;
        this.durability = durability;
    }


    [PunRPC]
    void RPC_Reload(int callingPhotonView)
    {
        if (photonView.ViewID != callingPhotonView)
        {
            return;
        }
        durability--;

        ammoLeft = maxAmmo;
    }

    [PunRPC]
    void RPC_Trigger()
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
