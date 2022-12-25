using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DecoyPlayer : MonoBehaviour
{
    public int avatarLoader;
    public GameObject[] AvatarModelPrefabs;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyDecoy());
        object avatarSelectionNumber;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AVATAR_SELECTION_NUMBER, out avatarSelectionNumber))
        {
            avatarLoader = (int)avatarSelectionNumber;
        }

        for (int i = 0; i < AvatarModelPrefabs.Length; i++)
        {
            if (AvatarModelPrefabs[avatarLoader] == AvatarModelPrefabs[i])
            {
                AvatarModelPrefabs[i].SetActive(true);
            }
            else
            {
                AvatarModelPrefabs[i].SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
            PhotonNetwork.Destroy(gameObject);
    }

    IEnumerator DestroyDecoy()
    {
        yield return new WaitForSeconds(15);
            PhotonNetwork.Destroy(gameObject);
    }
}
