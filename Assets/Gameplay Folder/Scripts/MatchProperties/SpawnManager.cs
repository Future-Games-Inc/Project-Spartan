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
    public Transform spawnPosition;

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
        Vector3 randomPosition = Random.insideUnitSphere * spawnRadius;
        randomPosition += transform.position;

        // Find the nearest point on the NavMesh to the random position
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPosition, out hit, spawnRadius, NavMesh.AllAreas))
        {
            object avatarSelectionNumber;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AVATAR_SELECTION_NUMBER, out avatarSelectionNumber))
            {
                int selectionValue = (int)avatarSelectionNumber;
                PhotonNetwork.Instantiate(playerPrefab[selectionValue].name, hit.position, Quaternion.identity);
            }

            spawnPosition.position = hit.position;
        }
    }
}
