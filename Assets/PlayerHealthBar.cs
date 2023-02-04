using UnityEngine.UI;
using Photon.Pun;

public class PlayerHealthBar : MonoBehaviourPunCallbacks
{
    public Slider[] slider;

    public void SetMaxHealth(int maxHealth)
    {
        photonView.RPC("RPC_SetMax", RpcTarget.AllBuffered, maxHealth);
    }

    public void SetCurrentHealth(int currentHealth)
    {
        photonView.RPC("RPC_SetCurrent", RpcTarget.AllBuffered, currentHealth);
    }

    [PunRPC]
    void RPC_SetMax(int maxHealth)
    {
        foreach (Slider healthBar in slider)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = maxHealth;
        }
    }

    [PunRPC]
    void RPC_SetCurrent(int currentHealth)
    {
        foreach (Slider healthBar in slider)
        {
            healthBar.value = currentHealth;
        }
    }
}
