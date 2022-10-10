using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AIWeapon : MonoBehaviour
{
    public GameObject bullet;
    public Transform bulletTransform;
    public float shootForce = 1000;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void Fire()
    {
        Rigidbody rb = PhotonNetwork.Instantiate("bullet", bulletTransform.position, Quaternion.identity).GetComponent<Rigidbody>();
        rb.AddForce(transform.right * shootForce);
        //rb.AddForce(transform.forward * 500f);
    }
}
