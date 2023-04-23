using UnityEngine;
using Photon.Pun;

public class VaultManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Vault vault;
    public void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Set the start rotation of the vault
            vault.transform.rotation = Quaternion.Euler(0, vault.startY, 0);
        }
    }

    [PunRPC]
    public void RotateVault(float y, float elapsedTime, float sliderValue)
    {
        // Rotate the vault object
        vault.transform.rotation = Quaternion.Euler(0, y, 0);

        // Synchronize activation timer and slider value
        vault.activated = true;
        vault.elapsedTime = elapsedTime;
        vault.activationSlider.gameObject.SetActive(true);
        vault.activationSlider.value = sliderValue;
    }
}