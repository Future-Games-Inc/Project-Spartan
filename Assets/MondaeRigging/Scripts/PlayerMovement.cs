using UnityEngine;
using UnityEngine.XR;
using Unity.XR.CoreUtils;
using Photon.Pun;
// using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Speed")]
    public float minSpeed = 3f;
    public float maxSpeed = 10f;
    public float accelerationTime = 3f; // Time it takes to reach maxSpeed
    public float currentSpeed;
    private float velocity;

    [Header("Controller Movement")]
    public XRNode inputSource;
    public XROrigin rig;
    private CharacterController characterController;
    private Vector3 previousPosition;

    [Header("Ground Floor & Gravity ")]
    public float gravity = -9.81f;
    public LayerMask groundLayer;

    [Header("Character Height")]
    public float additionalHeight = .10f;


    private float fallingSpeed;
    private Vector2 inputAxis;

    [Header("Right Controller ButtonSource")]
    public XRNode right_HandButtonSource;

    [Header("Jump Velocity")]
    public float jumpVelocity = 100f;
    public bool isJumping;


    private CharacterController character;

    [SerializeField] private bool primaryButtonPressed;

    private Camera playerCamera;

    public AudioSource audioSource;
    public AudioClip jumpClip;

    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<CharacterController>();

        object storedPlayerSpeed;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.PLAYER_SPEED, out storedPlayerSpeed) && (int)storedPlayerSpeed >= 1)
        {
            maxSpeed = (10f + ((int)storedPlayerSpeed / 10));
        }
        else
            maxSpeed = 10f;

        currentSpeed = minSpeed;
        playerCamera = rig.Camera.GetComponent<Camera>();
        previousPosition = character.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the player is moving
        bool isMoving = character.transform.position != previousPosition;

        // Update player speed based on movement
        if (isMoving)
        {
            // Increase speed gradually to maxSpeed
            currentSpeed = Mathf.SmoothDamp(currentSpeed, maxSpeed, ref velocity, accelerationTime);
        }
        else
        {
            // Reset speed to minSpeed when the player stops moving
            currentSpeed = minSpeed;
        }

        // Update the previous position variable for the next frame
        previousPosition = character.transform.position;

        InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);

        InputDevice jumpbutton = InputDevices.GetDeviceAtXRNode(right_HandButtonSource);
        jumpbutton.TryGetFeatureValue(CommonUsages.primaryButton, out primaryButtonPressed);

    }

    private void FixedUpdate()
    {
        CapsuleFollowHeadset();

        Quaternion headYaw = Quaternion.Euler(0, y: rig.Camera.transform.eulerAngles.y, 0);

        Vector3 direction = headYaw * new Vector3(inputAxis.x, 0, inputAxis.y);
        character.Move(direction * Time.fixedDeltaTime * currentSpeed);

        bool isGrounded = CheckIfGrounded();
        if (isGrounded)
        {
            fallingSpeed = 0;
            isJumping = false;
        }
        else
        {
            //Gravity
            fallingSpeed += gravity * Time.fixedDeltaTime;
            character.Move(Vector3.up * fallingSpeed * Time.fixedDeltaTime);
        }
        handleJumping();
    }

    void CapsuleFollowHeadset()
    {
        // changing the character height to the righ hight + additional height of our prefference
        character.height = rig.CameraInOriginSpaceHeight + additionalHeight;
        Vector3 capsulCenter = transform.InverseTransformPoint(rig.Camera.transform.position);
        character.center = new Vector3(capsulCenter.x, character.height / 2 + character.skinWidth, capsulCenter.z);
    }


    bool CheckIfGrounded()
    {
        // tells us if we are on the ground
        Vector3 rayStart = transform.TransformPoint(character.center);
        float rayLength = character.center.y + 0.01f;
        bool hasHit = Physics.SphereCast(rayStart, character.radius, Vector3.down, out RaycastHit hitInfo, rayLength, groundLayer);
        return hasHit;
    }

    // Todo make the jumping mechanism softer going up and quicker going down. 
    void handleJumping()
    {
        if (!isJumping && CheckIfGrounded() && primaryButtonPressed)
        {
            isJumping = true;
            // Calculate the jump direction based on the camera view
            Quaternion cameraPitchAndRoll = Quaternion.Euler(playerCamera.transform.eulerAngles.x, 0, 0);
            Quaternion cameraYaw = playerCamera.transform.rotation * Quaternion.Inverse(cameraPitchAndRoll);
            Vector3 jumpDirection = cameraYaw * Vector3.forward;
            jumpDirection.y = 1f; // add upward component to the jump direction

            character.Move(jumpDirection.normalized * jumpVelocity * Time.fixedDeltaTime);
            audioSource.PlayOneShot(jumpClip);

        }
        else if (!isJumping && CheckIfGrounded() && primaryButtonPressed)
        {
            isJumping = false;
        }
    }

    public void Boost(float buff)
    {
        Debug.Log("Boost Ability Activated");
        maxSpeed *= buff;
        accelerationTime = 0.25f;
    }

    public void ResetBoost(float buff)
    {
        maxSpeed /= buff;
        accelerationTime = 1f;
    }
}
