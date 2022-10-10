using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerWeapon1 : MonoBehaviour
{
    public Transform[] spawnPoint;
    public float fireSpeed = 200;

    public GameObject playerBullet;

    // Start is called before the first frame update
    void Start()
    {
        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
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
            GameObject spawnedBullet = Instantiate(playerBullet, t.position, Quaternion.identity);
            spawnedBullet.GetComponent<Rigidbody>().velocity = t.forward * fireSpeed;
        }
    }
    

}
