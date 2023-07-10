using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] playerPrefab;
    public bool gameOver;
    public GameObject winnerPlayer;

    public float spawnRadius = 300.0f;   // Maximum spawn radius

    public NavMeshSurface navMeshSurface;
    Vector3 spawnPosition;
    public Vector3 respawnPosition;

    // Start is called before the first frame update
    void OnEnable()
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
        // Create an array to store the valid positions
        Vector3[] spawnPositions = new Vector3[10];
        int validPositionsCount = 0;

        // Generate multiple random positions within the spawn radius
        for (int i = 0; i < spawnPositions.Length; i++)
        {
            Vector3 randomPosition = Random.insideUnitSphere * spawnRadius;
            randomPosition += transform.position;

            // Find the nearest point on the NavMesh to the random position
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPosition, out hit, spawnRadius, NavMesh.AllAreas))
            {
                // Add the valid position to the array
                spawnPositions[validPositionsCount] = hit.position;
                validPositionsCount++;
            }
        }

        // If there are valid positions, choose one randomly for spawning the enemy
        if (validPositionsCount > 0)
        {
            spawnPosition = spawnPositions[Random.Range(0, validPositionsCount)];
            respawnPosition = spawnPosition;

            object avatarSelectionNumber;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AVATAR_SELECTION_NUMBER, out avatarSelectionNumber))
            {
                int selectionValue = (int)avatarSelectionNumber;
                PhotonNetwork.Instantiate(playerPrefab[selectionValue].name, spawnPosition, Quaternion.identity);
            }
        }
    }
}
