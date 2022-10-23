using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerWeapon1 : MonoBehaviour
{
    public Transform[] spawnPoint;
    public float fireSpeed = 150;
    public AudioSource audioSource;
    public AudioClip shootSFX;
    public AudioClip reloadSFX;
    public float ammoMax = 20f;
    public float ammoCurrent;

    public GameObject playerBullet;

    // Start is called before the first frame update
    void Start()
    {
        ammoCurrent = ammoMax;
        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
        grabbable.activated.AddListener(FireBullet);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FireBullet(ActivateEventArgs arg)
    {
        if (ammoCurrent >= 0)
        {
            audioSource.PlayOneShot(shootSFX);
            foreach (Transform t in spawnPoint)
            {
                GameObject spawnedBullet = Instantiate(playerBullet, t.position, Quaternion.identity);
                spawnedBullet.GetComponent<Rigidbody>().velocity = t.right * fireSpeed;
                ammoCurrent -= 1;
            }
        }
        else if(ammoCurrent < 0)
        {
            StartCoroutine(Reload());
        }
    }

    IEnumerator Reload()
    {
        yield return new WaitForSeconds(0);
        audioSource.PlayOneShot(reloadSFX);
        yield return new WaitForSeconds(3);
        ammoCurrent = ammoMax;
    }
    

}
