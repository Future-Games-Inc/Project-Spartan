using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTrigger2 : MonoBehaviour
{

    public GameObject MainMenu;
    public GameObject bannerCanvas;
    public GameObject connectionLost;

    public bool activated;

    public AudioSource audioSource;
    public AudioClip[] audioClip;

    // Start is called before the first frame update
    void Start()
    {
        activated = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (MainMenu != null)
            MainMenu.SetActive(activated);
        bannerCanvas.SetActive(!activated);
        if (connectionLost != null)
            connectionLost.SetActive(activated);

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activated = true;
            ////if (!audioSource.isPlaying)
            ////    audioSource.PlayOneShot(audioClip[Random.Range(0, audioClip.Length)]);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        activated = false;
    }
}