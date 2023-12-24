using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static DroneHealth;

public class SlowMotionManager : MonoBehaviour
{
    public float slowDownFactor = 0.25f;
    private bool isSlowMotionActive = false;
    private bool charged;
    public bool available = true;
    public InputActionProperty rightThumbstickPress;
    public GameObject warpIcon;
    public float time = 0;
    public float warpTime = 5; // Time during which slow-motion is active
    private float cooldownTime = 0; // Time until the next slow-motion is available
    private const float cooldownDuration = 10; // Cooldown duration in seconds

    public bool IsSlowMotionActive; // Static flag
    public AudioSource audioSource;
    public AudioClip audioClip;

    private void OnEnable()
    {
        StartCoroutine(Recharge());
    }
    void Update()
    {
        if (isSlowMotionActive)
        {
            time += Time.deltaTime;

            if (time >= warpTime)
            {
                DeactivateSlowMotion();
                time = 0;
                cooldownTime = 0;
            }
        }
        else if (!available && charged)
        {
            cooldownTime += Time.deltaTime;

            if (cooldownTime >= cooldownDuration)
            {
                available = true;
            }
        }

        if (rightThumbstickPress.action.ReadValue<float>() >= .6f && !isSlowMotionActive && available)
        {
            ActivateSlowMotion();
        }

        warpIcon.SetActive(available && charged);
    }

    void ActivateSlowMotion()
    {
        isSlowMotionActive = true;
        available = false;
        audioSource.PlayOneShot(audioClip);
        GlobalSpeedManager.SpeedMultiplier = slowDownFactor;
    }

    void DeactivateSlowMotion()
    {
        isSlowMotionActive = false;
        audioSource.Stop();
        GlobalSpeedManager.SpeedMultiplier = 1;
    }

    IEnumerator Recharge()
    {
        yield return new WaitForSeconds(20);
        charged = true;
        available = true;
    }
}
