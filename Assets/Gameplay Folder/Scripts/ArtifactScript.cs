using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ArtifactScript : MonoBehaviourPunCallbacks
{
    public AudioSource audioSource;
    public AudioClip pickupClip;

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
    //    if (other.CompareTag("Player") || other.CompareTag("LeftHand") || other.CompareTag("RightHand"))
    //    {
    //        if (gameObject.CompareTag("Artifact1"))
    //        {
    //            other.GetComponentInParent<PlayerHealth>().Artifact1 = true;
    //            audioSource.PlayOneShot(pickupClip);
    //            StartCoroutine(Destroy());
    //        }

    //        if (gameObject.CompareTag("Artifact2"))
    //        {
    //            other.GetComponentInParent<PlayerHealth>().Artifact2 = true;
    //            audioSource.PlayOneShot(pickupClip);
    //            StartCoroutine(Destroy());
    //        }

    //        if (gameObject.CompareTag("Artifact3"))
    //        {
    //            other.GetComponentInParent<PlayerHealth>().Artifact3 = true;
    //            audioSource.PlayOneShot(pickupClip);
    //            StartCoroutine(Destroy());
    //        }

    //        if (gameObject.CompareTag("Artifact4"))
    //        {
    //            other.GetComponentInParent<PlayerHealth>().Artifact4 = true;
    //            audioSource.PlayOneShot(pickupClip);
    //            StartCoroutine(Destroy());
    //        }

    //        if (gameObject.CompareTag("Artifact5"))
    //        {
    //            other.GetComponentInParent<PlayerHealth>().Artifact5 = true;
    //            audioSource.PlayOneShot(pickupClip);
    //            StartCoroutine(Destroy());
    //        }
    //    }
    }

    IEnumerator Destroy()
   {
        yield return new WaitForSeconds(.75f);
        PhotonNetwork.Destroy(gameObject);
    }
}
