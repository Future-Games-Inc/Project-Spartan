using Photon.Pun;
using Photon.Realtime;
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

    public AudioSource audioSource;
    public AudioClip extractionClip;

    public static readonly byte ReactorExtractionTrue = 1;
    public static readonly byte ReactorExtractionFalse = 2;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Renderer>().material = normalMaterial;
        InvokeRepeating("ExtractionChirp", 0f, 5f);
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

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            ExitGames.Client.Photon.SendOptions sendOptions = new ExitGames.Client.Photon.SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(ReactorExtractionTrue, null, raiseEventOptions, sendOptions);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ReactorInteractor"))
        {
            playerHealth = other.GetComponentInParent<PlayerHealth>();
            playerHealth.reactorHeld = false;
            this.GetComponent<Renderer>().material = normalMaterial;

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            ExitGames.Client.Photon.SendOptions sendOptions = new ExitGames.Client.Photon.SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(ReactorExtractionFalse, null, raiseEventOptions, sendOptions);
        }
    }

    public void ExtractionChirp()
    {
        if(playerHealth != null && playerHealth.reactorHeld == true && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(extractionClip);
        }
    }
}
