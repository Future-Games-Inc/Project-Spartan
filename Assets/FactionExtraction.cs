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
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Uploading", 0f, 5f);
    }

    // Update is called once per frame
    [Obsolete]
    void Update()
    {
        if (playerHealth != null)
        {
            if (playerHealth.factionExtractionCount >= 100 && playerHealth.factionExtraction == true)
            {
                StartCoroutine(FactionExtracted());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
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

                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                    ExitGames.Client.Photon.SendOptions sendOptions = new ExitGames.Client.Photon.SendOptions { Reliability = true };
                    PhotonNetwork.RaiseEvent(FactionExtractionTrue, null, raiseEventOptions, sendOptions);
                }

                else if (playerHealth.MuerteDeDatacard == true && singleExtraction == false)
                {
                    inUse = true;
                    playerHealth.factionExtraction = true;
                    factionExtraction = "Muerte De Dios";
                    singleExtraction = true;

                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                    ExitGames.Client.Photon.SendOptions sendOptions = new ExitGames.Client.Photon.SendOptions { Reliability = true };
                    PhotonNetwork.RaiseEvent(FactionExtractionTrue, null, raiseEventOptions, sendOptions);
                }

                else if (playerHealth.ChaosDatacard == true && singleExtraction == false)
                {
                    inUse = true;
                    playerHealth.factionExtraction = true;
                    factionExtraction = "Chaos Cartel";
                    singleExtraction = true;

                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                    ExitGames.Client.Photon.SendOptions sendOptions = new ExitGames.Client.Photon.SendOptions { Reliability = true };
                    PhotonNetwork.RaiseEvent(FactionExtractionTrue, null, raiseEventOptions, sendOptions);
                }

                else if (playerHealth.CintSixDatacard == true && singleExtraction == false)
                {
                    inUse = true;
                    playerHealth.factionExtraction = true;
                    factionExtraction = "CintSix Cartel";
                    singleExtraction = true;

                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                    ExitGames.Client.Photon.SendOptions sendOptions = new ExitGames.Client.Photon.SendOptions { Reliability = true };
                    PhotonNetwork.RaiseEvent(FactionExtractionTrue, null, raiseEventOptions, sendOptions);
                }

                else if (playerHealth.FedZoneDatacard == true && singleExtraction == false)
                {
                    inUse = true;
                    playerHealth.factionExtraction = true;
                    factionExtraction = "Federation Zone Authority";
                    singleExtraction = true;

                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                    ExitGames.Client.Photon.SendOptions sendOptions = new ExitGames.Client.Photon.SendOptions { Reliability = true };
                    PhotonNetwork.RaiseEvent(FactionExtractionTrue, null, raiseEventOptions, sendOptions);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerHealth = other.GetComponent<PlayerHealth>();
            playerHealth.factionExtraction = false;
        }
        inUse = false;

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        ExitGames.Client.Photon.SendOptions sendOptions = new ExitGames.Client.Photon.SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(FactionExtractionFalse, null, raiseEventOptions, sendOptions);
    }

    [System.Obsolete]
    public IEnumerator FactionExtracted()
    {
        yield return new WaitForSeconds(0);
        audioSource.PlayOneShot(bankExtracted);
        yield return playerHealth.SubmitScoreRoutine(factionExtraction, -250);
        yield return playerHealth.SubmitScoreRoutine(playerHealth.characterFaction, 250);

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

        yield return new WaitForSeconds(2);
        inUse = false;
    }
     public void Uploading()
    {
        if(inUse == true && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(uploadChirp);
        }
    }

}