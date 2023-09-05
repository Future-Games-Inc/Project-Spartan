using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class ReactorGrab : MonoBehaviourPunCallbacks
{
    public PlayerHealth playerHealth;
    public GameObject holder;
    public Material normalMaterial;
    public Material mediumMaterial;
    public Material criticalMaterial;
    public GameObject reactorCore;

    private int currentInteractingPlayer = -1; // -1 means no one is interacting



    public AudioSource audioSource;
    public AudioClip extractionClip;

    public static readonly byte ReactorExtractionTrue = 20;
    public static readonly byte ReactorExtractionFalse = 21;

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
            PlayerHealth ph = other.GetComponentInParent<PlayerHealth>();
            int playerID = ph.GetComponent<PhotonView>().ViewID;

            // If no one is interacting or a different player wants to interact
            if (currentInteractingPlayer == -1 || currentInteractingPlayer != playerID)
            {
                currentInteractingPlayer = playerID; // Set the current interacting player
                playerHealth = ph;
                photonView.RPC("RPC_TriggerEnter", RpcTarget.AllBuffered);
                photonView.RPC("RaiseEvent1", RpcTarget.All, ReactorGrab.ReactorExtractionTrue, null);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ReactorInteractor"))
        {
            PlayerHealth ph = other.GetComponentInParent<PlayerHealth>();
            int playerID = ph.GetComponent<PhotonView>().ViewID;

            // If the exiting player is the one who is currently interacting
            if (currentInteractingPlayer == playerID)
            {
                currentInteractingPlayer = -1; // Reset to -1, allowing new interactions
                playerHealth = ph;
                photonView.RPC("RPC_TriggerExit", RpcTarget.AllBuffered);
            }
        }
    }

    public void ExtractionChirp()
    {
        photonView.RPC("RPC_Chirp", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void RPC_Chirp()
    {
        if (playerHealth != null && playerHealth.reactorHeld == true && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(extractionClip);
        }
    }

    [PunRPC]
    void RPC_TriggerExit()
    {
        playerHealth.reactorHeld = false;
        reactorCore.GetComponent<Renderer>().material = normalMaterial;
        photonView.RPC("RaiseEvent2", RpcTarget.All, ReactorGrab.ReactorExtractionFalse, null);
    }

    [PunRPC]
    void RPC_TriggerEnter()
    {
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


    [PunRPC]
    void RaiseEvent1(byte eventCode, object content, PhotonMessageInfo info)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        ExitGames.Client.Photon.SendOptions sendOptions = new ExitGames.Client.Photon.SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(eventCode, content, raiseEventOptions, sendOptions);
    }

    [PunRPC]
    void RaiseEvent2(byte eventCode, object content, PhotonMessageInfo info)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        ExitGames.Client.Photon.SendOptions sendOptions = new ExitGames.Client.Photon.SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(eventCode, content, raiseEventOptions, sendOptions);
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