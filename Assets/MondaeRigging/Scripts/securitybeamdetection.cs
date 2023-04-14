using UnityEngine;

public class securitybeamdetection : MonoBehaviour
{
    public SecurityBeam securityBeam;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            securityBeam.detectedPlayer = other.gameObject;
            StartCoroutine(securityBeam.FoundPlayer());
        }
    }
}
