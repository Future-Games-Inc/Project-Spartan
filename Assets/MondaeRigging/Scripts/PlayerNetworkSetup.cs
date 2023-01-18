using UnityEngine;
using Photon.Pun;
using RootMotion.FinalIK;
using TMPro;

public class PlayerNetworkSetup : MonoBehaviourPunCallbacks
{
    public GameObject localXRRigGameObject;
    public Camera myCamera;
    public PlayerMovement playerMovement;
    public AbilityDash dash;

    public GameObject[] AvatarModelPrefabs;

    public TextMeshProUGUI[] playerNameText;
    PhotonView photonView;

    public string characterFaction;

    public GameObject[] cyberEmblem;
    public GameObject[] cintEmblem;
    public GameObject[] fedEmblem;
    public GameObject[] chaosEmblem;
    public GameObject[] muerteEmblem;

    const string playerNamePrefKey = "PlayerName";
    // Start is called before the first frame update

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (!photonView.IsMine)
        {
            myCamera.enabled = false;
            localXRRigGameObject.SetActive(false);
            playerMovement.enabled = false;
            dash.enabled = false;

            if (photonView.Owner.NickName != null)
            {
                foreach (TextMeshProUGUI playerText in playerNameText)
                {
                    playerText.text = photonView.Owner.NickName;
                }

            }
            else
            {
                foreach (TextMeshProUGUI playerText in playerNameText)
                {
                    playerText.text = "Unknown React";
                }
            }

            photonView.RPC("SetPlayerFaction", RpcTarget.AllBuffered);
        }

        else if (photonView.IsMine)
        {
            myCamera.enabled = true;
            localXRRigGameObject.SetActive(true);
            playerMovement.enabled = true;
            dash.enabled = true;

            object avatarSelectionNumber;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AVATAR_SELECTION_NUMBER, out avatarSelectionNumber))
            {
                photonView.RPC("InitializeSelectedAvatarModel", RpcTarget.AllBuffered, (int)avatarSelectionNumber);
            }

            if (photonView.Owner.NickName != null)
            {
                foreach (TextMeshProUGUI playerText in playerNameText)
                {
                    playerText.text = photonView.Owner.NickName;
                }

            }
            else
            {
                foreach (TextMeshProUGUI playerText in playerNameText)
                {
                    playerText.text = "Unknown React";
                }
            }

            object faction;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.CYBER_SK_GANG, out faction) && (int)faction >= 1)
            {
                characterFaction = "Cyber SK Gang".ToString();
                foreach (GameObject emblem in cyberEmblem)
                    emblem.SetActive(true);
            }
            else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.MUERTE_DE_DIOS, out faction) && (int)faction >= 1)
            {
                characterFaction = "Muerte De Dios".ToString();
                foreach (GameObject emblem in muerteEmblem)
                    emblem.SetActive(true);
            }
            else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.CHAOS_CARTEL, out faction) && (int)faction >= 1)
            {
                characterFaction = "Chaos Cartel".ToString();
                foreach (GameObject emblem in chaosEmblem)
                    emblem.SetActive(true);
            }
            else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.CINTSIX_CARTEL, out faction) && (int)faction >= 1)
            {
                characterFaction = "CintSix Cartel".ToString();
                foreach (GameObject emblem in cintEmblem)
                    emblem.SetActive(true);
            }
            else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.FEDZONE_AUTHORITY, out faction) && (int)faction >= 1)
            {
                characterFaction = "Federation Zone Authority".ToString();
                foreach (GameObject emblem in fedEmblem)
                    emblem.SetActive(true);
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

    [PunRPC]
    public void SetPlayerFaction()
    {
        object faction;
        if (photonView.Owner.CustomProperties.TryGetValue(MultiplayerVRConstants.CYBER_SK_GANG, out faction) && (int)faction >= 1)
        {
            characterFaction = "Cyber SK Gang".ToString();
            foreach (GameObject emblem in cyberEmblem)
                emblem.SetActive(true);
        }
        else if (photonView.Owner.CustomProperties.TryGetValue(MultiplayerVRConstants.MUERTE_DE_DIOS, out faction) && (int)faction >= 1)
        {
            characterFaction = "Muerte De Dios".ToString();
            foreach (GameObject emblem in muerteEmblem)
                emblem.SetActive(true);
        }
        else if (photonView.Owner.CustomProperties.TryGetValue(MultiplayerVRConstants.CHAOS_CARTEL, out faction) && (int)faction >= 1)
        {
            characterFaction = "Chaos Cartel".ToString();
            foreach (GameObject emblem in chaosEmblem)
                emblem.SetActive(true);
        }
        else if (photonView.Owner.CustomProperties.TryGetValue(MultiplayerVRConstants.CINTSIX_CARTEL, out faction) && (int)faction >= 1)
        {
            characterFaction = "CintSix Cartel".ToString();
            foreach (GameObject emblem in cintEmblem)
                emblem.SetActive(true);
        }
        else if (photonView.Owner.CustomProperties.TryGetValue(MultiplayerVRConstants.FEDZONE_AUTHORITY, out faction) && (int)faction >= 1)
        {
            characterFaction = "Federation Zone Authority".ToString();
            foreach (GameObject emblem in fedEmblem)
                emblem.SetActive(true);
        }
    }
}