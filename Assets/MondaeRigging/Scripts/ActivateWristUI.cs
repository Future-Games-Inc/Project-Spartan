using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActivateWristUI : MonoBehaviour
{
    public InputActionProperty pinchAnimationAction;
    public InputActionProperty gripAnimationAction;

    public float triggerValue;
    public float pinchValue;

    public GameObject uiCanvas;
    public GameObject miniMap;
    public GameObject scoreboard;

    public bool activated;
    public float timer;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0f;
        activated = false;
        triggerValue = pinchAnimationAction.action.ReadValue<float>();
        pinchValue = gripAnimationAction.action.ReadValue<float>();
        uiCanvas.SetActive(false);
        miniMap.SetActive(false);
        scoreboard.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        triggerValue = pinchAnimationAction.action.ReadValue<float>();
        pinchValue = gripAnimationAction.action.ReadValue<float>();

        if (triggerValue >= .98f && pinchValue >= .98f && activated == false)
        {
            timer += Time.deltaTime;
            if (timer >= 1f)
            {
                StartCoroutine(WristUI());
            }
        }

        if (triggerValue < .98f && pinchValue < .98f)
        {
            timer = 0f;
        }

        if (triggerValue >= .98f && activated == true && timer < 1f || pinchValue >= .98f && activated == true && timer <1f)
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
