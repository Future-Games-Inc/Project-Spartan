using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AIWeapon : MonoBehaviour
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

    public bool fireWeaponBool;
    // Start is called before the first frame update
    void Start()
    {
        fireWeaponBool = true;
        if (this.transform.parent.gameObject.tag == "Enemy")
        {
            maxAmmo = 15;
        }
        else if (this.transform.parent.gameObject.tag == "BossEnemy")
        {
            maxAmmo = 20;
        }
        ammoLeft = maxAmmo;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Fire()
    {     
        if (ammoLeft >= 1)
        {
            fireWeaponBool = true;
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(weaponFire);
            }
            GameObject spawnedBullet = PhotonNetwork.Instantiate(bullet.name, bulletTransform.position, Quaternion.identity);
            if (this.transform.parent.gameObject.tag == "Enemy")
            {
                spawnedBullet.GetComponent<Bullet>().bulletModifier = (int)Random.Range(0, 3);
            }
            else if (this.transform.parent.gameObject.tag == "BossEnemy")
            {
                spawnedBullet.GetComponent<Bullet>().bulletModifier = (int)Random.Range(2, 5);
            }
            shootForce = (int)Random.Range(40, 75);
            spawnedBullet.GetComponent<Rigidbody>().velocity = bulletTransform.right * shootForce;
            ammoLeft--;
        }

        else if(ammoLeft >= 0)
        {
            fireWeaponBool = false;
            StartCoroutine(ReloadWeapon());
        }
    }

    IEnumerator ReloadWeapon()
    {
        yield return new WaitForSeconds(0);
        if (!audioSource2.isPlaying)
        {
            audioSource2.PlayOneShot(weaponReload);
        }
        yield return new WaitForSeconds(2);
        ammoLeft = maxAmmo;
        fireWeaponBool = true;
    }

}
