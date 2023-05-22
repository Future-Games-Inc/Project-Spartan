using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PhotonHealth : MonoBehaviourPunCallbacks, IPunObservable
{
    public int playerHeath = 100;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //sync health
        if(stream.IsWriting)
        {
            stream.SendNext(playerHeath);
        }
        else
        {
            //we are reading
            playerHeath = (int)stream.ReceiveNext();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
