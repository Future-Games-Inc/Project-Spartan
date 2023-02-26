using UnityEngine;
using UnityEngine.InputSystem;
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
    public InputActionProperty rightThumbstickPress;

    private CharacterController characterController;
    private PlayerMovement playerMovement;
    public bool isWallRunning = false;
    private Vector3 wallNormal = Vector3.zero;
    private Vector3 wallDirection;
    private float distanceToWall;
    public AudioSource audioSource;
    public AudioClip wallRunClip;

    [SerializeField] private bool primaryButtonPressed;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        // Check if we are on the ground
        bool movingFast = FastMoving();

        // If we are wall running and moving fast enough
        if (isWallRunning && movingFast && rightThumbstickPress.action.ReadValue<float>() >= .78f)
        {
            ApplyWallRunningMovement(wallNormal);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if we are moving fast enough
        bool movingFast = FastMoving();

        // If we collide with a wall and are moving fast enough, start wall running
        if (!isWallRunning && IsWall(other.gameObject) && movingFast)
        {
            CalculateDistanceToWall(other);
            Vector3 wallNormal = other.transform.up;
            float angleToWall = Vector3.Angle(transform.forward, wallNormal);

            if (angleToWall <= maxWallAngle)
            {
                // Start wall running
                isWallRunning = true;
                this.wallNormal = wallNormal;
                characterController.enabled = false; // Disable the character controller to avoid getting stuck on the wall
                playerMovement.enabled = false;

                // Set the player's velocity to the wall normal multiplied by the wall run speed
                Vector3 wallVelocity = wallNormal * wallRunSpeed;
                characterController.Move(wallVelocity * Time.deltaTime);
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

    public bool FastMoving()
    {
        // Check if we are on the ground by casting a ray downwards
        bool movingFast = playerMovement.currentSpeed >= 5;
        return movingFast;
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

    private void ApplyWallRunningMovement(Vector3 wallNormal)
    {
        // Determine the wall running direction (left or right)
        Vector3 wallTangent = Vector3.Cross(wallNormal, Vector3.up);
        float wallDirection = Vector3.Dot(transform.forward, wallTangent);
        wallDirection = Mathf.Sign(wallDirection);

        // Calculate the wall running movement
        float xInput = wallDirection;
        float yInput = 0f;

        // Project the movement vector onto the plane that is perpendicular to the wall normal
        Vector3 movement = Vector3.ProjectOnPlane(transform.forward, wallNormal).normalized;

        // Calculate the vertical component to move in an arc
        float arcHeight = 1f; // Adjust this to control the height of the arc
        float distanceToWallPlane = distanceToWall / Mathf.Cos(Vector3.Angle(transform.forward, wallNormal) * Mathf.Deg2Rad);
        float verticalDistance = Mathf.Max(distanceToWallPlane - 2f, 0f); // Subtract 2 meters to start the arc above the wall
        float verticalSpeed = Mathf.Sqrt(-2f * wallRunGravity * arcHeight);
        float timeToPeak = Mathf.Sqrt(-2f * arcHeight / wallRunGravity) + Mathf.Sqrt(2f * verticalDistance / wallRunGravity);
        float timeToFall = Mathf.Sqrt(-2f * (arcHeight - verticalDistance) / wallRunGravity);
        float jumpDuration = timeToPeak + timeToFall;
        float jumpSpeed = Mathf.Sqrt(2f * wallRunGravity * arcHeight);
        float gravity = -(2f * arcHeight) / Mathf.Pow(jumpDuration / 2f, 2f);
        float jumpVelocity = Mathf.Abs(gravity) * (jumpDuration / 2f);

        // Apply the wall running movement
        Vector3 velocity = movement * xInput * wallRunSpeed + Vector3.up * jumpVelocity;
        velocity += wallNormal * verticalSpeed;
        characterController.Move(velocity * Time.deltaTime);
        if (!audioSource.isPlaying)
            audioSource.PlayOneShot(wallRunClip);
    }
}