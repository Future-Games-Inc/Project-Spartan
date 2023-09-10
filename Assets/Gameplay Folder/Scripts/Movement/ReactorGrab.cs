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

    // Start is called before the first frame update
    void Start()
    {
        reactorCore.GetComponent<Renderer>().material = normalMaterial;
        InvokeRepeating("ExtractionChirp", 0f, 5f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ReactorInteractor"))
        {
            playerHealth = other.GetComponentInParent<PlayerHealth>();
            playerHealth.reactorHeld = true;


            if (playerHealth.reactorExtraction < 50)
            {
                reactorCore.GetComponent<Renderer>().material = mediumMaterial;
            }

            if (playerHealth.reactorExtraction >= 50)
            {
                reactorCore.GetComponent<Renderer>().material = criticalMaterial;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ReactorInteractor"))
        {
            playerHealth.reactorHeld = false;
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