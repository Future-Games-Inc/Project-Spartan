using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSurge : MonoBehaviour
{
    [Header("Power Surge Effect -------------------------------------------------------")]
    public float radius = 2f;
    public float lightRadius = 20f;

    public float minIntensity = 0.25f;
    public float maxIntensity = 1f;
    public float flickerSpeed = .05f;

    [Header("Power Surge Setup --------------------------------------------------------")]
    private Dictionary<Light, int> lightIndexDict;

    [SerializeField]
    bool activated = false;
    public bool canBeActivated;
    public Light[] lightToFlicker;
    public GameObject surgeBubble;
    public Transform bubbleScale;

    private Vector3 initialScale;
    private Coroutine bubbleScaleCoroutine;
    public MatchEffects matchProps;

    public static readonly byte[] memLight = new byte[4] { 0, 0, 0, 0 };

    public static object DeserializeLight(byte[] bytes)
    {
        float intensity = BitConverter.ToSingle(bytes, 0);
        return new Light() { intensity = intensity };
    }

    public static byte[] Serialize(object obj)
    {
        Light light = (Light)obj;
        float intensity = light.intensity;
        byte[] intensityBytes = BitConverter.GetBytes(intensity);
        return intensityBytes;
    }

    void OnEnable()
    {
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
        lightIndexDict = new Dictionary<Light, int>();

        int lightIndex = 0;
        foreach (GameObject obj in objectsWithTag)
        {
            if (obj != null)
            {
                Light[] lights = obj.GetComponentsInChildren<Light>();
                foreach (Light light in lights)
                {
                    lightToFlicker[lightIndex] = light;
                    lightIndexDict.Add(light, lightIndex);
                    lightIndex++;
                }
            }
        }

        initialScale = surgeBubble.transform.localScale;
    }

    public void ActivateSurge()
    {
        if (!activated && canBeActivated && matchProps.startMatchBool)
        {
            activated = true;
            bubbleScaleCoroutine = StartCoroutine(GrowBubbleCoroutine());
        }
    }

    void Update()
    {
        if (activated && canBeActivated)
        {
            StartCoroutine(BlackOut());
        }
    }

    IEnumerator BlackOut()
    {
        canBeActivated = false;
        yield return new WaitForSeconds(3);
        activated = false;
        foreach (Light light in lightToFlicker)
        {
            light.intensity = 0;
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, lightRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Security"))
            {
                DroneHealth enemyDamageCrit = collider.GetComponent<DroneHealth>();
                if (enemyDamageCrit != null)
                    enemyDamageCrit.TakeDamage(200);
                else
                {
                    SentryDrone enemyDamageCrit2 = collider.GetComponent<SentryDrone>();
                    if (enemyDamageCrit2 != null)
                        enemyDamageCrit.TakeDamage(200);
                }
            }

            if (collider.CompareTag("Enemy") || collider.CompareTag("BossEnemy"))
            {
                FollowAI enemyDamageCrit = collider.GetComponent<FollowAI>();
                if (enemyDamageCrit != null)
                {
                    enemyDamageCrit.TakeDamage(50);
                    enemyDamageCrit.EMPShock();
                }
            }
        }
        yield return new WaitForSeconds(10);
        canBeActivated = true;
        foreach (Light light in lightToFlicker)
        {
            light.intensity = maxIntensity;
        }

        surgeBubble.transform.localScale = initialScale;
        surgeBubble.SetActive(false);
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

            surgeBubble.SetActive(true);

            // Calculate a random target intensity for each light
            foreach (Light light in lightToFlicker)
            {
                float randomIntensity = UnityEngine.Random.Range(0.0f, 1.0f);
                float targetIntensity = Mathf.Lerp(minIntensity, maxIntensity, randomIntensity);
                int lightIndex = lightIndexDict[light];
                light.intensity = targetIntensity;
            }

            // Wait for a short time before flickering again
            yield return new WaitForSeconds(flickerSpeed);
        }
    }

    IEnumerator GrowBubbleCoroutine()
    {
        while (activated)
        {
            float scaleIncrease = 8f * Time.deltaTime;
            surgeBubble.transform.localScale += new Vector3(scaleIncrease, scaleIncrease, scaleIncrease);

            yield return null;
        }
    }
}