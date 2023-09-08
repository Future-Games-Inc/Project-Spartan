using UnityEngine;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerNetworkSetup : MonoBehaviour
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
        foreach (TextMeshProUGUI playerText in playerNameText)
        {
            playerText.text = "Unknown React";
        }
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

        if (PlayerPrefs.GetString(playerNamePrefKey) != null)
        {
            foreach (TextMeshProUGUI playerText in playerNameText)
            {
                playerText.text = PlayerPrefs.GetString(playerNamePrefKey);
            }

        }
        else if (PlayerPrefs.GetString(playerNamePrefKey) == null)
        {
            foreach (TextMeshProUGUI playerText in playerNameText)
            {
                playerText.text = "Unknown React";
            }
        }
    }
}