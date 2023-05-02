using System.Collections;
using System.Collections.Generic;
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

    private bool isRotatingBack = false;

    public GameObject keycardObject;

    public bool activated;

    public Material activatedMaterial;
    public Material decativatedMaterial;
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
            CheckForPlayerWithinRadius();

            if (CheckForPlayerWithinRadius() == true)
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
            }

            if (activated)
            {
                keycardObject.GetComponent<MeshRenderer>().material = activatedMaterial;
                if (!isHolding)
                {
                    // Increment the timer
                    timer += Time.deltaTime;

                    // Call RotateVault RPC to synchronize rotation across the network
                    photonView.RPC("RotateVault", RpcTarget.All, Mathf.Lerp(startY, endY, timer / duration), elapsedTime, activationSlider.value);

                    // Start holding if endY is reached
                    if (Mathf.Lerp(startY, endY, timer / duration) >= endY)
                    {
                        isHolding = true;
                    }
                }
                else if (isRotatingBack)
                {
                    // Increment the timer
                    timer += Time.deltaTime;

                    // Calculate the rotation amount based on the elapsed time
                    float t = timer / duration;
                    float y = Mathf.Lerp(endY, startY, t);

                    // Call RotateVault RPC to synchronize rotation across the network
                    photonView.RPC("RotateVault", RpcTarget.All, Mathf.Lerp(endY, startY, timer / duration), elapsedTime, activationSlider.value);

                    // Stop rotating if startY is reached
                    if (y <= startY)
                    {
                        transform.rotation = Quaternion.Euler(0, startY, 0);
                        isRotatingBack = false;
                        isHolding = false;
                        timer = 0;
                        keycardObject.GetComponent<MeshRenderer>().material = lootedMaterial;
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
                        // Start rotating back to endY
                        isRotatingBack = true;
                        timer = 0;
                        holdTimer = 0;
                    }
                }
            }
        }
    }

    [PunRPC]
    void RotateVault(float y, float elapsedTime, float sliderValue)
    {
        // Rotate the game object
        transform.rotation = Quaternion.Euler(0, y, 0);

        // Synchronize activation timer and slider value
        activated = true;
        this.elapsedTime = elapsedTime;
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
