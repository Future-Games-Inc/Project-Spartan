using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Collections;
using UnityEngine.EventSystems;
using Photon.Pun;
using ExitGames.Client.Photon.StructWrapping;

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
        StartCoroutine(DisplayAvailableContracts());
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
                ExitGames.Client.Photon.Hashtable bossContract = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.BossQuest, true } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(bossContract);

                ExitGames.Client.Photon.Hashtable bossContractTarget = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.BossQuestTarget, (int)acceptedContract.goal.requriedAmount} };
                PhotonNetwork.LocalPlayer.SetCustomProperties(bossContractTarget);

                ExitGames.Client.Photon.Hashtable bossContractCint = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.BossQuestCintTarget, (int)acceptedContract.cintRewards } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(bossContractCint);

                ExitGames.Client.Photon.Hashtable bossContractExp = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.BossQuestExpTarget, (int)acceptedContract.expRewards } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(bossContractExp);

                ExitGames.Client.Photon.Hashtable bossContractComp = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.BossQuestCompleted, false } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(bossContractComp);
            }
            else if (contractType == "Artifact")
            {
                ExitGames.Client.Photon.Hashtable artifactContract = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.ArtifactQuest, true } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(artifactContract);

                ExitGames.Client.Photon.Hashtable artifactContractTarget = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.ArtifactQuestTarget, (int)acceptedContract.goal.requriedAmount} };
                PhotonNetwork.LocalPlayer.SetCustomProperties(artifactContractTarget);

                ExitGames.Client.Photon.Hashtable ContractCint = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.ArtifactQuestCintTarget, (int)acceptedContract.cintRewards } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(ContractCint);

                ExitGames.Client.Photon.Hashtable ContractExp = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.ArtifactQuestExpTarget, (int)acceptedContract.expRewards } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(ContractExp);

                ExitGames.Client.Photon.Hashtable ContractComp = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.ArtifactQuestCompleted, false } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(ContractComp);
            }
            else if (contractType == "Bombs")
            {
                ExitGames.Client.Photon.Hashtable bombsContract = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.BombQuest, true } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(bombsContract);

                ExitGames.Client.Photon.Hashtable bombsContractTarget = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.BombQuestTarget, (int)acceptedContract.goal.requriedAmount} };
                PhotonNetwork.LocalPlayer.SetCustomProperties(bombsContractTarget);

                ExitGames.Client.Photon.Hashtable ContractCint = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.BombQuestCintTarget, (int)acceptedContract.cintRewards } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(ContractCint);

                ExitGames.Client.Photon.Hashtable ContractExp = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.BombQuestExpTarget, (int)acceptedContract.expRewards } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(ContractExp);

                ExitGames.Client.Photon.Hashtable ContractComp = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.BombQuestCompleted, false } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(ContractComp);
            }
            else if (contractType == "Guardian")
            {
                ExitGames.Client.Photon.Hashtable guardianContract = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.GuardianQuest, true } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(guardianContract);

                ExitGames.Client.Photon.Hashtable guardianContractTarget = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.GuardianQuestTarget, (int)acceptedContract.goal.requriedAmount } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(guardianContractTarget);

                ExitGames.Client.Photon.Hashtable ContractCint = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.GuardianQuestCintTarget, (int)acceptedContract.cintRewards } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(ContractCint);

                ExitGames.Client.Photon.Hashtable ContractExp = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.GuardianQuestExpTarget, (int)acceptedContract.expRewards } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(ContractExp);

                ExitGames.Client.Photon.Hashtable ContractComp = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.GuardianQuestCompleted, false } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(ContractComp);
            }
            else if (contractType == "Intel")
            {
                ExitGames.Client.Photon.Hashtable intelContract = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.IntelQuest, true } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(intelContract);

                ExitGames.Client.Photon.Hashtable intelContractTarget = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.IntelQuestTarget, (int)acceptedContract.goal.requriedAmount } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(intelContractTarget);

                ExitGames.Client.Photon.Hashtable ContractCint = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.IntelQuestCintTarget, (int)acceptedContract.cintRewards } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(ContractCint);

                ExitGames.Client.Photon.Hashtable ContractExp = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.IntelQuestExpTarget, (int)acceptedContract.expRewards } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(ContractExp);

                ExitGames.Client.Photon.Hashtable ContractComp = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.IntelQuestCompleted, false } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(ContractComp);
            }
            else if (contractType == "Collector")
            {
                ExitGames.Client.Photon.Hashtable collectorContract = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.CollectorQuest, true } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(collectorContract);

                ExitGames.Client.Photon.Hashtable collectorContractTarget = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.CollectorQuestTarget, (int)acceptedContract.goal.requriedAmount } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(collectorContractTarget);

                ExitGames.Client.Photon.Hashtable ContractCint = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.CollectorQuestCintTarget, (int)acceptedContract.cintRewards } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(ContractCint);

                ExitGames.Client.Photon.Hashtable ContractExp = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.CollectorQuestExpTarget, (int)acceptedContract.expRewards } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(ContractExp);

                ExitGames.Client.Photon.Hashtable ContractComp = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.CollectorQuestCompleted, false } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(ContractComp);
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
        yield return new WaitForSeconds(2.75f);
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
                    if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.BossQuest, out object contractState) && (bool)contractState == true && PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.BossQuestCompleted, out object contractComplete) && (bool)contractComplete == false)
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
                    if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.ArtifactQuest, out object contractState) && (bool)contractState == true && PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.ArtifactQuestCompleted, out object contractComplete) && (bool)contractComplete == false)
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
                    if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.BombQuest, out object contractState) && (bool)contractState == true && PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.BombQuestCompleted, out object contractComplete) && (bool)contractComplete == false)
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
                    if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.GuardianQuest, out object contractState) && (bool)contractState == true && PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.GuardianQuestCompleted, out object contractComplete) && (bool)contractComplete == false)
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
                    if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.IntelQuest, out object contractState) && (bool)contractState == true && PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.IntelQuestCompleted, out object contractComplete) && (bool)contractComplete == false)
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
                    if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.CollectorQuest, out object contractState) && (bool)contractState == true && PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.CollectorQuestCompleted, out object contractComplete) && (bool)contractComplete == false)
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