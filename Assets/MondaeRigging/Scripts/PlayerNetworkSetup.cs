using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using RootMotion.FinalIK;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using Unity.VisualScripting;

public class PlayerNetworkSetup : MonoBehaviourPunCallbacks
{
    public GameObject localXRRigGameObject;
    public VRIK localVRIK;

    public GameObject[] AvatarModelPrefabs;

    public TextMeshProUGUI[] playerNameText;

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            localXRRigGameObject.SetActive(true);
            localVRIK.enabled = true;

            object avatarSelectionNumber;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AVATAR_SELECTION_NUMBER, out avatarSelectionNumber))
            {
                photonView.RPC("InitializeSelectedAvatarModel", RpcTarget.AllBuffered, (int)avatarSelectionNumber);
            }
        }
        else
        {
            localXRRigGameObject.SetActive(false);
            localVRIK.enabled = false;
        }

        if (PhotonNetwork.LocalPlayer.NickName != null)
        {
            foreach (TextMeshProUGUI playerText in playerNameText)
            {
                playerText.text = PhotonNetwork.NickName;
            }

        }
        else
        {
            foreach (TextMeshProUGUI playerText in playerNameText)
            {
                playerText.text = "Unknown React";
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    [PunRPC]
    public void InitializeSelectedAvatarModel(int avatarSelectionNumber)
    {
        for (int i = 0; i < AvatarModelPrefabs.Length; i++)
        {
            if (AvatarModelPrefabs[avatarSelectionNumber] == AvatarModelPrefabs[i])
            {
                AvatarModelPrefabs[i].SetActive(true);
            }
            else
            {
                AvatarModelPrefabs[i].SetActive(false);
            }
        }
    }
}
