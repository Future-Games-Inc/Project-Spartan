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
}