using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void CompleteContract(Contract contract)
    {
        if (contract == null || IsContractCompleted(contract))
        {
            return;
        }

        contract.isCompleted = true;

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
