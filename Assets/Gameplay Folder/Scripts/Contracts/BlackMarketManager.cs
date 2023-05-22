using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class BlackMarketManager : MonoBehaviour
{
    public ContractData contractData; // Reference to the ContractData scriptable object

    public GameObject blackMarketUI; // UI panel for the black market area
    public GameObject contractButtonPrefab; // Prefab for the contract UI button
    public Transform contractButtonParent; // Parent transform to hold the contract UI buttons

    private List<Contract> acceptedContracts = new List<Contract>(); // List to store accepted contracts
    private List<Button> contractButtons = new List<Button>(); // List to store contract UI buttons

    // Called when the player interacts with the black market area
    //public void OpenBlackMarket()
    //{
    //    blackMarketUI.SetActive(true);

    //    // Display available contracts in the UI
    //    DisplayAvailableContracts();
    //}

    private void Start()
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
        Button contractButton = contractButtons[contractIndex];
        contractButton.interactable = false;

        // Update UI or provide visual feedback for the accepted contract
        contractButton.image.color = Color.green; // Highlight the accepted contract button

        Debug.Log("Contract accepted: " + acceptedContract.description);
    }

    // Display available contracts in the UI for the player to accept
    private void DisplayAvailableContracts()
    {
        // Clear existing contract buttons
        foreach (Button button in contractButtons)
        {
            Destroy(button.gameObject);
        }
        contractButtons.Clear();

        // Instantiate contract UI buttons for each available contract
        for (int i = 0; i < contractData.contracts.Length; i++)
        {
            Contract contract = contractData.contracts[i];

            // Instantiate the contract button from the prefab and set its parent
            Button contractButton = Instantiate(contractButtonPrefab, contractButtonParent).GetComponent<Button>();

            // Update UI elements for each contract
            contractButton.interactable = !acceptedContracts.Contains(contract); // Enable button if not already accepted

            // Set button click event to accept the contract
            int index = i; // To capture the correct contract index in the lambda expression
            contractButton.onClick.AddListener(() => AcceptContract(index));

            // Update UI text for contract description
            TextMeshProUGUI contractDescriptionText = contractButton.GetComponentInChildren<TextMeshProUGUI>();
            contractDescriptionText.text = contract.description;

            // Add the contract button to the list
            contractButtons.Add(contractButton);
        }
    }
}