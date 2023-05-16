using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocalSupplyCrate : MonoBehaviour
{
    public GameObject[] weaponPrefabs;
    public Slider activationSlider;
    public GameObject[] effects;
    public TextMeshProUGUI openText;
    public Image sliderImage;
    public AudioSource audioSource;
    public AudioClip audioClip;

    public float elapsedTime;
    public float activationTime = 15;
    public float radius = 4;

    public bool isActive;
    public bool contact = false;
    public bool playAudio = true;

    private void Start()
    {
        StartCoroutine(PlayAudioLoop());
        activationSlider.maxValue = activationTime;
        activationSlider.value = activationTime;
        activationSlider.gameObject.SetActive(true);
    }

    IEnumerator PlayAudioLoop()
    {
        while (playAudio)
        {
            if (CheckForPlayerWithinRadius() && !isActive)
            {
                audioSource.PlayOneShot(audioClip);
                yield return new WaitForSeconds(audioClip.length);
            }
            yield return new WaitForSeconds(.75f);
        }
    }

    public void StopAudio()
    {
        playAudio = false;
    }

    void Update()
    {
        if (CheckForPlayerWithinRadius())
        {
            if (!isActive)
            {
                elapsedTime += Time.deltaTime;
                float remainingTime = activationTime - elapsedTime;
                activationSlider.value = remainingTime;
                if (elapsedTime >= activationTime)
                {
                    isActive = true;
                    StopAudio();
                    activationSlider.gameObject.SetActive(false);
                    openText.gameObject.SetActive(true);
                }

                foreach (GameObject vfx in effects)
                {
                    vfx.SetActive(false);
                }
            }
        }
        else
        {
            foreach (GameObject vfx in effects)
            {
                vfx.SetActive(true);
            }
        }

        if (activationSlider.value <= (activationTime * 0.75) && activationSlider.value > (activationTime * 0.25))
            sliderImage.color = Color.yellow;
        if (activationSlider.value <= (activationTime * 0.25))
            sliderImage.color = Color.red;
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

    void InstantiateWeapons()
    {
        int count = Random.Range(0, weaponPrefabs.Length + 1);
        for (int i = 0; i < count; i++)
        {
            Vector3 randomPosition = transform.position + Random.onUnitSphere * (radius / 2);
            GameObject weaponPrefab = weaponPrefabs[Random.Range(0, weaponPrefabs.Length)];
            Instantiate(weaponPrefab, randomPosition, Quaternion.identity);
            Rigidbody rb = weaponPrefab.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LeftHand") || other.CompareTag("RightHand") || other.CompareTag("Player") && isActive == true)
        {
            InstantiateWeapons();
            StartCoroutine(Destroy());
            contact = true;
        }
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(.25f);
        Destroy(gameObject);
    }
}
