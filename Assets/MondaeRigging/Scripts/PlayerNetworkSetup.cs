using UnityEngine;
using Photon.Pun;
using RootMotion.FinalIK;
using TMPro;

public class PlayerNetworkSetup : MonoBehaviour
{
    public GameObject localXRRigGameObject;
    public VRIK localVRIK;
    public Camera myCamera;
    public PlayerMovement playerMovement;
    public AbilityDash dash;

    public GameObject[] AvatarModelPrefabs;

    public TextMeshProUGUI[] playerNameText;
    PhotonView photonView;

    const string playerNamePrefKey = "PlayerName";
    // Start is called before the first frame update

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        if(!photonView.IsMine)
        {
            myCamera.enabled = false;
            localXRRigGameObject.SetActive(false);
            localVRIK.enabled = false;
            playerMovement.enabled = false;
            dash.enabled = false;
        }

        else if (photonView.IsMine)
        {
            myCamera.enabled = true;
            localXRRigGameObject.SetActive(true);
            localVRIK.enabled = true;
            playerMovement.enabled = true;
            dash.enabled = true;

            object avatarSelectionNumber;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AVATAR_SELECTION_NUMBER, out avatarSelectionNumber))
            {
                photonView.RPC("InitializeSelectedAvatarModel", RpcTarget.AllBuffered, (int)avatarSelectionNumber);
            }
        }

        if (PlayerPrefs.HasKey(playerNamePrefKey))
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