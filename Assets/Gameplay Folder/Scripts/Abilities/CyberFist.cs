using BNG;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class CyberFist : MonoBehaviour
{
    public enum Hand { Left, Right }
    public Hand hapticHand = Hand.Right; // Default to right hand, but can be changed in the inspector

    private InputDevice device;
    public int damageAmount = 10;
    public Material transparent;
    public Material normalColor; // The default color of the fist.
    public Material hitColor; // The color of the fist when it hits an enemy.
    private MeshRenderer fistRenderer; // The renderer for the fist to change its color.
    public HandCollision inputSource;
    public Grabber grabber;
    public bool activated;

    private void Start()
    {
        // Initialize the fist's renderer.
        fistRenderer = GetComponent<MeshRenderer>();
        SetHapticHand(hapticHand);
    }

    void SetHapticHand(Hand hand)
    {
        XRNode targetNode = (hand == Hand.Left) ? XRNode.LeftHand : XRNode.RightHand;

        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(targetNode, devices);
        if (devices.Count > 0)
        {
            device = devices[0];
        }
    }

    private void Update()
    {
        if (!activated)
            fistRenderer.material = transparent;

        if (grabber.HeldGrabbable == null && inputSource.MakingFist)
        {
            activated = true;
            fistRenderer.material = normalColor;
        }
        else
            activated = false;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (activated)
        {
            if (collision.gameObject.CompareTag("BossEnemy") ||
                collision.gameObject.CompareTag("Enemy"))
            {
                // Assuming the enemy has a script that manages its health, called "EnemyHealth".
                FollowAI enemyHealth = collision.gameObject.GetComponentInParent<FollowAI>();

                if (enemyHealth != null) // If the enemy has an EnemyHealth script
                {
                    enemyHealth.TakeDamage(damageAmount);
                }
            }

            else if (collision.gameObject.CompareTag("Security"))
            {
                // Assuming the enemy has a script that manages its health, called "EnemyHealth".
                DroneHealth enemyHealth = collision.gameObject.GetComponentInParent<DroneHealth>();

                if (enemyHealth != null) // If the enemy has an EnemyHealth script
                {
                    enemyHealth.TakeDamage(damageAmount);
                }
                else
                {
                    SentryDrone enemyHealth2 = collision.gameObject.GetComponentInParent<SentryDrone>();

                    if (enemyHealth2 != null) // If the enemy has an EnemyHealth script
                    {
                        enemyHealth2.TakeDamage(damageAmount);
                    }
                }
            }

            // Haptic feedback
            HapticFeedback();

            // Change color of fist
            ChangeFistColor();
        }
    }

    private void HapticFeedback()
    {
        if (device.isValid)
        {
            // The values here are just examples. Adjust the duration, frequency, and amplitude as needed.
            HapticCapabilities capabilities;
            if (device.TryGetHapticCapabilities(out capabilities) && capabilities.supportsImpulse)
            {
                uint channel = 0;
                float duration = 0.5f;
                float amplitude = 0.5f;
                float frequency = 1.0f;
                device.SendHapticImpulse(channel, amplitude, duration);
            }
        }
    }

    private void ChangeFistColor()
    {
        if (fistRenderer != null)
        {
            fistRenderer.material = hitColor;
            Invoke("ResetFistColor", 1.5f); // Reset the color after 1 second.
        }
    }

    private void ResetFistColor()
    {
        if (fistRenderer != null)
        {
            fistRenderer.material = normalColor;
        }
    }
}
