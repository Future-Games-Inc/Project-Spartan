using UnityEngine;

public class HolographicRecording : MonoBehaviour
{
    public GameObject[] hologramObject;
    public GameObject[] button;
    public AudioSource voiceoverAudio;

    private bool isPlaying;

    public float radius = 2f;
    public PlayerHealth playerHealth;

    public string terminalName;
    public GameObject activatedIcon;

    private void Update()
    {
        if (isPlaying)
        {
            // Check if the voiceover has finished playing
            if (!voiceoverAudio.isPlaying)
            {
                // Reset the hologram and stop playing
                StopPlayingRecording();
            }
        }

        activatedIcon.SetActive(HasTerminalAccess());
    }

    public void ActivateRecording()
    {
        // Check if the voiceover is already playing
        if (isPlaying)
        {
            return;
        }

        // Activate the hologram and play the voiceover
        foreach (GameObject hologram in hologramObject)
            hologram.SetActive(true);
        voiceoverAudio.Play();

        isPlaying = true;

        if (!HasTerminalAccess())
        {
            SaveTerminalAccess(15);
        }

    }

    public bool HasTerminalAccess()
    {
        // Check if the player has accessed the terminal before
        if (PlayerPrefs.HasKey(terminalName))
        {
            return true;
        }
        return false;
    }

    public void SaveTerminalAccess(int XP)
    {
        // Save terminal access in player's custom properties
        PlayerPrefs.SetInt(terminalName, 1);
        playerHealth.GetXP(XP);
    }

    public void StopPlayingRecording()
    {
        // Deactivate the hologram and stop the voiceover
        foreach (GameObject hologram in hologramObject)
            hologram.SetActive(false);
        voiceoverAudio.Stop();
        foreach (GameObject text in button)
            text.SetActive(true);

        isPlaying = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isPlaying)
                // Stop playing the recording when the player leaves the trigger radius
                StopPlayingRecording();
        }
    }
}
