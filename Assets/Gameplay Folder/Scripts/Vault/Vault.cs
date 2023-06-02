using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Vault : MonoBehaviourPunCallbacks
{
    public float startY;
    public float endY;
    public float duration;
    public float holdDuration;
    public float moveSpeed = 1f; // Speed of movement

    private float timer;

    private bool isHolding = false;
    private float holdTimer = 0f;

    private bool isMovingUp;

    public GameObject keycardObject;

    public bool activated;

    public Material activatedMaterial;
    public Material deactivatedMaterial;
    public Material lootedMaterial;

    public float elapsedTime;
    public float activationTime = 15;
    public float radius = 4;
    public Slider activationSlider;

    private void Start()
    {
        activationSlider.maxValue = activationTime;
        activationSlider.value = activationTime;
        startY = transform.position.y;
        endY = startY + 4f; // Adjust the end position as per your requirement
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            // Check for player within radius and synchronize activation timer and slider value
            bool playerWithinRadius = CheckForPlayerWithinRadius();
            photonView.RPC("RPC_UpdateVaultActivation", RpcTarget.All, playerWithinRadius);

            if (activated)
            {
                photonView.RPC("RPC_VaultMaterial", RpcTarget.All, activatedMaterial);
                if (isMovingUp)
                {
                    // Move the object up
                    transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

                    // Check if the object has reached the end position
                    if (transform.position.y >= endY)
                    {
                        isMovingUp = false;
                        isHolding = true;
                        holdTimer = 0f;
                    }
                }
                else if (isHolding)
                {
                    // Increment the hold timer
                    holdTimer += Time.deltaTime;

                    // Check if hold duration has elapsed
                    if (holdTimer >= holdDuration)
                    {
                        isHolding = false;
                        isMovingUp = false;
                        holdTimer = 0f;
                    }
                }
                else
                {
                    photonView.RPC("RPC_VaultMaterial", RpcTarget.All, lootedMaterial);
                    // Move the object down
                    transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);

                    // Check if the object has reached the start position
                    if (transform.position.y <= startY)
                    {
                        isMovingUp = true;
                        isHolding = false;
                        holdTimer = 0f;
                    }

                }
            }
        }
    }

    [PunRPC]
    void RPC_UpdateVaultActivation(bool playerWithinRadius)
    {
        if (!activated && playerWithinRadius)
        {
            elapsedTime += Time.deltaTime;
            float remainingTime = activationTime - elapsedTime;
            activationSlider.value = remainingTime;
            if (elapsedTime >= activationTime)
            {
                activated = true;
                isMovingUp = true;
                activationSlider.gameObject.SetActive(false);
            }
        }
        else if (!playerWithinRadius)
        {
            // Stop the vault if player moves away outside the radius
            isHolding = false;
            isMovingUp = false;
        }
    }

    [PunRPC]
    void RPC_VaultMaterial(Material material)
    {
        keycardObject.GetComponent<MeshRenderer>().material = material;
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