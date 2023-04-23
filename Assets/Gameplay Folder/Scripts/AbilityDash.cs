using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

public class AbilityDash : Abilities
{
    public XRNode right_HandButtonSource;
    [SerializeField] private bool secondaryButtonPressed;

    [SerializeField] private int boostPercentage;
    [SerializeField] PlayerMovement movement;

    private float boostAsPercent;

    public AudioSource audioSource;
    public AudioClip dashClip;

    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<PlayerMovement>();

        object storedPlayerDash;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.PLAYER_DASH, out storedPlayerDash) && (int)storedPlayerDash >= 1)
            boostAsPercent = ((100 + boostPercentage) / 100) +((int)storedPlayerDash * (int)0.75);
        else
            boostAsPercent = (100 + boostPercentage) / 100;

        object storedDashCooldown;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.DASH_COOLDOWN, out storedDashCooldown) && (int)storedDashCooldown >= 1)
            coolDown = 5 - (int)storedDashCooldown;
        else
            coolDown = 5;
    }

    // Update is called once per frame
    void Update()
    {
        InputDevice dashButton = InputDevices.GetDeviceAtXRNode(right_HandButtonSource);
        dashButton.TryGetFeatureValue(CommonUsages.secondaryButton, out secondaryButtonPressed);

        if (Time.time >= abilityTimer && secondaryButtonPressed)
        {
            AbilityEffect();
            abilityTimer = Time.time + coolDown;

        }
    }

    private void AbilityEffect()
    {
        movement.Boost(boostAsPercent);
        audioSource.PlayOneShot(dashClip);
        Invoke("ResetAbility", duration);
    }

    private void ResetAbility()
    {
        movement.ResetBoost(boostAsPercent);   
    }
}
