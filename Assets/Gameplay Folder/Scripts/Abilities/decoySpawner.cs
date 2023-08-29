using Photon.Pun;
using System.Collections;
using UnityEngine;

public class decoySpawner : MonoBehaviour
{
    public Transform[] spawnLocations;
    public GameObject decoyModel;
    public PlayerHealth player;

    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(DecoySpawnActive());
    }

    IEnumerator DecoySpawnActive()
    {
        yield return new WaitForSeconds(0);
        foreach (Transform spawnPoint in spawnLocations)
        {
            PhotonNetwork.InstantiateRoomObject(decoyModel.name, spawnPoint.position, Quaternion.identity, 0, null);
        }
        player.decoyDeploy = false;
        this.gameObject.SetActive(false);
    }
}
