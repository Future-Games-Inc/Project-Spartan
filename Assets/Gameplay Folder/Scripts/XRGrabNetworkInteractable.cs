using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;

public class XRGrabNetworkInteractable : XRGrabInteractable
{
    private PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    [System.Obsolete]
    protected override void OnSelectEntered(XRBaseInteractor interactor)
    {
        if (photonView.Owner == PhotonNetwork.LocalPlayer)
        {
            return;
        }
        photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
        base.OnSelectEntered(interactor);
    }

    [System.Obsolete]
    protected override void OnSelectExited(XRBaseInteractor interactor)
    {
        if (!PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            photonView.TransferOwnership(PhotonNetwork.MasterClient);
        }
        base.OnSelectExited(interactor);
    }

}
