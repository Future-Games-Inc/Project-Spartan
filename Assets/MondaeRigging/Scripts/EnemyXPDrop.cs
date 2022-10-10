using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemyXPDrop : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SaveData playerManager = other.GetComponent<SaveData>();

            float xpDrop = 30f;

            //cal it at random probability
            if (Random.Range(0, 100f) < xpDrop)
            {
                playerManager.UpdateSkills(10);
            }
            playerManager.UpdateSkills(5);
        }
        PhotonNetwork.Destroy(gameObject);
    }
}

