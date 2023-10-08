using System.Collections;
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
    private Rigidbody playerRb;
    public bool activated = false;

    private void OnEnable()
    {
        StartCoroutine(Recharge());
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
            jetpackSource.Stop();
            if (playerRb)
            {
                Destroy(playerRb);
            }
            StartCoroutine(Recharge());
        }

        character.Move(moveDirection * Time.deltaTime);
        fuelIcon.SetActive(fuel);
    }

    void ActivateJetpack()
    {
        activated = true;
        moveDirection = Vector3.up * liftVelocity;

        if (playerGameObject.GetComponent<Rigidbody>() == null)
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
    }
}
