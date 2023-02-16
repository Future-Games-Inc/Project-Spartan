using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ArtifactScript : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("LeftHand") || other.CompareTag("RightHand"))
        {
            if (gameObject.CompareTag("Artifact1"))
            {
                other.GetComponentInParent<PlayerHealth>().Artifact1 = true;
                PhotonNetwork.Destroy(gameObject);
            }

            if (gameObject.CompareTag("Artifact2"))
            {
                other.GetComponentInParent<PlayerHealth>().Artifact2 = true;
                PhotonNetwork.Destroy(gameObject);
            }

            if (gameObject.CompareTag("Artifact3"))
            {
                other.GetComponentInParent<PlayerHealth>().Artifact3 = true;
                PhotonNetwork.Destroy(gameObject);
            }

            if (gameObject.CompareTag("Artifact4"))
            {
                other.GetComponentInParent<PlayerHealth>().Artifact4 = true;
                PhotonNetwork.Destroy(gameObject);
            }

            if (gameObject.CompareTag("Artifact6"))
            {
                other.GetComponentInParent<PlayerHealth>().Artifact5 = true;
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
