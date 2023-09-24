using UnityEngine;

public class CuaTerminal : MonoBehaviour
{
    public GameObject logo;
    public GameObject button;
    public GameObject panel;
    public GameObject[] holograms;

    public PlayerHealth playerHealth;
    public float radius = 2f;
    public string terminalName;
    public GameObject activatedIcon;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        activatedIcon.SetActive(HasTerminalAccess());
       
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
                PlayerHealth health = collider.GetComponentInParent<PlayerHealth>();
                if (health != null)
                {
                    playerHealth = health;
                    if (!HasTerminalAccess())
                    {
                        SaveTerminalAccess(15);
                    }
                }
            }
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
}
