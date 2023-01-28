using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class EnemyHealthBar : MonoBehaviourPunCallbacks
{
    public Slider slider;

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
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
    }

    [PunRPC]
    void RPC_SetCurrent(int currentHealth)
    {
        slider.value = currentHealth;
    }
}
