using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using RootMotion.FinalIK;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class PlayerNetworkSetup : MonoBehaviourPunCallbacks
{
    public GameObject localXRRigGameObject;
    public VRIK localVRIK;
    public VRAvatarCalibrator localCalibrator;

    public GameObject[] AvatarModelPrefabs;

    public TextMeshProUGUI playerNameText;

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            localXRRigGameObject.SetActive(true);
            localVRIK.enabled = true;
            localCalibrator.enabled = true;

            object avatarSelectionNumber;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AVATAR_SELECTION_NUMBER, out avatarSelectionNumber))
            {
                Debug.Log("Avatar Selection Number: " + (int)avatarSelectionNumber);
                photonView.RPC("InitializeSelectedAvatarModel", RpcTarget.AllBuffered, (int)avatarSelectionNumber);
            }

            TeleportationArea[] teleportationAreas = GameObject.FindObjectsOfType<TeleportationArea>();
            if (teleportationAreas.Length > 0)
            {
                Debug.Log("Found " + teleportationAreas.Length + " teleportation areas.");
                foreach (var item in teleportationAreas)
                {
                    item.teleportationProvider = localXRRigGameObject.GetComponent<TeleportationProvider>();
                }
            }
            localXRRigGameObject.AddComponent<AudioListener>();
        }
        else
        {
            localXRRigGameObject.SetActive(false);
            localVRIK.enabled = false;
            localCalibrator.enabled = false;
        }

        if(playerNameText.text != null)
        {
            playerNameText.text = photonView.Owner.NickName;
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

            //AvatarInputConverter avatarInputConverter = transform.GetComponent<AvatarInputConverter>();
            //AvatarHolder avatarHolder = selectedAvatarGameobject.GetComponent<AvatarHolder>();
            //SetUpAvatarGameobject(avatarHolder.HeadTransform, avatarInputConverter.AvatarHead);
            //SetUpAvatarGameobject(avatarHolder.BodyTransform, avatarInputConverter.AvatarBody);
            //SetUpAvatarGameobject(avatarHolder.HandLeftTransform, avatarInputConverter.AvatarHand_Left);
            //SetUpAvatarGameobject(avatarHolder.HandRightTransform, avatarInputConverter.AvatarHand_Right);
        }

        //void SetUpAvatarGameobject(Transform avatarModelTransform, Transform mainAvatarTransform)
        //{
        //    avatarModelTransform.SetParent(mainAvatarTransform);
        //    avatarModelTransform.localPosition = Vector3.zero;
        //    avatarModelTransform.localRotation = Quaternion.identity;
        //}
    }
}
