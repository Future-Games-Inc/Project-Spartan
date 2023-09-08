using UnityEngine;
using UnityEngine.InputSystem;

public class ActivateWristUI : MonoBehaviour
{
    public InputActionProperty leftThumbstickPress;

    public GameObject uiCanvas;
    public GameObject miniMap;
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

        if (PlayerPrefs.HasKey("BossQuest") && PlayerPrefs.GetInt("BossQuest") == 1)
            BossIcon.SetActive(activated);

        if (PlayerPrefs.HasKey("ArtifactQuest") && PlayerPrefs.GetInt("ArtifactQuest") == 1)
            ArtifactIcon.SetActive(activated);

        if (PlayerPrefs.HasKey("BombQuest") && PlayerPrefs.GetInt("BombQuest") == 1)
            BombIcon.SetActive(activated);

        if (PlayerPrefs.HasKey("GuardianQuest") && PlayerPrefs.GetInt("GuardianQuest") == 1)
            GuardianIcon.SetActive(activated);

        if (PlayerPrefs.HasKey("IntelQuest") && PlayerPrefs.GetInt("IntelQuest") == 1)
            IntelIcon.SetActive(activated);

        if (PlayerPrefs.HasKey("CollectorQuest") && PlayerPrefs.GetInt("CollectorQuest") == 1)
            CollectorIcon.SetActive(activated);
    }
}
