using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AbilityDash : Abilities
{
    public InputActionReference rightThumstickPress = null;

    [SerializeField] private int boostPercentage;
    [SerializeField] PlayerMovement movement;

    private float boostAsPercent;

    private void Awake()
    {
        rightThumstickPress.action.performed += RH_ThumbstickPress;
    }

    private void OnDestroy()
    {
        rightThumstickPress.action.performed -= RH_ThumbstickPress;
    }


    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<PlayerMovement>();
        boostAsPercent = (100 + boostPercentage) / 100;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time >= abilityTimer && rightThumstickPress.action.IsPressed())
        {
            AbilityEffect();
            abilityTimer = Time.time + coolDown;

        }
    }

    private void AbilityEffect()
    {
        movement.Boost(boostAsPercent);
        Invoke("ResetAbility", duration);
    }

    private void ResetAbility()
    {
        movement.ResetBoost(boostAsPercent);   
    }

    private void RH_ThumbstickPress(InputAction.CallbackContext context)
    {


    }
}
