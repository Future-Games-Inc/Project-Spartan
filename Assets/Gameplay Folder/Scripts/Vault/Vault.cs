using UnityEngine;
using UnityEngine.UI;

public class Vault : MonoBehaviour
{
    public float startY;
    public float endY;
    public float holdDuration;
    public float moveSpeed = 1f; // Speed of movement

    private float timer;

    public bool isHolding;
    private float holdTimer = 0f;

    public bool isMovingUp;

    public GameObject keycardObject;

    public bool activated = false;
    public bool hasBeenActivated = false; // Added variable to track if the vault has been activated before

    public Material activatedMaterial;
    public Material deactivatedMaterial;
    public Material lootedMaterial;
    public MatchEffects matchProps;

    public float elapsedTime;
    public float activationTime = 15;
    public float radius = 4;
    public Slider activationSlider;

    private void Start()
    {
        activationSlider.maxValue = activationTime;
        activationSlider.value = activationTime;
        startY = transform.position.y;
        endY = startY + 2.5f; // Adjust the end position as per your requirement
        keycardObject.GetComponent<MeshRenderer>().material = activatedMaterial;
    }

    void Update()
    {
        // Check for player within radius and synchronize activation timer and slider value
        bool playerWithinRadius = CheckForPlayerWithinRadius();
        UpdateVaultActivation(playerWithinRadius);

        if (activated && hasBeenActivated) // Only move if the vault has been activated before
        {

            if (isMovingUp && isHolding)
            {
                keycardObject.GetComponent<MeshRenderer>().material = activatedMaterial;
                radius = 10f;
                // Move the object up
                transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

                // Check if the object has reached the end position
                if (transform.position.y >= endY)
                {
                    isMovingUp = false;
                    holdTimer = 0f;
                }
            }
            else if (isHolding)
            {
                keycardObject.GetComponent<MeshRenderer>().material = deactivatedMaterial;
                radius = 10f;
                // Increment the hold timer
                holdTimer += Time.deltaTime;

                // Check if hold duration has elapsed
                if (holdTimer >= holdDuration)
                {
                    isHolding = false;
                }
            }
            else
            {
                keycardObject.GetComponent<MeshRenderer>().material = lootedMaterial;
                radius = 4f;
                // Move the object down
                transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);

                // Check if the object has reached the start position
                if (transform.position.y <= startY)
                {
                    activated = false;
                    activationSlider.maxValue = activationTime;
                    activationSlider.value = activationTime;
                    elapsedTime = 0f;
                    holdTimer = 0f;
                }
            }
        }

        if(matchProps.spawnReactor)
            Destroy(gameObject);
    }

    void UpdateVaultActivation(bool playerWithinRadius)
    {
        if (!activated && playerWithinRadius && matchProps.startMatchBool)
        {
            elapsedTime += Time.deltaTime;
            float remainingTime = activationTime - elapsedTime;
            activationSlider.value = remainingTime;
            if (elapsedTime >= activationTime)
            {
                isMovingUp = true;
                isHolding = true;
                hasBeenActivated = true; // Update the flag to indicate the vault has been activated
                activationSlider.gameObject.SetActive(false);
                activated = true;
            }
        }
        else if (!playerWithinRadius)
        {
            // Stop the vault if player moves away outside the radius
            isHolding = false;
            isMovingUp = false;
        }
    }

    public bool CheckForPlayerWithinRadius()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }
}