using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemyXPDrop : MonoBehaviour
{
    public SpawnManager1 spawnManager;
    // Start is called before the first frame update
    void Start()
    {
        spawnManager = GameObject.FindGameObjectWithTag("spawnManager").GetComponent<SpawnManager1>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (this.tag == "XP")
        {
            if (other.CompareTag("Player"))
            {
                float xpDrop = 10f;

                //cal it at random probability
                if (Random.Range(0, 100f) < xpDrop)
                {
                    other.gameObject.GetComponent<PlayerHealth>().UpdateSkills(10);
                }
                else
                {
                    other.gameObject.GetComponent<PlayerHealth>().UpdateSkills(5);
                }
                PhotonNetwork.Destroy(gameObject);
            }           
        }

        else if (this.tag == "Health")
        {
            if (other.CompareTag("Player"))
            {
                spawnManager.healthCount -= 1;
                other.GetComponent<PlayerHealth>().AddHealth(10);
                PhotonNetwork.Destroy(gameObject);
            }           
        }

        else if (this.tag == "MinorHealth")
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<PlayerHealth>().AddHealth(5);
                PhotonNetwork.Destroy(gameObject);
            }
        }

        else if(this.tag == "ExtraXP")
        {
            if (other.CompareTag("Player"))
            {
                float xpDrop = 10f;

                //cal it at random probability
                if (Random.Range(0, 100f) < xpDrop)
                {
                    other.gameObject.GetComponent<PlayerHealth>().UpdateSkills(100);
                }
                else
                {
                    other.gameObject.GetComponent<PlayerHealth>().UpdateSkills(50);
                }
                PhotonNetwork.Destroy(gameObject);
            }
        }

        else if (this.tag == "toxicDropNormal")
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<PlayerHealth>().toxicEffectActive = true;
                other.GetComponent<PlayerHealth>().Toxicity(10);
                PhotonNetwork.Destroy(gameObject);
            }
        }

        else if (this.tag == "toxicDropExtra")
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<PlayerHealth>().toxicEffectActive = true;
                other.GetComponent<PlayerHealth>().Toxicity(20);
                PhotonNetwork.Destroy(gameObject);
            }
        }

        else if (this.tag == "bulletModifierNormal")
        {
            if (other.CompareTag("Player"))
            {                
                other.GetComponent<PlayerHealth>().BulletImprove(10,2);
                other.GetComponent<PlayerHealth>().bulletImproved = true;
                PhotonNetwork.Destroy(gameObject);
            }
        }

        else if (this.tag == "bulletModifierExtra")
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<PlayerHealth>().bulletImproved = true;
                other.GetComponent<PlayerHealth>().BulletImprove(15, 4);
                PhotonNetwork.Destroy(gameObject);
            }
        }

        else if (this.tag == "MPShield")
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<PlayerHealth>().shieldActive = true;
                other.GetComponent<PlayerHealth>().Shield(10);
                PhotonNetwork.Destroy(gameObject);
            }
        }

        else
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}

