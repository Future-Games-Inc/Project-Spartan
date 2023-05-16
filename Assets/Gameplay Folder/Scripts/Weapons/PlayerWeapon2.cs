using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerWeapon2 : MonoBehaviour
{
    public Transform[] spawnPoint;
    public float fireSpeed = 20;
    public AudioSource audioSource;
    public AudioClip shootSFX;
    public AudioClip reloadSFX;

    public int maxAmmo;
    public int ammoLeft;
    public int durability;

    public GameObject stickyBullet;
    public GameObject gravityBullet;
    public GameObject blackoutBullet;

    public bool active = true;
    public bool isFiring = false;
    public bool hasTouched = false;

    public bool sticky = true;
    public bool gravity;
    public bool blackout;

    // Start is called before the first frame update
    void Start()
    {
        maxAmmo = 50;
        durability = 3;
        ammoLeft = maxAmmo;
        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
        grabbable.activated.AddListener(StartFireBullet);
        grabbable.deactivated.AddListener(StopFireBullet);
        StartCoroutine(PickedUp());
        StartCoroutine(DestroyWeapon())
;
    }

    // Update is called once per frame
    void Update()
    {
        if (durability == 0)
        {
            Destroy(gameObject);
        }
    }

    public void StartFireBullet(ActivateEventArgs arg)
    {
        isFiring = true;
        StartCoroutine(FireBullet());
    }

    public void StopFireBullet(DeactivateEventArgs arg)
    {
        isFiring = false;
        StopCoroutine(FireBullet());
    }

    public void StickyActive()
    {
        sticky = true;
        gravity = false;
        blackout = false;
    }

    public void GravityActive()
    {
        sticky = false;
        gravity = true;
        blackout = false;
    }

    public void BlackOutActive()
    {
        sticky = false;
        gravity = false;
        blackout = true;
    }

    IEnumerator FireBullet()
    {
        while (isFiring)
        {
            if (ammoLeft >= 1 && active == true)
            {
                if (!audioSource.isPlaying)
                    audioSource.PlayOneShot(shootSFX);
                foreach (Transform t in spawnPoint)
                {
                    if (sticky)
                    {
                        GameObject spawnedBullet = Instantiate(stickyBullet, t.position, Quaternion.identity);
                        spawnedBullet.GetComponent<Rigidbody>().velocity = t.forward * fireSpeed;
                        ammoLeft--;
                    }
                    if (gravity)
                    {
                        GameObject spawnedBullet = Instantiate(gravityBullet, t.position, Quaternion.identity);
                        spawnedBullet.GetComponent<Rigidbody>().velocity = t.forward * fireSpeed;
                        ammoLeft--;
                    }
                    if (blackout)
                    {
                        GameObject spawnedBullet = Instantiate(blackoutBullet, t.position, Quaternion.identity);
                        spawnedBullet.GetComponent<Rigidbody>().velocity = t.forward * fireSpeed;
                        ammoLeft--;
                    }
                }
            }

            if (ammoLeft <= 0 && active == true)
            {
                active = false;
                StartCoroutine(Reload());
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator Reload()
    {
        yield return new WaitForSeconds(0);
        StopCoroutine(FireBullet());
        audioSource.PlayOneShot(reloadSFX);
        yield return new WaitForSeconds(3);
        ammoLeft = maxAmmo;
        durability--;
        active = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LeftHand") || other.CompareTag("RightHand"))
        {
            hasTouched = true;
        }
    }

    IEnumerator PickedUp()
    {
        yield return new WaitForSeconds(20);
        if (!hasTouched)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator DestroyWeapon()
    {
        yield return new WaitForSeconds(120);
        Destroy(gameObject);
    }
}
