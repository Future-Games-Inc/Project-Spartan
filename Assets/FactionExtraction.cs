using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class FactionExtraction : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public string factionExtraction;

    public bool inUse = false;
    public bool singleExtraction = false;

    public AudioSource audioSource;
    public AudioClip bankExtracted;
    // Start is called before the first frame update
    void Start()
    {

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
                }

                else if (playerHealth.MuerteDeDatacard == true && singleExtraction == false)
                {
                    inUse = true;
                    playerHealth.factionExtraction = true;
                    factionExtraction = "Muerte De Dios";
                    singleExtraction = true;
                }

                else if (playerHealth.ChaosDatacard == true && singleExtraction == false)
                {
                    inUse = true;
                    playerHealth.factionExtraction = true;
                    factionExtraction = "Chaos Cartel";
                    singleExtraction = true;
                }

                else if (playerHealth.CintSixDatacard == true && singleExtraction == false)
                {
                    inUse = true;
                    playerHealth.factionExtraction = true;
                    factionExtraction = "CintSix Cartel";
                    singleExtraction = true;
                }

                else if (playerHealth.FedZoneDatacard == true && singleExtraction == false)
                {
                    inUse = true;
                    playerHealth.factionExtraction = true;
                    factionExtraction = "Federation Zone Authority";
                    singleExtraction = true;
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
}