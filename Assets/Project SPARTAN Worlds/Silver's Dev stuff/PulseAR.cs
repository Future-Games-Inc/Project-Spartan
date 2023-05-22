using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
public class PulseAR : MonoBehaviour
{
    public Transform[] spawnPoint;
    [Header("Audio ---------------------------------------")]
    public AudioSource audioSource;
    public AudioClip reloadSFX;
    public AudioClip EmptySFX;
    private bool playingEmpty = false;
    [Header("Gun property --------------------------------")]
    public int maxAmmo;
    [HideInInspector]
    public int ammoLeft;
    public int durability;
    public float ReloadDuration;
    [Header("Bullet Types --------------------------------")]
    public GameObject playerBullet;
    public GameObject PulseBullet;
    [HideInInspector]
    public bool active = true;
    [HideInInspector]
    public bool isFiring = false;
    [HideInInspector]
    public bool hasTouched = false;
    [Header("Keybinds ------------------------------------")]
    public InputActionProperty triggerL;
    public InputActionProperty triggerR;
    [HideInInspector]
    public bool isDual = false;
    [HideInInspector]
    public bool isTriggerSingle = false;
    // Start is called before the first frame update
    void Start()
    {
        maxAmmo = 25;
        ammoLeft = maxAmmo;
        //StartCoroutine(PickedUp());
        //StartCoroutine(DestroyWeapon())
    }

    // Update is called once per frame
    void Update()
    {
        checkForInputs();
        // check for durablity
        if (durability <= 0) StartCoroutine(DestroyWeapon());
    }

    private void checkForInputs()
    {
        // ignore if the gun is not being held
        if (!isTriggerSingle) return;
        // hold down trigger R for fire (works on left hand as well)
        if ((triggerR.action.ReadValue<float>() > 0.8f || triggerL.action.ReadValue<float>() > 0.8f))
        {
            // if both trigger held down then do pulse fire
            if (triggerL.action.ReadValue<float>() > 0.8f && isDual)
            {
                // pulse fire
                if (!isFiring) StartCoroutine(FireBullet(true));
            }
            else
            {
                // normal fire
                if (!isFiring) StartCoroutine(FireBullet());
            }
        }
    }

    public void setDual(bool state)
    {
        isDual = state;
    }

    public void setHeld(bool state)
    {
        isTriggerSingle = state;
    }

    IEnumerator FireBullet(bool pulse = false)
    {
        isFiring = true;
        if (pulse)
        {
            // do the pulse fire thing
            if (ammoLeft > 0 && active == true)
            {
                GameObject spawnedBullet = Instantiate(PulseBullet, spawnPoint[0].position, Quaternion.identity);
                audioSource.PlayOneShot(spawnedBullet.GetComponent<BulletBehavior>().fireSound);
                spawnedBullet.GetComponent<Rigidbody>().velocity = spawnPoint[0].forward * spawnedBullet.GetComponent<BulletBehavior>().TravelSpeed;
                ammoLeft -= 3;
            }
            yield return new WaitForSeconds(PulseBullet.GetComponent<BulletBehavior>().RateOfFire);
        } 
        // normal fire mode
        else {
            if (ammoLeft > 0 && active == true)
            {
                GameObject spawnedBullet = Instantiate(playerBullet, spawnPoint[0].position, Quaternion.identity);
                audioSource.PlayOneShot(spawnedBullet.GetComponent<BulletBehavior>().fireSound);
                spawnedBullet.GetComponent<Rigidbody>().velocity = spawnPoint[0].forward * spawnedBullet.GetComponent<BulletBehavior>().TravelSpeed;
                ammoLeft--;
            }
            yield return new WaitForSeconds(playerBullet.GetComponent<BulletBehavior>().RateOfFire);
        }

        if (ammoLeft <= 0 && active == true)
        {
            if (!playingEmpty) StartCoroutine(playEmptyAudio());

            StartCoroutine(Reload());


            IEnumerator playEmptyAudio()
            {
                playingEmpty = true;
                audioSource.PlayOneShot(reloadSFX);
                yield return new WaitForSeconds(2f);
                playingEmpty = false;
            }
        }

        isFiring = false;
    }

    IEnumerator Reload()
    {
        active = false;
        audioSource.PlayOneShot(reloadSFX);
        yield return new WaitForSeconds(ReloadDuration);
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
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }
}
