using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneSpawner : MonoBehaviour
{
    public GameObject droneModel;
    public PlayerHealth player;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AICompanionSpawn());
    }

    IEnumerator AICompanionSpawn()
    {
        while (true)
        {
            //photonView.RPC("RPC_DroneSpawner", RpcTarget.All);
            yield return null;
        }
    }

    //[PunRPC]
    //void RPC_DroneSpawner()
    //{
    //    PhotonNetwork.Instantiate(droneModel.name, transform.position, Quaternion.identity);
    //    player.aiCompanion = false;
    //    this.gameObject.SetActive(false);
    //}
}
