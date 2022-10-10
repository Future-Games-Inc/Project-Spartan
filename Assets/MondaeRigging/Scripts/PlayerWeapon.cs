using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
    using UnityEngine.XR.Interaction.Toolkit;

public class PlayerWeapon : MonoBehaviour
{
    public Transform[] spawnPoint;
    public float fireSpeed = 20;

    // Start is called before the first frame update
    void Start()
    {
        XRGrabNetworkInteractable grabbable = GetComponent<XRGrabNetworkInteractable>();
        grabbable.activated.AddListener(FireBullet);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FireBullet(ActivateEventArgs arg)
    {
        foreach (Transform t in spawnPoint)
        {
            GameObject spawnedBullet = PhotonNetwork.Instantiate("bullet", t.position, Quaternion.identity);
            spawnedBullet.GetComponent<Rigidbody>().velocity = t.forward * fireSpeed;
        }
    }
    

}
