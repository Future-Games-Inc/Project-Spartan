using System.Collections;
using UnityEngine;
using Photon.Pun;

public class SpawnManager : MonoBehaviour
{
    public Vector3 spawnPosition;
    public GameObject playerPrefab;
    public bool gameOver;
    public GameObject winnerPlayer;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(SpawnPlayer());
            }
        }
        gameOver = false;
        winnerPlayer = null;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }

    IEnumerator SpawnPlayer()
    {
        yield return new WaitForSeconds(10);
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.InstantiateRoomObject(playerPrefab.name, spawnPosition, Quaternion.identity);
        }
    }
}
