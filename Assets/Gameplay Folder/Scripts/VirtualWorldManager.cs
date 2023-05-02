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
        if (PhotonNetwork.LocalPlayer.CustomProperties["IsDead"] != null && (bool)PhotonNetwork.LocalPlayer.CustomProperties["IsDead"])
        {
            // Check if the local player is the master client
            if (PhotonNetwork.IsMasterClient)
            {
                // Transfer master client to the next player in the room
                Photon.Realtime.Player[] otherPlayers = PhotonNetwork.PlayerListOthers;
                if (otherPlayers.Length > 0)
                {
                    PhotonNetwork.SetMasterClient(otherPlayers[0]);
                }
            }

            // Leave the room and load the new scene
            PhotonNetwork.LeaveRoom();
        }
    }

    #region Photon Callback Methods 

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("TD Main Menu");
    }

    #endregion
}
