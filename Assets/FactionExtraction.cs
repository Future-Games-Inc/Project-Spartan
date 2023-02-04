using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using UnityEngine;

public class FactionExtraction : MonoBehaviourPunCallbacks
{
    public PlayerHealth playerHealth;
    public string factionExtraction;

    public bool inUse = false;
    public bool singleExtraction = false;

    public AudioSource audioSource;
    public AudioClip bankExtracted;
    public AudioClip uploadChirp;

    public static readonly byte FactionExtractionTrue = 10;
    public static readonly byte FactionExtractionFalse = 11;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Uploading", 0f, 5f);
    }

    void Update()
    {
        if (playerHealth != null)
        {
            photonView.RPC("RPC_FactionExtracted", RpcTarget.AllBuffered);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (inUse == false && singleExtraction == false)
            {
                playerHealth = other.GetComponent<PlayerHealth>();
                photonView.RPC("RPC_TriggerEnter", RpcTarget.AllBuffered);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerHealth = other.GetComponent<PlayerHealth>();
            photonView.RPC("RPC_TriggerExit", RpcTarget.AllBuffered);
        }
    }

    [System.Obsolete]
    public IEnumerator FactionExtracted()
    {
        yield return new WaitForSeconds(0);
        photonView.RPC("RPC_FactionExtraction", RpcTarget.AllBuffered);
        yield return new WaitForSeconds(2);
        photonView.RPC("RPC_Use", RpcTarget.AllBuffered);
    }
    public void Uploading()
    {
        photonView.RPC("RPC_Uploading", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void RPC_TriggerEnter()
    {
        if (playerHealth.CyberGangDatacard == true && singleExtraction == false)
        {
            inUse = true;
            playerHealth.factionExtraction = true;
            factionExtraction = "Cyber SK Gang";
            singleExtraction = true;

            photonView.RPC("RaiseEvent1", RpcTarget.All, FactionExtraction.FactionExtractionTrue, null);
        }

        else if (playerHealth.MuerteDeDatacard == true && singleExtraction == false)
        {
            inUse = true;
            playerHealth.factionExtraction = true;
            factionExtraction = "Muerte De Dios";
            singleExtraction = true;

            photonView.RPC("RaiseEvent1", RpcTarget.All, FactionExtraction.FactionExtractionTrue, null);
        }

        else if (playerHealth.ChaosDatacard == true && singleExtraction == false)
        {
            inUse = true;
            playerHealth.factionExtraction = true;
            factionExtraction = "Chaos Cartel";
            singleExtraction = true;

            photonView.RPC("RaiseEvent1", RpcTarget.All, FactionExtraction.FactionExtractionTrue, null);
        }

        else if (playerHealth.CintSixDatacard == true && singleExtraction == false)
        {
            inUse = true;
            playerHealth.factionExtraction = true;
            factionExtraction = "CintSix Cartel";
            singleExtraction = true;

            photonView.RPC("RaiseEvent1", RpcTarget.All, FactionExtraction.FactionExtractionTrue, null);
        }

        else if (playerHealth.FedZoneDatacard == true && singleExtraction == false)
        {
            inUse = true;
            playerHealth.factionExtraction = true;
            factionExtraction = "Federation Zone Authority";
            singleExtraction = true;

            photonView.RPC("RaiseEvent1", RpcTarget.All, FactionExtraction.FactionExtractionTrue, null);
        }
    }

    [PunRPC]
    void RPC_TriggerExit()
    {
        playerHealth.factionExtraction = false;
        inUse = false;

        photonView.RPC("RaiseEvent2", RpcTarget.All, FactionExtraction.FactionExtractionFalse, null);
    }

    [PunRPC]
    [Obsolete]
    void RPC_FactionExtraction()
    {
        audioSource.PlayOneShot(bankExtracted);
        playerHealth.SubmitScoreRoutine(factionExtraction, -250);
        playerHealth.SubmitScoreRoutine(playerHealth.characterFaction, 250);

        if (playerHealth.CyberGangDatacard == true && singleExtraction == true)
        {
            playerHealth.CyberGangDatacard = false;
            singleExtraction = false;
        }

        else if (playerHealth.MuerteDeDatacard == true && singleExtraction == true)
        {
            playerHealth.MuerteDeDatacard = false;
            singleExtraction = false;
        }

        else if (playerHealth.ChaosDatacard == true && singleExtraction == true)
        {
            playerHealth.ChaosDatacard = false;
            singleExtraction = false;
        }

        else if (playerHealth.CintSixDatacard == true && singleExtraction == true)
        {
            playerHealth.CintSixDatacard = false;
            singleExtraction = false;
        }

        else if (playerHealth.FedZoneDatacard == true && singleExtraction == true)
        {
            playerHealth.FedZoneDatacard = false;
            singleExtraction = false;
        }
        playerHealth.GetXP(20);
    }

    [PunRPC]
    void RPC_Uploading()
    {
        if (inUse == true && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(uploadChirp);
        }
    }

    [PunRPC]
    void RPC_Use()
    {
        inUse = false;
    }

    [PunRPC]
    [Obsolete]
    void RPC_FactionExtracted()
    {
        if (playerHealth.factionExtractionCount >= 100 && playerHealth.factionExtraction == true)
        {
            StartCoroutine(FactionExtracted());
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