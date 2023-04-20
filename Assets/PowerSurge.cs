using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class PowerSurge : MonoBehaviour
{
    public float radius = 2f;
    public float lightRadius = 20f;

    public Light[] lightToFlicker;

    public float minIntensity = 0.25f;
    public float maxIntensity = 1f;
    public float flickerSpeed = .05f;

    private float targetIntensity;

    public bool activated;
    public bool canBeActivated;

    void Start()
    {
        activated = false;
        canBeActivated = true;
        StartCoroutine(Flicker());

        // Find all game objects with the specified tag within the radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, lightRadius);
        GameObject[] objectsWithTag = new GameObject[colliders.Length];

        int i = 0;
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.CompareTag("HackLights"))
            {
                objectsWithTag[i] = collider.gameObject;
                i++;
            }
        }

        int totalLights = 0;
        foreach (GameObject obj in objectsWithTag)
        {
            if (obj != null)
            {
                totalLights += obj.GetComponentsInChildren<Light>().Length;
            }
        }

        lightToFlicker = new Light[totalLights];

        int lightIndex = 0;
        foreach (GameObject obj in objectsWithTag)
        {
            if (obj != null)
            {
                Light[] lights = obj.GetComponentsInChildren<Light>();
                foreach (Light light in lights)
                {
                    lightToFlicker[lightIndex] = light;
                    lightIndex++;
                }
            }
        }
    }

    void Update()
    {
        if (CheckForPlayerWithinRadius() == true)
        {
            activated = true;
        }

        if (activated && canBeActivated)
        {
            StartCoroutine(BlackOut());
        }
    }

    IEnumerator BlackOut()
    {
        yield return new WaitForSeconds(3);
        canBeActivated = false;
        activated = false;
        foreach (Light light in lightToFlicker)
        {
            light.intensity = 0;
        }
        yield return new WaitForSeconds(10);
        canBeActivated = true;
        foreach (Light light in lightToFlicker)
        {
            light.intensity = maxIntensity;
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

    IEnumerator Flicker()
    {
        while (true)
        {
            while (!activated)
            {
                // Wait until activated is true
                yield return null;
            }
            // Calculate a random target intensity for each light
            foreach (Light light in lightToFlicker)
            {
                float randomIntensity = Random.Range(0.0f, 1.0f);
                float targetIntensity = Mathf.Lerp(minIntensity, maxIntensity, randomIntensity);
                light.intensity = targetIntensity;
            }

            // Wait for a short time before flickering again
            yield return new WaitForSeconds(flickerSpeed);
        }
    }
}
