using UnityEngine;
using Photon.Pun;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerNetworkSetup : MonoBehaviourPunCallbacks
{
    public XROrigin localXRRigGameObject;
    public Camera myCamera;
    public PlayerMovement playerMovement;
    public AbilityDash dash;
    public ActionBasedController[] controller;
    public HandPoseAnimator[] handPoseAnimators;
    public ActivateWristUI wristUI;
    public ActionBasedSnapTurnProvider controllerSnapTurnProvider;
    public ActionBasedContinuousTurnProvider controllerContinuousTurnProvider;
    public EarlyExtraction earlyExtraction;

    public TextMeshProUGUI[] playerNameText;
    public GameObject[] playerInformation;

    const string playerNamePrefKey = "PlayerName";
    // Start is called before the first frame update

    void OnEnable()
    {
        if (photonView != null)
        {
            if (!photonView.IsMine)
            {
                myCamera.enabled = false;
                localXRRigGameObject.enabled = false;
                playerMovement.enabled = false;
                dash.enabled = false;
                wristUI.enabled = false;
                controllerSnapTurnProvider.enabled = false;
                controllerContinuousTurnProvider.enabled = false;
                earlyExtraction.enabled = false;

                foreach (ActionBasedController controllers in controller)
                {
                    controllers.enabled = false;
                }

                foreach (HandPoseAnimator hands in handPoseAnimators)
                {
                    hands.enabled = false;
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
            }

            else if (photonView.IsMine)
            {
                myCamera.enabled = true;
                localXRRigGameObject.enabled = true;
                playerMovement.enabled = true;
                dash.enabled = true;
                controllerSnapTurnProvider.enabled = true;
                controllerContinuousTurnProvider.enabled = true;
                earlyExtraction.enabled = true;
                wristUI.enabled = true;

                foreach (GameObject information in playerInformation)
                {
                    information.SetActive(false);
                }

                foreach (ActionBasedController controllers in controller)
                {
                    controllers.enabled = true;
                }

                foreach (HandPoseAnimator hands in handPoseAnimators)
                {
                    hands.enabled = true;
                }

                if (photonView.Owner.NickName != null)
                {
                    foreach (TextMeshProUGUI playerText in playerNameText)
                    {
                        playerText.text = photonView.Owner.NickName;
                    }

                }
                else if (photonView.Owner.NickName == null)
                {
                    foreach (TextMeshProUGUI playerText in playerNameText)
                    {
                        playerText.text = "Unknown React";
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}