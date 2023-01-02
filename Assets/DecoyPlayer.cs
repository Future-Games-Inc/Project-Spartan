using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DecoyPlayer : MonoBehaviour
{
    public int avatarLoader;
    public GameObject[] AvatarModelPrefabs;
    public GameObject decoyDeath;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        decoyDeath.SetActive(false);
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
        StartCoroutine(DecoyKilled());
    }

    IEnumerator DestroyDecoy()
    {
        yield return new WaitForSeconds(15);
        decoyDeath.SetActive(true);
        animator.SetTrigger("Death");
        yield return new WaitForSeconds(3f);
        PhotonNetwork.Destroy(gameObject);
    }

    IEnumerator DecoyKilled()
    {
        animator.SetTrigger("Death");
        decoyDeath.SetActive(true);
        yield return new WaitForSeconds(3f);
        PhotonNetwork.Destroy(gameObject);
    }
}
