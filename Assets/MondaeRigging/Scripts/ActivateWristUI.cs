using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActivateWristUI : MonoBehaviour
{
    public InputActionProperty pinchAnimationAction;
    public InputActionProperty gripAnimationAction;

    public float triggerValue;
    public float pinchValue;

    public GameObject uiCanvas;
    // Start is called before the first frame update
    void Start()
    {
        triggerValue = pinchAnimationAction.action.ReadValue<float>();
        pinchValue = gripAnimationAction.action.ReadValue<float>();
        uiCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        triggerValue = pinchAnimationAction.action.ReadValue<float>();
        pinchValue = gripAnimationAction.action.ReadValue<float>();

        if (triggerValue >= .8f && pinchValue >= .8f)
        {
            uiCanvas.SetActive(true);
        }
        else
        {
            uiCanvas.SetActive(false);
        }
    }
}
