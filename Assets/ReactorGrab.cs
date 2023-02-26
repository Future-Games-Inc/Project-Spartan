using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class ReactorGrab : MonoBehaviourPunCallbacks
{
    public PlayerHealth playerHealth;
    public Material normalMaterial;
    public Material mediumMaterial;
    public Material criticalMaterial;

    public AudioSource audioSource;
    public AudioClip extractionClip;

    public static readonly byte ReactorExtractionTrue = 20;
    public static readonly byte ReactorExtractionFalse = 21;

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
        if (other.CompareTag("ReactorInteractor"))
        {
            playerHealth = other.GetComponentInParent<PlayerHealth>();
            photonView.RPC("RPC_TriggerEnter", RpcTarget.AllBuffered);
            photonView.RPC("RaiseEvent1", RpcTarget.All, ReactorGrab.ReactorExtractionTrue, null);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ReactorInteractor"))
        {
            playerHealth = other.GetComponentInParent<PlayerHealth>();
            photonView.RPC("RPC_TriggerExit", RpcTarget.AllBuffered);
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
        this.GetComponent<Renderer>().material = normalMaterial;
        photonView.RPC("RaiseEvent2", RpcTarget.All, ReactorGrab.ReactorExtractionFalse, null);
    }

    [PunRPC]
    void RPC_TriggerEnter()
    {
        playerHealth.reactorHeld = true;

        if (playerHealth.reactorExtraction < 50)
        {
            this.GetComponent<Renderer>().material = mediumMaterial;
        }

        if (playerHealth.reactorExtraction >= 50)
        {
            this.GetComponent<Renderer>().material = criticalMaterial;
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
}