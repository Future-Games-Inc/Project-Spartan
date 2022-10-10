using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHandOnInput : MonoBehaviour
{
    public Animator handAnimator;
    public InputActionProperty pinch_AnimationAction;
    public InputActionProperty grip_AnimationAction;

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       float trigger_Value = pinch_AnimationAction.action.ReadValue<float>();
        handAnimator.SetFloat("Trigger", trigger_Value);
        float grip_Value = grip_AnimationAction.action.ReadValue<float>();
        handAnimator.SetFloat("Grip", grip_Value);

    }
}
