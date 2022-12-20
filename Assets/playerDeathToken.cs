using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerDeathToken : MonoBehaviour
{
    public int tokenValue;
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
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerHealth>().UpdateSkills(tokenValue);
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
