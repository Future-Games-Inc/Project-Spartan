using UnityEngine;

public class PickupSounds : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clip;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Pickups"))
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
