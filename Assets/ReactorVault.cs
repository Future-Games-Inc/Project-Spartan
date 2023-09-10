using UnityEngine;
using UnityEngine.UI;

public class ReactorVault : MonoBehaviour
{
    public float startY;
    public float endY;
    public float holdDuration;
    public float moveSpeed = 1f; // Speed of movement

    public bool isHolding;

    public bool isMovingUp;

    public GameObject keycardObject;

    public bool activated = false;
    public bool hasBeenActivated = false; // Added variable to track if the vault has been activated before

    public Material activatedMaterial;
    public Material deactivatedMaterial;
    public MatchEffects matchProps;

    public float elapsedTime;
    public float activationTime = 60;
    public float radius = 4;
    public Slider activationSlider;

    private void Start()
    {
        activationSlider.maxValue = activationTime;
        activationSlider.value = activationTime;
        startY = transform.position.y;
        endY = startY + 2.5f; // Adjust the end position as per your requirement
        keycardObject.GetComponent<MeshRenderer>().material = deactivatedMaterial;
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
                    Destroy(gameObject);
                }
            }
        }
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