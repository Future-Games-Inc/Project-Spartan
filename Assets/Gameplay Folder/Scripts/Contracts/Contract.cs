using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Contract
{
    public int ID;
    public string description;
    public string objectives;
    public bool isCompleted;
    public float duration; // Duration in seconds
    public DateTime completionTime;

    public Contract(int id, string desc, string obj, float dur)
    {
        ID = id;
        description = desc;
        objectives = obj;
        duration = dur;
        isCompleted = false;
        completionTime = DateTime.MinValue;
    }
}

[CreateAssetMenu(fileName = "ContractData", menuName = "Game/Contract Data")]
public class ContractData : ScriptableObject
{
    public Contract[] contracts;
}

public class ContractManager : MonoBehaviour
{
    public ContractData contractData; // Reference to the ContractData scriptable object

    private List<Contract> activeContracts = new List<Contract>();
    private List<Contract> completedContracts = new List<Contract>();

    public void AddContract(Contract contract)
    {
        if (contract == null || IsContractActive(contract) || activeContracts.Count >= GetMaxContracts(contract))
        {
            return;
        }

        activeContracts.Add(contract);
    }

    public void RemoveContract(Contract contract)
    {
        if (contract == null || !IsContractActive(contract))
        {
            return;
        }

        activeContracts.Remove(contract);
    }

    public bool IsContractActive(Contract contract)
    {
        return activeContracts.Contains(contract);
    }

    public bool IsContractCompleted(Contract contract)
    {
        return completedContracts.Contains(contract);
    }

    public bool IsContractExpired(Contract contract)
    {
        if (contract == null || !IsContractCompleted(contract))
        {
            return false;
        }

        DateTime expirationTime = contract.completionTime.AddDays(7); // Assuming a one-week duration

        return DateTime.Now >= expirationTime;
    }

    public void CompleteContract(Contract contract)
    {
        if (contract == null || IsContractCompleted(contract))
        {
            return;
        }

        contract.isCompleted = true;
        contract.completionTime = DateTime.Now;

        completedContracts.Add(contract);
    }

    private int GetMaxContracts(Contract contract)
    {
        // Determine the maximum number of contracts based on the source (black market or terminal)
        int maxContracts = 0;

        if (contractData.contracts != null)
        {
            maxContracts = contractData.contracts.Length;
        }

        return maxContracts;
    }
}
