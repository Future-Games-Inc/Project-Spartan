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
    void Start()
    {
        StartCoroutine(TokenActivation());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && tokenActivated == true)
        {
            player = other.GetComponent<PlayerHealth>();
            photonView.RPC("RPC_DestroyToken", RpcTarget.All, 0, null);
        }
    }

    private IEnumerator TokenActivation()
    {
        yield return new WaitForSeconds(1f);
        photonView.RPC("RPC_TokenActivated", RpcTarget.All, 0, null);
    }

    [PunRPC]
    void RPC_TokenActivated()
    {
        tokenActivated = true;       
    }

    [PunRPC]
    void RPC_DestroyToken()
    {
        player.UpdateSkills(tokenValue);
        tokenValue = 0;
        //if (faction.ToString() != player.characterFaction.ToString())
        //{
            player.FactionDataCard();
            PhotonNetwork.Destroy(gameObject);
        //}
    }
}
