using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class AudioFileElement : MonoBehaviour
{
    public Button audioFileButton;
    public AudioSource audioSource;
    public AudioClip audioClip;
    public GameObject textView;

    public string audioFileKey;

    public void Start()
    {
        textView.SetActive(false);
        bool unlocked = PlayerHasUnlockedAudioFile();

        if (audioFileButton != null)
        {
            audioFileButton.interactable = unlocked;
        }
    }

    private bool PlayerHasUnlockedAudioFile()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(audioFileKey, out object audioState))
        {
            // Check the custom property value for the audio file key
            bool unlocked = (bool)audioState;
            return unlocked;
        }

        return false;
    }

    public void PlayAudioFile()
    {
        if (PlayerHasUnlockedAudioFile())
        {
            textView.SetActive(false);
            // Play the audio file
            audioSource.PlayOneShot(audioClip);
        }
    }

    public void StopAudioFile()
    {
        audioSource.Stop();
    }
}
