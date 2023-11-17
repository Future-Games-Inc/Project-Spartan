using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Collections;
using UnityEngine.EventSystems;

public class BlackMarketManager : MonoBehaviour
{
    public ContractData contractData; // Reference to the ContractData scriptable object

    public GameObject blackMarketUI; // UI panel for the black market area
    public GameObject contractButtonPrefab; // Prefab for the contract UI button
    public Transform contractButtonParent; // Parent transform to hold the contract UI buttons

    public List<Contract> acceptedContracts = new List<Contract>(); // List to store accepted contracts
    public List<Button> contractButtons = new List<Button>(); // List to store contract UI buttons
    public List<int> filteredContractIndices = new List<int>();

    public TopReactsLeaderboard leaderboard;

    public float radius;
    public bool active;
    public bool screenActivated = false;
    public GameObject screen1;
    public GameObject activeEffect;
    public GameObject selectedButton;

    public GameObject contractWindow;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI expText;
    public TextMeshProUGUI cintText;

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
    }

    // Called when the player accepts a contract
    public void AcceptContract(int filteredContractIndex)
    {
        if (acceptedContracts.Count >= 3)
        {
            return;
        }

        int contractIndex = contractButtons[filteredContractIndex].GetComponent<ContractButton>().ID; // Get the actual contract index
        Contract acceptedContract = contractData.contracts[contractIndex];
        acceptedContracts.Add(acceptedContract);
        string contractType = acceptedContract.goal.ContractType.ToString();


        if (!acceptedContract.isCompleted)
        {
            if (contractType == "BossEnemy")
            {
                if (!PlayerPrefs.HasKey("BossQuest"))
                {
                    PlayerPrefs.SetInt("BossQuest", 1);
                    PlayerPrefs.SetInt("BossQuestTarget", (int)acceptedContract.goal.requriedAmount);
                    PlayerPrefs.SetInt("BossQuestCintTarget", (int)acceptedContract.cintRewards);
                    PlayerPrefs.SetInt("BossQuestExpTarget", (int)acceptedContract.expRewards);
                    PlayerPrefs.SetInt("BossQuestCompleted", 0);
                }
            }
            else if (contractType == "Artifact")
            {
                if (!PlayerPrefs.HasKey("ArtifactQuest"))
                {
                    PlayerPrefs.SetInt("ArtifactQuest", 1);
                    PlayerPrefs.SetInt("ArtifactQuestTarget", (int)acceptedContract.goal.requriedAmount);
                    PlayerPrefs.SetInt("ArtifactQuestCintTarget", (int)acceptedContract.cintRewards);
                    PlayerPrefs.SetInt("ArtifactQuestExpTarget", (int)acceptedContract.expRewards);
                    PlayerPrefs.SetInt("ArtifactQuestCompleted", 0);
                }
            }
            else if (contractType == "Bombs")
            {
                if (!PlayerPrefs.HasKey("BombQuest"))
                {
                    PlayerPrefs.SetInt("BombQuest", 1);
                    PlayerPrefs.SetInt("BombQuestTarget", (int)acceptedContract.goal.requriedAmount);
                    PlayerPrefs.SetInt("BombQuestCintTarget", (int)acceptedContract.cintRewards);
                    PlayerPrefs.SetInt("BombQuestExpTarget", (int)acceptedContract.expRewards);
                    PlayerPrefs.SetInt("BombQuestCompleted", 0);
                }
            }
            else if (contractType == "Guardian")
            {
                if (!PlayerPrefs.HasKey("GuardianQuest"))
                {
                    PlayerPrefs.SetInt("GuardianQuest", 1);
                    PlayerPrefs.SetInt("GuardianQuestTarget", (int)acceptedContract.goal.requriedAmount);
                    PlayerPrefs.SetInt("GuardianQuestCintTarget", (int)acceptedContract.cintRewards);
                    PlayerPrefs.SetInt("GuardianQuestExpTarget", (int)acceptedContract.expRewards);
                    PlayerPrefs.SetInt("GuardianQuestCompleted", 0);
                }
            }
            else if (contractType == "Intel")
            {
                if (!PlayerPrefs.HasKey("IntelQuest"))
                {
                    PlayerPrefs.SetInt("IntelQuest", 1);
                    PlayerPrefs.SetInt("IntelQuestTarget", (int)acceptedContract.goal.requriedAmount);
                    PlayerPrefs.SetInt("IntelQuestCintTarget", (int)acceptedContract.cintRewards);
                    PlayerPrefs.SetInt("IntelQuestExpTarget", (int)acceptedContract.expRewards);
                    PlayerPrefs.SetInt("IntelQuestCompleted", 0);
                }
            }
            else if (contractType == "Collector")
            {
                if (!PlayerPrefs.HasKey("CollectorQuest"))
                {
                    PlayerPrefs.SetInt("CollectorQuest", 1);
                    PlayerPrefs.SetInt("CollectorQuestTarget", (int)acceptedContract.goal.requriedAmount);
                    PlayerPrefs.SetInt("CollectorQuestCintTarget", (int)acceptedContract.cintRewards);
                    PlayerPrefs.SetInt("CollectorQuestExpTarget", (int)acceptedContract.expRewards);
                    PlayerPrefs.SetInt("CollectorQuestCompleted", 0);
                }
            }
        }

        // Disable the accept button for this contract
        Button contractButton = contractButtons[filteredContractIndex];
        contractButton.interactable = false;

        // Update UI or provide visual feedback for the accepted contract
        contractButton.image.color = Color.green; // Highlight the accepted contract button
    }

    public bool CheckForPlayerWithinRadius()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    // Display available contracts in the UI for the player to accept
    public IEnumerator DisplayAvailableContracts()
    {
        yield return new WaitForSeconds(.75f);
        active = true;
        if (CheckForPlayerWithinRadius())
            // Clear existing contract buttons
            foreach (Button button in contractButtons)
            {
                Destroy(button.gameObject);
            }
        contractButtons.Clear();
        filteredContractIndices.Clear(); // Clear the filtered contract indices list

        // Instantiate contract UI buttons for each available contract
        for (int i = 1; i < contractData.contracts.Length; i++)
        {
            Contract contract = contractData.contracts[i];

            if (leaderboard.currentLevelInt >= contract.requiredLevel && !contract.isCompleted)
            {
                // Instantiate the contract button from the prefab and set its parent
                Button contractButton = Instantiate(contractButtonPrefab, contractButtonParent).GetComponent<Button>();
                contractButton.GetComponent<ContractButton>().ID = contract.ID;

                // Add EventTrigger component to handle pointer enter event
                EventTrigger trigger = contractButton.gameObject.AddComponent<EventTrigger>();

                // Create the entry for the pointer enter event
                EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry();
                pointerEnterEntry.eventID = EventTriggerType.PointerEnter;
                pointerEnterEntry.callback.AddListener((data) => OnPointerEnterContractButton((PointerEventData)data));

                // Add the pointer enter event to the EventTrigger
                trigger.triggers.Add(pointerEnterEntry);

                // Update UI elements for each contract
                contractButton.interactable = !acceptedContracts.Contains(contract); // Enable button if not already accepted

                // Set button click event to accept the contract
                int contractIndex = contractButtons.Count; // Capture the current value of 'i'
                contractButton.onClick.AddListener(() => AcceptContract(contractIndex));

                // Update UI text for contract description
                TextMeshProUGUI contractDescriptionText = contractButton.GetComponentInChildren<TextMeshProUGUI>();
                contractDescriptionText.text = contract.description;

                // Add the contract button to the list
                contractButtons.Add(contractButton);

                if (contract.goal.ContractType.ToString() == "BossEnemy")
                {
                    if (PlayerPrefs.HasKey("BossQuest") && PlayerPrefs.GetInt("BossQuestCompleted") == 0)
                    {
                        contract.isActive = true;
                        acceptedContracts.Add(contract);

                        Button contractButtonMain = contractButtons[contractIndex];
                        contractButtonMain.interactable = false;

                        // Update UI or provide visual feedback for the accepted contract
                        contractButton.image.color = Color.green; // Highlight the accepted contract button
                    }
                }

                else if (contract.goal.ContractType.ToString() == "Artifact")
                {
                    if (PlayerPrefs.HasKey("ArtifactQuest") && PlayerPrefs.GetInt("ArtifactQuestCompleted") == 0)
                    {
                        contract.isActive = true;
                        acceptedContracts.Add(contract);

                        Button contractButtonMain = contractButtons[contractIndex];
                        contractButtonMain.interactable = false;

                        // Update UI or provide visual feedback for the accepted contract
                        contractButton.image.color = Color.green; // Highlight the accepted contract button
                    }
                }

                else if (contract.goal.ContractType.ToString() == "Bombs")
                {
                    if (PlayerPrefs.HasKey("BombQuest") && PlayerPrefs.GetInt("BombQuestCompleted") == 0)
                    {
                        contract.isActive = true;
                        acceptedContracts.Add(contract);

                        Button contractButtonMain = contractButtons[contractIndex];
                        contractButtonMain.interactable = false;

                        // Update UI or provide visual feedback for the accepted contract
                        contractButton.image.color = Color.green; // Highlight the accepted contract button
                    }
                }

                else if (contract.goal.ContractType.ToString() == "Guardian")
                {
                    if (PlayerPrefs.HasKey("GuardianQuest") && PlayerPrefs.GetInt("GuardianQuestCompleted") == 0)
                    {
                        contract.isActive = true;
                        acceptedContracts.Add(contract);

                        Button contractButtonMain = contractButtons[contractIndex];
                        contractButtonMain.interactable = false;

                        // Update UI or provide visual feedback for the accepted contract
                        contractButton.image.color = Color.green; // Highlight the accepted contract button
                    }
                }

                else if (contract.goal.ContractType.ToString() == "Intel")
                {
                    if (PlayerPrefs.HasKey("IntelQuest") && PlayerPrefs.GetInt("IntelQuestCompleted") == 0)
                    {
                        contract.isActive = true;
                        acceptedContracts.Add(contract);

                        Button contractButtonMain = contractButtons[contractIndex];
                        contractButtonMain.interactable = false;

                        // Update UI or provide visual feedback for the accepted contract
                        contractButton.image.color = Color.green; // Highlight the accepted contract button
                    }
                }

                else if (contract.goal.ContractType.ToString() == "Collector")
                {
                    if (PlayerPrefs.HasKey("CollectorQuest") && PlayerPrefs.GetInt("CollectorQuestCompleted") == 0)
                    {
                        contract.isActive = true;
                        acceptedContracts.Add(contract);

                        Button contractButtonMain = contractButtons[contractIndex];
                        contractButtonMain.interactable = false;

                        // Update UI or provide visual feedback for the accepted contract
                        contractButton.image.color = Color.green; // Highlight the accepted contract button
                    }
                }

                // Add the index to the filteredContractIndices list
                filteredContractIndices.Add(contractButtons.Count);
            }
        }
    }

    private void Update()
    {
        activeEffect.SetActive(active);
        contractWindow.SetActive(screenActivated);

        if (active && CheckForPlayerWithinRadius())
            screen1.SetActive(true);
        else
            screen1.SetActive(false);

        if (!CheckForPlayerWithinRadius())
            screenActivated = false;
    }

    public void OpenContractWindow()
    {
        if (CheckForPlayerWithinRadius())
        {
            screenActivated = true;
            int contractIndex = selectedButton.GetComponent<ContractButton>().ID;
            Contract contract = contractData.contracts[contractIndex];

            titleText.text = contract.description.ToString();
            descriptionText.text = contract.objectives.ToString();
            expText.text = "EXP Rewards: " + contract.expRewards.ToString();
            cintText.text = "Cint Reward: " + contract.cintRewards.ToString();
        }
    }

    public void OnPointerEnterContractButton(PointerEventData eventData)
    {
        selectedButton = eventData.pointerEnter.transform.parent.gameObject;
        OpenContractWindow();
    }
}