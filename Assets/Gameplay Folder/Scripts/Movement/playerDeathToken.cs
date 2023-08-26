using Photon.Pun;
using System.Collections;
using UnityEngine;

public class playerDeathToken : MonoBehaviourPunCallbacks
{
    public int tokenValue;
    public string faction;
    public PlayerHealth player;

    public bool tokenActivated = false;

    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(TokenActivation());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && tokenActivated == true && other.GetComponent<PlayerHealth>().alive == true)
        {
            player = other.GetComponent<PlayerHealth>();
            photonView.RPC("RPC_DestroyToken", RpcTarget.All);
        }
    }

    private IEnumerator TokenActivation()
    {
        yield return new WaitForSeconds(1f);
        photonView.RPC("RPC_TokenActivated", RpcTarget.All);
    }

    [PunRPC]
    void RPC_TokenActivated()
    {
        if (!photonView.IsMine)
            return;
        tokenActivated = true;       
    }

    [PunRPC]
    void RPC_DestroyToken()
    {
        if (!photonView.IsMine)
            return;
        player.UpdateSkills(tokenValue);
        tokenValue = 0;
        //if (faction.ToString() != player.characterFaction.ToString())
        //{
            player.FactionDataCard();
            PhotonNetwork.Destroy(gameObject);
        //}
    }
}
