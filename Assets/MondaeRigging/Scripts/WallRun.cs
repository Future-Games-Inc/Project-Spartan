using UnityEngine;
using UnityEngine.XR;

public class WallRun : MonoBehaviour
{
    [Header("Wall Running")]
    public float wallRunSpeed = 10f; // Speed when wall running
    public float wallRunGravity = -10f; // Gravity while wall running
    public float wallRunJumpForce = 3f; // Jump force while wall running
    public float maxWallAngle = 90f; // Maximum angle of wall for wall running
    public float minWallSpeed = 3f; // Minimum speed required to wall run

    [Header("Ground Checking")]
    public float groundCheckDistance = 0.1f; // Distance to check for ground
    public LayerMask groundLayers; // Layers to consider as ground
    public Transform groundCheck; // Transform representing position to check for ground

    [Header("Input")]
    public XRNode leftHandController; // Left hand controller for input
    public XRNode right_HandButtonSource;

    private CharacterController characterController;
    private PlayerMovement playerMovement;
    public bool isWallRunning = false;
    private Vector3 wallNormal = Vector3.zero;
    private Vector3 wallDirection;
    private float distanceToWall;

    [SerializeField] private bool primaryButtonPressed;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        // Check if we are on the ground
        bool isGrounded = CheckGrounded();

        // If we are wall running and not grounded, apply wall running movement
        if (isWallRunning && !isGrounded)
        {
            ApplyWallRunningMovement();
        }

        InputDevice jumpbutton = InputDevices.GetDeviceAtXRNode(right_HandButtonSource);
        jumpbutton.TryGetFeatureValue(CommonUsages.primaryButton, out primaryButtonPressed);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if we are on the ground
        bool isGrounded = CheckGrounded();
        CalculateDistanceToWall(other);

        // If we collide with a wall and are moving fast enough, start wall running
        if (!isWallRunning && IsWall(other.gameObject) && !isGrounded)
        {
            Debug.Log("Wall");
            Vector3 velocity = characterController.velocity;
            float speed = velocity.magnitude;
            Vector3 direction = velocity.normalized;
            Vector3 wallNormal = other.transform.up;

            if (Vector3.Angle(direction, wallNormal) <= maxWallAngle)
            {
                StartWallRunning(wallNormal);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If we stop colliding with the wall, stop wall running
        if (isWallRunning && IsWall(other.gameObject))
        {
            StopWallRunning();
        }
    }

    public bool CheckGrounded()
    {
        // Check if we are on the ground by casting a ray downwards
        bool isGrounded = characterController.isGrounded;
        return isGrounded;
    }

    public bool IsWall(GameObject gameObject)
    {
        // Check if the object is a wall by checking if it has a wall tag
        return gameObject.CompareTag("Wall");
    }

    private void StartWallRunning(Vector3 wallNormal)
    {
        // Start wall running
        isWallRunning = true;
        this.wallNormal = wallNormal;
        characterController.enabled = false; // Disable the character controller to avoid getting stuck on the wall
        playerMovement.enabled = false;
    }

    private void StopWallRunning()
    {
        // Stop wall running
        isWallRunning = false;
        wallNormal = Vector3.zero;

        // Gradually reduce the player's velocity towards the opposite direction of the wall's normal vector
        Vector3 oppositeDirection = -characterController.velocity.normalized;
        float speedReductionRate = 5f; // Adjust this to control the speed reduction rate
        while (Vector3.Dot(characterController.velocity, oppositeDirection) > 0f)
        {
            characterController.Move(oppositeDirection * Time.deltaTime * speedReductionRate);
        }

        characterController.enabled = true; // Enable the character controller again
        playerMovement.enabled = true;
    }

    void CalculateDistanceToWall(Collider other)
    {
        distanceToWall = Vector3.Distance(transform.position, other.ClosestPoint(transform.position));
    }

    private void ApplyWallRunningMovement()
    {
        Collider other = GetComponent<Collider>();
        // Determine the wall running direction (left or right)
        float wallDirection = Vector3.Dot(wallNormal, transform.right);
        if (wallDirection > 0f)
        {
            wallDirection = 1f; // Right
        }
        else if (wallDirection < 0f)
        {
            wallDirection = -1f; // Left
        }
        else
        {
            wallDirection = 0f;
        }

        // Calculate the wall running movement
        float xInput = wallDirection;
        float yInput = 0f;

        Vector3 direction = new Vector3(xInput, 0f, yInput).normalized;
        Vector3 wallTangent = Vector3.Cross(wallNormal, Vector3.up);

        // Calculate the movement vector along the wall
        Vector3 movement = wallTangent * direction.x + wallNormal * direction.z;

        // Calculate the vertical component to move in an arc
        float arcHeight = 1f; // Adjust this to control the height of the arc
        float verticalSpeed = Mathf.Sqrt(2 * wallRunGravity * arcHeight);
        float timeToPeak = verticalSpeed / -wallRunGravity;
        float timeToWall = distanceToWall / wallRunSpeed;
        float totalTime = timeToPeak + timeToWall;
        float verticalDistance = arcHeight + 0.5f * wallRunGravity * Mathf.Pow(totalTime, 2f);
        Vector3 verticalMovement = Vector3.up * verticalDistance;

        // Apply the wall running movement
        characterController.Move((movement * wallRunSpeed + verticalMovement) * Time.deltaTime);

        // Apply gravity while wall running
        characterController.Move(Vector3.down * wallRunGravity * Time.deltaTime);

        if (isWallRunning && primaryButtonPressed)
        {
            // Jump off the wall
            Vector3 jumpDirection = wallTangent.normalized + Vector3.up;
            characterController.enabled = true; // Enable the character controller again
            playerMovement.enabled = false;
            characterController.Move(jumpDirection * wallRunJumpForce);
            isWallRunning = false; // Stop wall running
            wallNormal = Vector3.zero;
        }
    }
}