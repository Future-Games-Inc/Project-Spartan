using UnityEngine;
using UnityEngine.InputSystem;

public class ActivateWristUI : MonoBehaviour
{
    public InputActionProperty leftThumbstickPress;

    public GameObject uiCanvas;
    public GameObject miniMap;
    public GameObject scoreboard;
    public float timer;

    public bool activated;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0f;
        activated = false;
        uiCanvas.SetActive(false);
        miniMap.SetActive(false);
        scoreboard.SetActive(false);
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

        if (activated == true)
        {
            uiCanvas.SetActive(true);
            miniMap.SetActive(true);
            scoreboard.SetActive(true);
        }
        else
        {
            uiCanvas.SetActive(false);
            miniMap.SetActive(false);
            scoreboard.SetActive(false);
        }
    }
}
