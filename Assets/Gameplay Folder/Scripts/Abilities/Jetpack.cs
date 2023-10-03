using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Jetpack : MonoBehaviour
{
    public bool fuel = true;
    private float time = 0;
    public float flyTime = 3f;
    public float fallVelocity = -5f;
    public CharacterController character;
    public float liftVelocity = 100f;
    private Vector3 moveDirection = Vector3.zero;
    public bool slowFall = false;
    public InputActionProperty rightThumbstickPress;
    public AudioSource jetpackSource;
    public AudioClip jetpackclip;
    public GameObject fuelIcon;
    public GameObject playerGameObject;
    public Rigidbody playerRb;
    public bool activated;

    private void OnEnable()
    {
        StartCoroutine(Recharge());
    }
    void Update()
    {
        // If we're flying, increment the timer
        if (rightThumbstickPress.action.ReadValue<float>() >= .78f && fuel && !activated)
        {
            ActivateJetpack();
        }

        newJetpack();
        character.Move(moveDirection * Time.deltaTime);
        fuelIcon.SetActive(fuel);

        if(activated)
        {
            time += Time.deltaTime;
        }
    }

    void ActivateJetpack()
    {
        activated = true;
        character.Move(Vector3.up * liftVelocity * Time.fixedDeltaTime);
        slowFall = true;
        if (!jetpackSource.isPlaying)
        {
            jetpackSource.PlayOneShot(jetpackclip);
        }
    }

    public void newJetpack()
    {
        if (playerGameObject.GetComponent<Rigidbody>() == null && activated)
        {
            playerRb = playerGameObject.AddComponent<Rigidbody>();
            playerRb.constraints = RigidbodyConstraints.FreezeRotation;
            playerRb.useGravity = false; // Optional: This depends on if you want gravity to affect the player during the swing.
        }
        moveDirection = Vector3.zero;

        if (time > flyTime && fuel)
        {
            fuel = false;
            activated = false;
            StartCoroutine(Refuel());
        }

        if (character.isGrounded)
        {
            slowFall = false;
            jetpackSource.Stop();
            if (playerRb)
            {
                Destroy(playerRb);
            }
        }

        if (slowFall && !fuel)
        {
            moveDirection.y += fallVelocity;
        }
    }

    IEnumerator Refuel()
    {
        yield return new WaitForSeconds(10);
        time = 0.0f;
        fuel = true;
    }

    IEnumerator Recharge()
    {
        yield return new WaitForSeconds(20);
        time = 0.0f;
        fuel = true;
    }
}
