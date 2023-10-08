using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SupplyDropCrate : MonoBehaviour
{
    public GameObject weaponPrefabs;
    public Slider activationSlider;
    public GameObject[] effects;
    public Image sliderImage;
    public AudioSource audioSource;
    public AudioClip audioClip;
    public AudioClip spawnClip;

    public Animator animator;
    public string animationName;

    public Transform spawn1;

    private float elapsedTime;
    public float activationTime = 15;
    public float radius = 4;

    private bool isActive;
    public bool playAudio = true;

    public float forceMagnitude;
    public MatchEffects matchEffects;


    private void OnEnable()
    {
        matchEffects = FindObjectOfType<MatchEffects>();
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

    void Update()
    {
        if (CheckForPlayerWithinRadius() == true)
        {
            if (!isActive)
            {
                animator.enabled = true;
                elapsedTime += Time.deltaTime;
                float remainingTime = activationTime - elapsedTime;
                activationSlider.value = remainingTime;

                // Calculate the progress of the activation based on elapsedTime and activationTime.
                float activationProgress = elapsedTime / activationTime;

                // Play the animation according to this progress.
                animator.Play(animationName, 0, activationProgress);

                // Since you're manually controlling the playback progress of the animation, 
                // set the animator speed to 0 so that the animation doesn't play on its own.
                animator.speed = 0;

                if (elapsedTime >= activationTime)
                {
                    InstantiateWeapons();
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
        isActive = true;
        activationSlider.gameObject.SetActive(false);

        Rigidbody rb = this.GetComponent<Rigidbody>();
        rb.AddForce(Vector3.up * forceMagnitude);
        // Shuffle the weaponPrefabs array

        audioSource.PlayOneShot(spawnClip);

        // Instantiate a weapon for each spawn point
        Instantiate(weaponPrefabs, spawn1.position, spawn1.rotation);
        matchEffects.MissionStep2();

        StartCoroutine(Destroy());
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }
}