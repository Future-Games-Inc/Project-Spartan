using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class CuaTerminal : MonoBehaviourPunCallbacks
{
    public GameObject logo;
    public GameObject button;
    public GameObject panel;
    public GameObject[] holograms;

    public PlayerHealth playerHealth;
    public float radius = 2f;
    public string terminalName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
                logo.SetActive(true);
                button.SetActive(true);
                panel.SetActive(true);
                foreach(GameObject hologram in holograms)
                {
                    hologram.SetActive(false);
                }
        }
    }

    public void Activate()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("Player"))
            {
                PlayerHealth health = collider.GetComponent<PlayerHealth>();
                if (health != null)
                {
                    playerHealth = health;
                    if (!HasTerminalAccess(photonView.Owner))
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
}
