using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnManager : MonoBehaviour
{
    public Vector3 spawnPosition;
    public GameObject playerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnPlayer());
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }

    IEnumerator SpawnPlayer()
    {
        yield return new WaitForSeconds(2);
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);
    }
}
