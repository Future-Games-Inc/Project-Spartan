using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActivateWristUI : MonoBehaviour
{
    public InputActionProperty leftThumbstickPress;

    public GameObject uiCanvas;
    public GameObject miniMap;
    public GameObject scoreboard;

    public bool activated;

    // Start is called before the first frame update
    void Start()
    {
        activated = false;
        uiCanvas.SetActive(false);
        miniMap.SetActive(false);
        scoreboard.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (leftThumbstickPress.action.ReadValue<float>() >= .78f && activated == false)
        {
            StartCoroutine(WristUI());
        }

        if (leftThumbstickPress.action.ReadValue<float>() >= .78f && activated == true)
        {
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

    IEnumerator WristUI()
    {
        yield return new WaitForSeconds(0);
        activated = true;
    }
}
