using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

public class RadioTowerPost : MonoBehaviourPunCallbacks
{
    public XRSocketInteractor socket;
    public TextMeshProUGUI displayText;
    public bool Connected;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
            photonView.RPC("RPC_RadioStart", RpcTarget.All);
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator CheckTower()
    {
        while(true)
        {
            while (!socket.hasSelection)
            {
                yield return null;
            }
            photonView.RPC("RPC_TowersConnected", RpcTarget.All);
        }
    }

    [PunRPC]
    void RPC_RadioStart()
    {
        displayText.fontSize = 24;
        displayText.text = "Disconneected";
        StartCoroutine(CheckTower());    
    }

    [PunRPC]
    void RPC_TowersConnected()
    {
        displayText.text = "Connected";
        Connected = true;
    }
}
