using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine;

public class VirtualWorldManager : MonoBehaviourPunCallbacks
{
    public static VirtualWorldManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    public void LeaveRoomAndLoadHomeScene()
    {
        // Leave the room and load the new scene
        PhotonNetwork.LeaveRoom();
    }

    #region Photon Callback Methods 

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("TD Main Menu");
    }

    #endregion
}
