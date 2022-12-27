using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnnouncerFX : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] audioClips;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("PlayAnnouncement", 15, Random.Range(15,45));
    }

    public void PlayAnnouncement()
    {
        if (!audioSource.isPlaying)
        audioSource.PlayOneShot(audioClips[Random.Range(0,audioClips.Length)]);
    }
}
