using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Jetpack : MonoBehaviour
{
    public bool fuel = false;
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
    private Rigidbody playerRb;
    public bool activated = false;
    private Coroutine rechargeCoroutine = null;

    private void OnEnable()
    {
        StartCoroutine(Refuel());
    }

    void Update()
    {
        // Jetpack Activation
        if (rightThumbstickPress.action.ReadValue<float>() >= 0.78f && fuel && !activated)
        {
            ActivateJetpack();
        }

        // Increment time while jetpack is activated
        if (activated)
        {
            time += Time.deltaTime;
        }

        // SlowFall Activation
        if (time > flyTime && fuel)
        {
            fuel = false;
            activated = false;
            moveDirection.y += fallVelocity;
            StartCoroutine(Refuel());
        }

        // Landing on Ground
        if (character.isGrounded)
        {
            slowFall = false;
            moveDirection = Vector3.zero; // Reset the moveDirection.
            jetpackSource.Stop();
            if (playerRb)
            {
                Destroy(playerRb);
            }
            if (rechargeCoroutine == null) // Only start the coroutine if it's not already running.
            {
                rechargeCoroutine = StartCoroutine(Recharge());
            }
        }

        character.Move(moveDirection * Time.deltaTime);
        fuelIcon.SetActive(fuel);
    }

    void ActivateJetpack()
    {
        activated = true;
        moveDirection = Vector3.up * liftVelocity;

        if (!playerRb) // Only create the Rigidbody if it doesn't exist.
        {
            playerRb = playerGameObject.AddComponent<Rigidbody>();
            playerRb.constraints = RigidbodyConstraints.FreezeRotation;
            playerRb.useGravity = false;
        }

        if (!jetpackSource.isPlaying)
        {
            jetpackSource.PlayOneShot(jetpackclip);
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
        rechargeCoroutine = null; // Reset the coroutine tracker.
    }
}
