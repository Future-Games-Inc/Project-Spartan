using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntelScript : MonoBehaviourPunCallbacks
{
    public AudioSource audioSource;
    public AudioClip pickupClip;

    public SpawnManager1 enemyCounter;

    // Start is called before the first frame update
    void OnEnable()
    {
        enemyCounter = GameObject.FindGameObjectWithTag("spawnManager").GetComponent<SpawnManager1>();
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
            StartCoroutine(Destroy());
        }
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(.75f);
        enemyCounter.photonView.RPC("RPC_UpdateIntel", RpcTarget.AllBuffered);
        PhotonNetwork.Destroy(gameObject);
    }
}
