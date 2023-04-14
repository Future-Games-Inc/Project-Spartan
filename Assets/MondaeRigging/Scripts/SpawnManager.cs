using System.Collections;
using UnityEngine;
using Photon.Pun;

public class SpawnManager : MonoBehaviour
{
    public Vector3 spawnPosition;
    public GameObject[] playerPrefab;
    public bool gameOver;
    public GameObject winnerPlayer;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            StartCoroutine(SpawnPlayer());
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
        yield return new WaitForSeconds(3);
        object avatarSelectionNumber;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AVATAR_SELECTION_NUMBER, out avatarSelectionNumber))
        {
            int selectionValue = (int)avatarSelectionNumber;
            PhotonNetwork.Instantiate(playerPrefab[selectionValue].name, spawnPosition, Quaternion.identity);
        }
    }
}
