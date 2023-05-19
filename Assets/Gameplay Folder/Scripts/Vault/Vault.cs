using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Vault : MonoBehaviourPunCallbacks
{
    public float startY;
    public float endY;
    public float duration;
    public float holdDuration;

    private float timer;

    private bool isHolding = false;
    private float holdTimer = 0f;

    private bool isMovingBack = false;

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
                photonView.RPC("RPC_VaultOpenMaterial", RpcTarget.All);
                if (!isHolding)
                {
                    // Increment the timer
                    timer += Time.deltaTime;

                    // Calculate the position based on the elapsed time
                    float t = timer / duration;
                    float y = Mathf.Lerp(startY, endY, t);

                    // Call MoveVault RPC to synchronize position across the network
                    photonView.RPC("MoveVault", RpcTarget.All, y);

                    // Start holding if endY is reached
                    if (y >= endY)
                    {
                        isHolding = true;
                    }
                }
                else if (isMovingBack)
                {
                    // Increment the timer
                    timer += Time.deltaTime;

                    // Calculate the position based on the elapsed time
                    float t = timer / duration;
                    float y = Mathf.Lerp(endY, startY, t);

                    // Call MoveVault RPC to synchronize position across the network
                    photonView.RPC("MoveVault", RpcTarget.All, y);

                    // Stop moving if startY is reached
                    if (y <= startY)
                    {
                        transform.position = new Vector3(transform.position.x, startY, transform.position.z);
                        isMovingBack = false;
                        isHolding = false;
                        timer = 0;
                        photonView.RPC("RPC_VaultClosedMaterial", RpcTarget.All);
                        enabled = false;
                    }
                }
                else
                {
                    // Increment the hold timer
                    holdTimer += Time.deltaTime;

                    // Check if hold duration has elapsed
                    if (holdTimer >= holdDuration)
                    {
                        // Start moving back to endY
                        isMovingBack = true;
                        timer = 0;
                        holdTimer = 0;
                    }
                }
            }
        }
    }

    [PunRPC]
    void RPC_UpdateVaultActivation(bool playerWithinRadius)
    {
        if (!activated)
        {
            elapsedTime += Time.deltaTime;
            float remainingTime = activationTime - elapsedTime;
            activationSlider.value = remainingTime;
            if (elapsedTime >= activationTime)
            {
                activated = true;
                activationSlider.gameObject.SetActive(false);
            }
        }
        else if (!playerWithinRadius)
        {
            // Stop the vault if player moves away outside the radius
            isHolding = false;
            isMovingBack = false;
        }
    }

    [PunRPC]
    void RPC_VaultOpenMaterial()
    {
        keycardObject.GetComponent<MeshRenderer>().material = activatedMaterial;
    }

    [PunRPC]
    void RPC_VaultClosedMaterial()
    {
        keycardObject.GetComponent<MeshRenderer>().material = lootedMaterial;
    }

    [PunRPC]
    void MoveVault(float y)
    {
        // Move the game object on the Y-axis
        transform.position = new Vector3(transform.position.x, y, transform.position.z);

        // Synchronize activation slider position
        activationSlider.gameObject.SetActive(true);
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