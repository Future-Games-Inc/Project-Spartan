using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;

public class SpawnManager : MonoBehaviourPunCallbacks
{
    public GameObject[] playerPrefab;
    public bool gameOver;
    public GameObject winnerPlayer;

    public float spawnRadius = 300.0f;

    public NavMeshSurface navMeshSurface;
    Vector3 spawnPosition;
    public Vector3 respawnPosition;

    void OnEnable()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            StartCoroutine(SpawnPlayer());
        }
        gameOver = false;
        winnerPlayer = null;
    }

    IEnumerator SpawnPlayer()
    {
        yield return new WaitForSeconds(.5f);
        Vector3 randomPosition = GenerateRandomPosition();

        if (randomPosition != Vector3.zero)
        {
            spawnPosition = randomPosition;
            respawnPosition = spawnPosition;

            object avatarSelectionNumber;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AVATAR_SELECTION_NUMBER, out avatarSelectionNumber))
            {
                int selectionValue = (int)avatarSelectionNumber;
                PhotonNetwork.Instantiate(playerPrefab[selectionValue].name, spawnPosition, Quaternion.identity);
            }
        }
    }

    Vector3 GenerateRandomPosition()
    {
        Vector3 randomPosition = Random.insideUnitSphere * spawnRadius;
        randomPosition += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPosition, out hit, spawnRadius, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return Vector3.zero;
    }
}
