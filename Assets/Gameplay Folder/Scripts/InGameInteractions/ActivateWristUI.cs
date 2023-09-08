using System.Collections;
using Unity.VisualScripting;
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
    public GameObject codeScreen;
    public GameObject reactor;
    public float timer;

    public bool activated;
    public MatchEffects matchEffects;

    public GameObject identifier;
    public Rotator rotator;
    public Material deactivated;
    public Material far;
    public Material medium;
    public Material close;

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
        matchEffects = GameObject.FindGameObjectWithTag("Props").GetComponent<MatchEffects>();
        codeScreen = GameObject.FindGameObjectWithTag("Code");
        StartCoroutine(Identifier());
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

    IEnumerator Identifier()
    {
        while (true)
        {
            if (!matchEffects.startMatchBool)
            {
                identifier.GetComponent<MeshRenderer>().material = deactivated;
                rotator.speed = 10f;
            }

            else if (matchEffects.startMatchBool && !matchEffects.spawnReactor)
            {
                Vector3 pointA = this.gameObject.transform.position;
                Vector3 pointB = codeScreen.gameObject.transform.position;
                float distance = Vector3.Distance(pointA, pointB);
                if (distance >= 100f)
                {
                    identifier.GetComponent<MeshRenderer>().material = deactivated;
                    rotator.speed = 10f;
                }
                else if (distance >= 80f && distance < 100f)
                {
                    identifier.GetComponent<MeshRenderer>().material = far;
                    rotator.speed = 15f;
                }
                else if (distance >= 40f && distance < 80f)
                {
                    identifier.GetComponent<MeshRenderer>().material = medium;
                    rotator.speed = 20f;
                }
                else if (distance >= 20f && distance < 40f)
                {
                    identifier.GetComponent<MeshRenderer>().material = close;
                    rotator.speed = 30f;
                }
            }

            else if (matchEffects.startMatchBool && matchEffects.spawnReactor)
            {
                reactor = GameObject.FindGameObjectWithTag("Reactor");
                Vector3 pointA = this.gameObject.transform.position;
                Vector3 pointB = reactor.gameObject.transform.position;
                float distance = Vector3.Distance(pointA, pointB);
                if (distance >= 100f)
                {
                    identifier.GetComponent<MeshRenderer>().material = deactivated;
                    rotator.speed = 10f;
                }
                else if (distance >= 80f && distance < 100f)
                {
                    identifier.GetComponent<MeshRenderer>().material = far;
                    rotator.speed = 15f;
                }
                else if (distance >= 40f && distance < 80f)
                {
                    identifier.GetComponent<MeshRenderer>().material = medium;
                    rotator.speed = 20f;
                }
                else if (distance >= 0f && distance < 40f)
                {
                    identifier.GetComponent<MeshRenderer>().material = close;
                    rotator.speed = 30f;
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }
}
