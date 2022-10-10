using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnManager : MonoBehaviour
{
    public Vector3 spawnPosition;

    // Start is called before the first frame update
    void Start()
    {
        if(PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.Instantiate("Player", spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.Log("Network not ready");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
