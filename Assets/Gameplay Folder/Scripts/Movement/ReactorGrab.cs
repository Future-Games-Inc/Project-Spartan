using Photon.Pun.Demo.Cockpit;
using UnityEngine;

public class ReactorGrab : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public GameObject holder;
    public Material normalMaterial;
    public Material mediumMaterial;
    public Material criticalMaterial;
    public GameObject reactorCore;

    public AudioSource audioSource;
    public AudioClip extractionClip;

    public AudioClip armorClip;
    public AudioClip healthClip;
    public AudioClip ammoClip;

    public GameObject[] buttons;
    public GameObject offLine;

    public bool held;

    // Start is called before the first frame update
    void OnEnable()
    {
        reactorCore.GetComponent<Renderer>().material = normalMaterial;
        InvokeRepeating("ExtractionChirp", 0f, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (held)
        {
            if (playerHealth.reactorExtraction >= 50)
            {
                reactorCore.GetComponent<Renderer>().material = criticalMaterial;
            }
            else
                reactorCore.GetComponent<Renderer>().material = mediumMaterial;
        }

        if(playerHealth.reactorExtraction >= 20)
        {
            foreach(GameObject button in buttons) 
            {
                button.SetActive(true);
            }
            offLine.SetActive(true);
        }
        else
        {
            foreach (GameObject button in buttons)
            {
                button.SetActive(false);
            }
            offLine.SetActive(false);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ReactorInteractor"))
        {
            playerHealth = other.GetComponentInParent<PlayerHealth>();
            playerHealth.reactorHeld = true;
            held = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ReactorInteractor"))
        {
            playerHealth.reactorHeld = false;
            held = false;
            reactorCore.GetComponent<Renderer>().material = normalMaterial;
        }
    }

    public void ExtractionChirp()
    {
        if (playerHealth != null && playerHealth.reactorHeld == true && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(extractionClip);
        }
    }

    public void UnfreezeTransforms()
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.None;
    }


    public void FreezeConstraints()
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void ArmorRecharge()
    {
        if (playerHealth.reactorExtraction >= 20)
        {
            playerHealth.maxArmor += 100;
            playerHealth.AddArmor(200);
            playerHealth.reactorExtraction -= 20;
            audioSource.PlayOneShot(armorClip);
        }
    }

    public void HealthRecharge()
    {
        if (playerHealth.reactorExtraction >= 20)
        {
            playerHealth.maxHealth += 100;
            playerHealth.AddHealth(200);
            playerHealth.reactorExtraction -= 20;
            audioSource.PlayOneShot(healthClip);
        }
    }

    public void AmmoRecharge()
    {
        if (playerHealth.reactorExtraction >= 20)
        {
            playerHealth.maxAmmo += 50;
            playerHealth.bulletModifier += 5;
            playerHealth.reactorExtraction -= 20;
            audioSource.PlayOneShot(ammoClip);
        }
    }
}