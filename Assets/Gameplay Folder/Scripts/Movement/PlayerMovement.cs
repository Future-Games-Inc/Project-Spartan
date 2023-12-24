using UnityEngine;
using UnityEngine.XR;
using Unity.XR.CoreUtils;
// using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Speed")]
    public float minSpeed = 6f;
    public float accelerationTime = 10f; // Time it takes to reach maxSpeed
    public float currentSpeed;

    [Header("Controller Movement")]
    public XRNode inputSource;
    public XROrigin rig;

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
    public bool isGrounded;


    private CharacterController character;

    [SerializeField] private bool primaryButtonPressed;

    private Camera playerCamera;

    public AudioSource audioSource;
    public AudioClip jumpClip;

    public PlayerHealth player;
    public bool canMove;

    // Start is called before the first frame update
    void OnEnable()
    {
        character = GetComponent<CharacterController>();
        currentSpeed = PlayerPrefs.HasKey("PLAYER_SPEED") && PlayerPrefs.GetInt("PLAYER_SPEED") >= 1
            ? minSpeed + ((int)(PlayerPrefs.GetInt("PLAYER_SPEED") / 10)) : minSpeed;

        playerCamera = rig.Camera.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
            canMove = true;
        else
            canMove = (player.activeState != PlayerHealth.States.Shocked);

        if (canMove)
        {
            InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);
            device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);

            InputDevice jumpButton = InputDevices.GetDeviceAtXRNode(right_HandButtonSource);
            jumpButton.TryGetFeatureValue(CommonUsages.primaryButton, out primaryButtonPressed);
        }
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            CapsuleFollowHeadset();

            Quaternion headYaw = Quaternion.Euler(0, y: rig.Camera.transform.eulerAngles.y, 0);

            Vector3 direction = headYaw * new Vector3(inputAxis.x, 0, inputAxis.y);
            character.Move(direction * Time.fixedDeltaTime * currentSpeed);

            isGrounded = CheckIfGrounded();
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
        float rayLength = character.center.y + 0.05f;
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
        if (canMove)
        {
            currentSpeed *= buff;
            accelerationTime = 0.25f;
        }
    }

    public void ResetBoost(float buff)
    {
        currentSpeed /= buff;
        accelerationTime = 1f;
    }
}
