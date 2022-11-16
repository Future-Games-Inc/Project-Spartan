using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ReactorGrab : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public Material normalMaterial;
    public Material mediumMaterial;
    public Material criticalMaterial;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Renderer>().material = normalMaterial;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("ReactorInteractor"))
        {
            playerHealth = other.GetComponentInParent<PlayerHealth>();
            playerHealth.reactorHeld = true;

            if(playerHealth.reactorExtraction < 50)
            {
                this.GetComponent<Renderer>().material = mediumMaterial;
            }

            if (playerHealth.reactorExtraction >= 50)
            {
                this.GetComponent<Renderer>().material = criticalMaterial;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ReactorInteractor"))
        {
            playerHealth = other.GetComponentInParent<PlayerHealth>();
            playerHealth.reactorHeld = false;
            this.GetComponent<Renderer>().material = normalMaterial;
        }
    }
}
