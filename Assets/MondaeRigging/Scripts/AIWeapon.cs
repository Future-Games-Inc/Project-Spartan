using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

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
        fireWeaponBool = false;
        shootForce = Random.Range(40, 75);
        maxAmmo = 25;
        ammoLeft = maxAmmo;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Fire()
    {
        fireWeaponBool = true;
        StartCoroutine(FireWeapon());

    }

    IEnumerator FireWeapon()
    {
        while (fireWeaponBool == true)
        {
            yield return new WaitForSeconds(0);
            if (ammoLeft >= 1)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.PlayOneShot(weaponFire);
                }
                Rigidbody rb = PhotonNetwork.Instantiate(bullet.name, bulletTransform.position, Quaternion.identity).GetComponent<Rigidbody>();
                rb.velocity = transform.right * shootForce;
                ammoLeft--;
            }

            if (ammoLeft <= 0)
            {
                if (!audioSource2.isPlaying)
                {
                    audioSource2.PlayOneShot(weaponReload);
                }
                new WaitForSeconds(3);
                ammoLeft = maxAmmo;
            }
            yield return new WaitForSeconds(2);
        }

    }
}
