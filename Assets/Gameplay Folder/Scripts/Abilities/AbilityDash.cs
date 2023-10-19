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

        boostAsPercent = PlayerPrefs.HasKey("PLAYER_DASH") && PlayerPrefs.GetInt("PLAYER_DASH") >= 1
            ? ((100 + boostPercentage) / 100) + ((int)(PlayerPrefs.GetInt("PLAYER_DASH") * .75)) : (100 + boostPercentage) / 100;

        coolDown = PlayerPrefs.HasKey("DASH_COOLDOWN") && PlayerPrefs.GetInt("DASH_COOLDOWN") >= 1
            ? 5 - (int)PlayerPrefs.GetInt("DASH_COOLDOWN") : 5;
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
