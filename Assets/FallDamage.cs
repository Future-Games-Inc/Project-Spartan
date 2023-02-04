using UnityEngine;

public class FallDamage : MonoBehaviour
{
    public float fallDamageThreshold = 10f; // The fall distance at which damage is applied
    public float damageAmount = 25f; // The amount of damage to apply

    private CharacterController characterController;
    public PlayerHealth playerHealth;
    private float fallDistance;
    private bool isFalling;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // Check if the player is currently falling
        if (characterController.velocity.y < 0 && !characterController.isGrounded)
        {
            isFalling = true;
            fallDistance += Time.deltaTime * Physics.gravity.y;
        }
        else
        {
            if (isFalling)
            {
                // Check if the player is grabbing something to break their fall
                if (fallDistance > fallDamageThreshold)
                {
                    ApplyDamage();
                }

                fallDistance = 0f;
                isFalling = false;                
            }
        }
    }

    private void ApplyDamage()
    {
        // Reduce the player's health
        playerHealth.TakeDamage(20);
    }
}
