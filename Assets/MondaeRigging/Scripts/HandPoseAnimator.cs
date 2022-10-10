using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;



public class HandPoseAnimator : MonoBehaviour
{
    public InputActionProperty pinchAnimationAction;
    public InputActionProperty gripAnimationAction;

    public Animator[] handAnimator;
    public string whichHand = "";

    // Update is called once per frame
    void Update()
    {
        foreach (Animator animations in handAnimator)
        {
            float triggerValue = pinchAnimationAction.action.ReadValue<float>();
            animations.SetFloat("Trigger_" + whichHand, triggerValue);

            float pinchValue = gripAnimationAction.action.ReadValue<float>();
            animations.SetFloat("Grip_" + whichHand, pinchValue);
        }
    }
}
