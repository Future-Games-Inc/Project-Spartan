using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class FactionExtraction : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public string factionExtraction;

    public bool inUse = false;
    public bool singleExtraction = false;

    public AudioSource audioSource;
    public AudioClip bankExtracted;
    public AudioClip uploadChirp;

    public static readonly byte FactionExtractionTrue = 1;
    public static readonly byte FactionExtractionFalse = 2;

    PhotonView pV;
    // Start is called before the first frame update
    void Start()
    {
        pV = GetComponent<PhotonView>();
        InvokeRepeating("Uploading", 0f, 5f);
    }

    // Update is called once per frame
    [Obsolete]
    void Update()
    {
        pV.RPC("RPC_FactionExtracted", RpcTarget.AllBuffered);
    }

    private void OnTriggerEnter(Collider other)
    {
        pV.RPC("RPC_TriggerEnter", RpcTarget.AllBuffered, other);
    }

    private void OnTriggerExit(Collider other)
    {
        pV.RPC("RPC_TriggerExit", RpcTarget.AllBuffered, other);
    }

    [System.Obsolete]
    public IEnumerator FactionExtracted()
    {
        yield return new WaitForSeconds(0);
        pV.RPC("RPC_FactionExtraction", RpcTarget.AllBuffered);
        yield return new WaitForSeconds(2);
        pV.RPC("RPC_Use", RpcTarget.AllBuffered);
    }
    public void Uploading()
    {
        pV.RPC("RPC_Uploading", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void RPC_TriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (inUse == false && singleExtraction == false)
            {
                playerHealth = other.GetComponent<PlayerHealth>();

                if (playerHealth.CyberGangDatacard == true && singleExtraction == false)
                {
                    inUse = true;
                    playerHealth.factionExtraction = true;
                    factionExtraction = "Cyber SK Gang";
                    singleExtraction = true;

                    if (PhotonNetwork.IsMasterClient)
                    {
                        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                        ExitGames.Client.Photon.SendOptions sendOptions = new ExitGames.Client.Photon.SendOptions { Reliability = true };
                        PhotonNetwork.RaiseEvent(FactionExtractionTrue, null, raiseEventOptions, sendOptions);
                    }
                }

                else if (playerHealth.MuerteDeDatacard == true && singleExtraction == false)
                {
                    inUse = true;
                    playerHealth.factionExtraction = true;
                    factionExtraction = "Muerte De Dios";
                    singleExtraction = true;

                    if (PhotonNetwork.IsMasterClient)
                    {
                        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                        ExitGames.Client.Photon.SendOptions sendOptions = new ExitGames.Client.Photon.SendOptions { Reliability = true };
                        PhotonNetwork.RaiseEvent(FactionExtractionTrue, null, raiseEventOptions, sendOptions);
                    }
                }

                else if (playerHealth.ChaosDatacard == true && singleExtraction == false)
                {
                    inUse = true;
                    playerHealth.factionExtraction = true;
                    factionExtraction = "Chaos Cartel";
                    singleExtraction = true;

                    if (PhotonNetwork.IsMasterClient)
                    {
                        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                        ExitGames.Client.Photon.SendOptions sendOptions = new ExitGames.Client.Photon.SendOptions { Reliability = true };
                        PhotonNetwork.RaiseEvent(FactionExtractionTrue, null, raiseEventOptions, sendOptions);
                    }
                }

                else if (playerHealth.CintSixDatacard == true && singleExtraction == false)
                {
                    inUse = true;
                    playerHealth.factionExtraction = true;
                    factionExtraction = "CintSix Cartel";
                    singleExtraction = true;

                    if (PhotonNetwork.IsMasterClient)
                    {
                        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                        ExitGames.Client.Photon.SendOptions sendOptions = new ExitGames.Client.Photon.SendOptions { Reliability = true };
                        PhotonNetwork.RaiseEvent(FactionExtractionTrue, null, raiseEventOptions, sendOptions);
                    }
                }

                else if (playerHealth.FedZoneDatacard == true && singleExtraction == false)
                {
                    inUse = true;
                    playerHealth.factionExtraction = true;
                    factionExtraction = "Federation Zone Authority";
                    singleExtraction = true;

                    if (PhotonNetwork.IsMasterClient)
                    {
                        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                        ExitGames.Client.Photon.SendOptions sendOptions = new ExitGames.Client.Photon.SendOptions { Reliability = true };
                        PhotonNetwork.RaiseEvent(FactionExtractionTrue, null, raiseEventOptions, sendOptions);
                    }
                }
            }
        }

    }

    [PunRPC]
    void RPC_TriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerHealth = other.GetComponent<PlayerHealth>();
            playerHealth.factionExtraction = false;
        }
        inUse = false;

        if (PhotonNetwork.IsMasterClient)
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            ExitGames.Client.Photon.SendOptions sendOptions = new ExitGames.Client.Photon.SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(FactionExtractionFalse, null, raiseEventOptions, sendOptions);
        }
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
        if (playerHealth != null)
        {
            if (playerHealth.factionExtractionCount >= 100 && playerHealth.factionExtraction == true)
            {
                StartCoroutine(FactionExtracted());
            }
        }
    }

}