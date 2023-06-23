using UnityEngine;

[CreateAssetMenu(fileName = "New Pickup", menuName = "Game/Pickup")]
public class Pickup : ScriptableObject
{
    public string pickupType;
    public int xpAmount;
    public int healthAmount;
    public int toxicAmount;
    public int bulletModifierDamage;
    public int bulletModifierCount;
    public int armorAmount;
}
