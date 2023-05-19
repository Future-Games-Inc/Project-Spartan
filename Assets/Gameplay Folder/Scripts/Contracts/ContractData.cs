using UnityEngine;

[CreateAssetMenu(fileName = "ContractData", menuName = "Game/Contract Data")]
public class ContractData : ScriptableObject
{
    public Contract[] contracts;
}
