using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemyXPDrop : MonoBehaviour
{
    public SaveData saveData;
    public SpawnManager1 spawnManager;
    // Start is called before the first frame update
    void Start()
    {
        saveData = GameObject.FindGameObjectWithTag("SaveData").GetComponent<SaveData>();
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
                float xpDrop = 30f;

                //cal it at random probability
                if (Random.Range(0, 100f) < xpDrop)
                {
                    saveData.UpdateSkills(10);
                }
                saveData.UpdateSkills(5);
                PhotonNetwork.Destroy(gameObject);
            }
        }

        else if (this.tag == "Health")
        {
            if (other.CompareTag("Player"))
            {
                spawnManager.healthCount -= 1;
                other.GetComponent<PlayerHealth>().TakeDamage(-10);
                PhotonNetwork.Destroy(gameObject);
            }
        }

        else if (other.CompareTag("Player"))
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}

