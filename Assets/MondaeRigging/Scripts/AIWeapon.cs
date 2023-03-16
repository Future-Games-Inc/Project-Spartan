using System.Collections;
using UnityEngine;
using Photon.Pun;

public class AIWeapon : MonoBehaviourPunCallbacks
{
    public GameObject bullet;
    public Transform bulletTransform;
    public float shootForce;

    public int ammoLeft;
    public int maxAmmo;

    public AudioSource audioSource;
    public AudioSource audioSource2;
    public AudioClip weaponFire;
    public AudioClip weaponReload;

    public bool fireWeaponBool = false;
    public bool canShoot = true;
    // Start is called before the first frame update
    void Start()
    {
        photonView.RPC("RPC_Start", RpcTarget.AllBuffered);
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator Fire()
    {
        while (true)
        {
            if (ammoLeft >= 1 && fireWeaponBool == true && canShoot == true)
            {
                if (!audioSource.isPlaying)
                {
                    photonView.RPC("RPC_ShootAudio", RpcTarget.AllBuffered);
                }
                GameObject spawnedBullet = PhotonNetwork.InstantiateRoomObject(bullet.name, bulletTransform.position, Quaternion.identity, 0);
                if (this.gameObject.tag == "Enemy")
                {
                    spawnedBullet.GetComponent<Bullet>().bulletModifier = (int)Random.Range(0, 3);
                }
                else if (this.gameObject.tag == "BossEnemy")
                {
                    spawnedBullet.GetComponent<Bullet>().bulletModifier = (int)Random.Range(2, 5);
                }
                shootForce = (int)Random.Range(40, 75);
                spawnedBullet.GetComponent<Rigidbody>().velocity = bulletTransform.right * shootForce;

                ammoLeft--;
                yield return new WaitForSeconds(Random.Range(0.1f, 0.54f));
            }

            else if (ammoLeft <= 0)
            {
                canShoot = false;
                StartCoroutine(ReloadWeapon());
            }

            yield return null;
        }
    }

    IEnumerator ReloadWeapon()
    {
        yield return new WaitForSeconds(0);
        if (!audioSource2.isPlaying)
        {
            photonView.RPC("RPC_Reload1", RpcTarget.AllBuffered);
        }
        yield return new WaitForSeconds(2);
        photonView.RPC("RPC_Reload2", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void RPC_Start()
    {
        if (gameObject.tag == "Enemy")
        {
            maxAmmo = 10;
        }
        else if (gameObject.tag == "BossEnemy")
        {
            maxAmmo = 15;
        }
        ammoLeft = maxAmmo;
        StartCoroutine(Fire());
    }

    [PunRPC]
    void RPC_ShootAudio()
    {
        audioSource.PlayOneShot(weaponFire);
    }

    [PunRPC]
    void RPC_Reload1()
    {
        audioSource2.PlayOneShot(weaponReload);
    }

    [PunRPC]
    void RPC_Reload2()
    {
        ammoLeft = maxAmmo;
        canShoot = true;
    }


}
