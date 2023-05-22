using UnityEngine;
using UnityEngine.UI;

public class TerminalManager : MonoBehaviour
{
    public ContractData contractData; // Reference to the ContractData scriptable object
    public GameObject terminalUI; // UI panel for the terminal
    public Button pickupButton; // Button to pick up a contract
    public Text contractDescriptionText; // Text displaying contract description

    private Contract terminalContract; // Reference to the contract picked up from the terminal

    // Called when the player interacts with the terminal
    public void OpenTerminal()
    {
        terminalUI.SetActive(true);

        // Display available contract in the UI
        DisplayAvailableContract();
    }

    // Called when the player picks up a contract from the terminal
    public void PickupContract()
    {
        if (terminalContract != null)
        {
            
            return;
        }

        terminalContract = GetRandomTerminalContract();
        pickupButton.interactable = false;

        // Update UI or provide visual feedback for the picked up contract
        
    }

    // Display an available contract in the UI for the player to pick up
    private void DisplayAvailableContract()
    {
        pickupButton.interactable = true;
        contractDescriptionText.text = GetRandomTerminalContract().description;
    }

    // Retrieve a random terminal contract from the ContractData scriptable object
    private Contract GetRandomTerminalContract()
    {
        int randomIndex = Random.Range(0, contractData.contracts.Length);
        return contractData.contracts[randomIndex];
    }
}