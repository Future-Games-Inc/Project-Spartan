using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Vault : MonoBehaviour
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

    private float elapsedTime;
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

                // Calculate the rotation amount based on the elapsed time
                float t = timer / duration;
                float y = Mathf.Lerp(startY, endY, t);

                // Rotate the game object
                transform.rotation = Quaternion.Euler(0, y, 0);

                // Start holding if endY is reached
                if (y >= endY)
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

                // Rotate the game object
                transform.rotation = Quaternion.Euler(0, y, 0);

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
    bool CheckForPlayerWithinRadius()
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
