using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerWeapon : MonoBehaviour
{
    public GameObject player;
    public Transform[] spawnPoint;
    public float fireSpeed = 20;
    public GameObject bullet;

    public int maxAmmo;
    public int ammoLeft;

    public GameObject reloadingScreen;

    public AudioSource audioSource;
    public AudioClip weaponFire;
    public AudioClip weaponReload;

    // Start is called before the first frame update
    void Start()
    {
        reloadingScreen.SetActive(false);
        maxAmmo = 25;
        ammoLeft = maxAmmo;
        XRGrabNetworkInteractable grabbable = GetComponent<XRGrabNetworkInteractable>();
        grabbable.activated.AddListener(FireBullet);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void FireBullet(ActivateEventArgs arg)
    {
        if (ammoLeft >= 1)
        {
            foreach (Transform t in spawnPoint)
            {
                audioSource.PlayOneShot(weaponFire);
                GameObject spawnedBullet = PhotonNetwork.Instantiate(bullet.name, t.position, Quaternion.identity);
                spawnedBullet.transform.parent = player.transform;
                spawnedBullet.GetComponent<Rigidbody>().velocity = t.right * fireSpeed;
                ammoLeft -= 1;
            }
        }

        if (ammoLeft <= 0)
        {
            StartCoroutine(ReloadWeapon());
        }
    }

    IEnumerator ReloadWeapon()
    {
        yield return new WaitForSeconds(0);
        reloadingScreen.SetActive(true);
        audioSource.PlayOneShot(weaponReload);
        yield return new WaitForSeconds(2);
        ammoLeft = maxAmmo;
        reloadingScreen.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LeftHand") || other.CompareTag("RightHand"))
        {
            player = other.gameObject;
        }
    }


}
