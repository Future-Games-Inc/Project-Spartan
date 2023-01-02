using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;
using Photon.Realtime;

public class ConnectToPhoton : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.ConnectUsingSettings();
        }

    }

    // Update is called once per frame
    public override void OnConnected()
    {
        Debug.Log("OnConnectred called. Server available.");
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Conencted to Master Server with player name: " + PhotonNetwork.LocalPlayer.NickName);
    }
}
