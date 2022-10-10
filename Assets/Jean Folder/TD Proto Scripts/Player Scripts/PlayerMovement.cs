using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Unity.XR.CoreUtils;
// using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Speed")]
    public float movementSpeed = 1f;
    [Header("Controller Movement")]
    public XRNode inputSource;

    private XROrigin rig;

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

    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<CharacterController>();
        rig = GetComponent<XROrigin>();

    }

    // Update is called once per frame
    void Update()
    {
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
        character.Move(direction * Time.fixedDeltaTime * movementSpeed);

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
            character.Move(Vector3.up * jumpVelocity * Time.fixedDeltaTime);

        }
        else if (!isJumping && CheckIfGrounded() && primaryButtonPressed)
        {
            isJumping = false;
        }

    }

    public void Boost(float buff)
    {
        Debug.Log("Boost Ability Activated");
        movementSpeed = movementSpeed * buff;

    }


    public void ResetBoost(float buff)
    {
        movementSpeed = movementSpeed / buff;

    }



}
