using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DroneSpawner : MonoBehaviour
{
    public GameObject droneModel;
    public PlayerHealth player;

    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(AICompanionSpawn());
    }

    IEnumerator AICompanionSpawn()
    {
        yield return new WaitForSeconds(0);
        PhotonNetwork.Instantiate(droneModel.name, transform.position, Quaternion.identity);
        player.aiCompanion = false;
        this.gameObject.SetActive(false);
    }
}
