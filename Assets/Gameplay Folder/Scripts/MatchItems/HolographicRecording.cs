using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class HolographicRecording : MonoBehaviourPunCallbacks
{
    public GameObject[] hologramObject;
    public GameObject[] button;
    public AudioSource voiceoverAudio;

    private bool isPlaying;

    public float radius = 2f;
    public PlayerHealth playerHealth;

    public string terminalName;

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

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("Player"))
            {
                PlayerHealth health = collider.GetComponent<PlayerHealth>();
                if(health != null)
                {
                    playerHealth = health;
                    if(!HasTerminalAccess(photonView.Owner))
                    {
                        SaveTerminalAccess(photonView.Owner, 75);
                    }
                }
            }
        }
    }

    public bool HasTerminalAccess(Player player)
    {
        // Check if the player has accessed the terminal before
        object terminalAccess;
        if (player.CustomProperties.TryGetValue(terminalName, out terminalAccess))
        {
            return (bool)terminalAccess;
        }
        return false;
    }

    public void SaveTerminalAccess(Player player, int XP)
    {
        // Save terminal access in player's custom properties
        player.SetCustomProperties(new Hashtable() { { terminalName, true } });
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
