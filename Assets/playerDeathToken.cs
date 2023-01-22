using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerDeathToken : MonoBehaviour
{
    public int tokenValue;
    public string faction;

    public bool tokenActivated = false;

    PhotonView pV;
    // Start is called before the first frame update
    void Start()
    {
        pV = GetComponent<PhotonView>();
        StartCoroutine(TokenActivation());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        pV.RPC("RPC_Trigger", RpcTarget.AllBuffered, other);
    }

    private IEnumerator TokenActivation()
    {
        yield return new WaitForSeconds(1f);
        pV.RPC("RPC_TokenActivated", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void RPC_TokenActivated()
    {
        if (!pV.IsMine) { return; }
        tokenActivated = true;
    }

    [PunRPC]
    void RPC_Trigger(Collider other)
    {
        if(!pV.IsMine) { return; }

        if (other.CompareTag("Player") && tokenActivated == true)
        {
            other.gameObject.GetComponent<PlayerHealth>().UpdateSkills(tokenValue);
            tokenValue = 0;

            if (faction.ToString() != other.gameObject.GetComponent<PlayerHealth>().characterFaction.ToString())
            {
                other.GetComponent<PlayerHealth>().FactionDataCard(faction);
                PhotonNetwork.Destroy(gameObject);
            }

            PhotonNetwork.Destroy(gameObject);
        }

    }
}
