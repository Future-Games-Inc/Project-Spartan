using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntelScript : MonoBehaviourPunCallbacks
{
    public AudioSource audioSource;
    public AudioClip pickupClip;

    // Start is called before the first frame update
    void OnEnable()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PickupSlot"))
        {
            other.GetComponentInParent<PlayerHealth>().IntelFound();
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
