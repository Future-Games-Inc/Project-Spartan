using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerWeapon1 : MonoBehaviour
{
    public Transform[] spawnPoint;
    public float fireSpeed = 20;
    public AudioSource audioSource;
    public AudioClip shootSFX;
    public AudioClip reloadSFX;

    public int maxAmmo;
    public int ammoLeft;
    public int durability;

    public GameObject playerBullet;

    // Start is called before the first frame update
    void Start()
    {
        maxAmmo = 20;
        durability = 3;
        ammoLeft = maxAmmo;
        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
        grabbable.activated.AddListener(FireBullet);
    }

    // Update is called once per frame
    void Update()
    {
        if(durability == 0)
        {
            Destroy(gameObject);
        }
    }

    public void FireBullet(ActivateEventArgs arg)
    {
        if (ammoLeft >= 0)
        {
            audioSource.PlayOneShot(shootSFX);
            foreach (Transform t in spawnPoint)
            {
                GameObject spawnedBullet = Instantiate(playerBullet, t.position, Quaternion.identity);
                spawnedBullet.GetComponent<Rigidbody>().velocity = transform.right * fireSpeed;
                ammoLeft --;
            }
        }
        else if(ammoLeft < 0)
        {
            StartCoroutine(Reload());
        }
    }

    IEnumerator Reload()
    {
        yield return new WaitForSeconds(0);
        audioSource.PlayOneShot(reloadSFX);
        yield return new WaitForSeconds(3);
        ammoLeft = maxAmmo;
        durability--;
    }
    

}
