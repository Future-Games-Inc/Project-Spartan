using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeflectionActivator : MonoBehaviour
{
    public GameObject shield;
    public bool activated;
    public bool canBeActivated;
    public float time;
    public float shieldTimer;
    public GameObject rechargeIcon;

    public AudioSource audioSource;
    public AudioClip audioClip;

    public InputActionProperty rightThumbstickPress;

    public PlayerHealth player;
    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(RechargeStart());
    }

    // Update is called once per frame
    void Update()
    {
        rechargeIcon.SetActive(activated);

        // Activate the shield if thumbstick is pressed and shield is not already activated.
        if (rightThumbstickPress.action.ReadValue<float>() >= 0.78f && !activated && canBeActivated && time == 0.0f)
        {
            audioSource.PlayOneShot(audioClip);
            activated = true;
            canBeActivated = false;
            shield.SetActive(true);
            rechargeIcon.SetActive(false);
            player.isShielded = true;
        }

        // Increment time only if the shield is active
        if (activated)
        {
            time += Time.deltaTime;
        }

        // Deactivate the shield if time exceeds the shield timer
        if (time > shieldTimer && activated)
        {
            audioSource.Stop();
            activated = false;
            shield.SetActive(false);
            StartCoroutine(Recharge());
            player.isShielded = false;
        }
    }

    IEnumerator Recharge()
    {
        yield return new WaitForSeconds(10);
        time = 0.0f;
        canBeActivated = true;

    }

    IEnumerator RechargeStart()
    {
        yield return new WaitForSeconds(20);
        time = 0.0f;
        canBeActivated = true;
    }
}
