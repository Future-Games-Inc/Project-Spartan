using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BlackMarketManager : MonoBehaviour
{
    public ContractData contractData; // Reference to the ContractData scriptable object

    public GameObject blackMarketUI; // UI panel for the black market area
    public Button acceptButton; // Button to accept a contract
    public Text contractDescriptionText; // Text displaying contract description

    private List<Contract> acceptedContracts = new List<Contract>(); // List to store accepted contracts
    public Button[] contractButtons;

    // Called when the player interacts with the black market area
    public void OpenBlackMarket()
    {
        blackMarketUI.SetActive(true);

        // Display available contracts in the UI
        DisplayAvailableContracts();
    }

    // Called when the player accepts a contract
    public void AcceptContract(int contractIndex)
    {
        if (acceptedContracts.Count >= 3)
        {
            return;
        }

        Contract acceptedContract = contractData.contracts[contractIndex];
        acceptedContracts.Add(acceptedContract);

        // Disable the accept button for this contract
        Button contractButton = acceptButton.transform.parent.GetComponent<Button>();
        contractButton.interactable = false;

        // Update UI or provide visual feedback for the accepted contract

        Debug.Log("Contract accepted: " + acceptedContract.description);
    }

    // Display available contracts in the UI for the player to accept
    private void DisplayAvailableContracts()
    {
        // Assuming you have UI buttons to represent each available contract
        for (int i = 0; i < contractData.contracts.Length; i++)
        {
            Contract contract = contractData.contracts[i];
            Button contractButton = contractButtons[i]; // Access the corresponding contract button

            // Update UI elements for each contract
            contractButton.interactable = !acceptedContracts.Contains(contract); // Enable button if not already accepted


            // Set button click event to accept the contract
            contractButton.onClick.RemoveAllListeners();
            int index = i; // To capture the correct contract index in the lambda expression
            contractButton.onClick.AddListener(() => AcceptContract(index));

            // Update UI text for contract description
            // ...
        }
    }
}
