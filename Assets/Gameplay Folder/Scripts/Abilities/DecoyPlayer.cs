using System.Collections;
using UnityEngine;
using Photon.Pun;

public class DecoyPlayer : MonoBehaviourPunCallbacks
{
    public GameObject decoyDeath;
    public Animator animator;
    public bool active = true;

    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(DestroyDecoy());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet") && active || other.CompareTag("EnemyBullet") && active)
        StartCoroutine(DecoyKilled());
    }

    IEnumerator DestroyDecoy()
    {
        yield return new WaitForSeconds(15);
        PhotonNetwork.InstantiateRoomObject(decoyDeath.name, transform.position, Quaternion.identity, 0, null);
        photonView.RPC("RPC_DecoyDestroyed", RpcTarget.All);
        yield return new WaitForSeconds(3f);
        PhotonNetwork.Destroy(gameObject);
    }

    IEnumerator DecoyKilled()
    {
        PhotonNetwork.InstantiateRoomObject(decoyDeath.name, transform.position, Quaternion.identity, 0, null);
        photonView.RPC("RPC_DecoyKilled", RpcTarget.All);
        yield return new WaitForSeconds(3f);
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    void RPC_DecoyDestroyed()
    {
        if (!photonView.IsMine)
            return;
        active = false;
        animator.SetTrigger("Death");
    }

    [PunRPC]
    void RPC_DecoyKilled()
    {
        if (!photonView.IsMine)
            return;
        active = false;
        animator.SetTrigger("Death");
    }
}
