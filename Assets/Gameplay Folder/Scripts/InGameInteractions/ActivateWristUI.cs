using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class ActivateWristUI : MonoBehaviour
{
    public InputActionProperty leftThumbstickPress;

    public GameObject uiCanvas;
    public GameObject miniMap;
    public GameObject scoreboard;
    public GameObject BossIcon;
    public GameObject BombIcon;
    public GameObject IntelIcon;
    public GameObject GuardianIcon;
    public GameObject CollectorIcon;
    public GameObject ArtifactIcon;
    public float timer;

    public bool activated;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0f;
        activated = false;
        uiCanvas.SetActive(activated);
        miniMap.SetActive(activated);
        scoreboard.SetActive(activated);
        BossIcon.SetActive(activated);
        BombIcon.SetActive(activated);
        IntelIcon.SetActive(activated);
        GuardianIcon.SetActive(activated);
        CollectorIcon.SetActive(activated);
        ArtifactIcon.SetActive(activated);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (leftThumbstickPress.action.ReadValue<float>() >= .78f && activated == false && timer >= 1f)
        {
            timer = 0f;
            activated = true;
        }

        if (leftThumbstickPress.action.ReadValue<float>() >= .78f && activated == true && timer >= 1f)
        {
            timer = 0f;
            activated = false;
        }

        uiCanvas.SetActive(activated);
        miniMap.SetActive(activated);
        scoreboard.SetActive(activated);

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.BossQuest, out object contractState) && (bool)contractState == true)
            BossIcon.SetActive(activated);

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.ArtifactQuest, out object contractState2) && (bool)contractState2 == true)
            ArtifactIcon.SetActive(activated);

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.BombQuest, out object contractState3) && (bool)contractState3 == true)
            BombIcon.SetActive(activated);

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.GuardianQuest, out object contractState4) && (bool)contractState4 == true)
            GuardianIcon.SetActive(activated);

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.IntelQuest, out object contractState5) && (bool)contractState5 == true)
            IntelIcon.SetActive(activated);

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.CollectorQuest, out object contractState6) && (bool)contractState6 == true)
            ArtifactIcon.SetActive(activated);
    }
}
