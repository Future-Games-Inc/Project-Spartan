using Photon.Pun;
using UnityEngine;

public class SocketInteractor : MonoBehaviourPunCallbacks
{
    private Vector3 originalScale;
    private bool isObjectInSocket = false;

    private void OnEnable()
    {
        originalScale = transform.localScale;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isObjectInSocket && other.CompareTag("PickupStorage"))
        {
            // Object is placed in the socket interactor
            isObjectInSocket = true;
            photonView.RPC("RPC_Scale", RpcTarget.All, originalScale * 0.5f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isObjectInSocket && other.CompareTag("PickupStorage"))
        {
            // Object is removed from the socket interactor
            isObjectInSocket = false;
            photonView.RPC("RPC_Scale", RpcTarget.All, originalScale);
        }
    }

    [PunRPC]
    void RPC_Scale(Vector3 scale)
    {
        transform.localScale = scale;
    }
}
