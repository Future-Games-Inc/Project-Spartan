using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleBullet : MonoBehaviour
{
    FixedJoint fixedJoint;
    [HideInInspector]
    public GameObject collisionObject;

    public GrappleGun grappleGun;
    public Vector3 hitPoint;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "GrapplePoint" && grappleGun.grappled)
        {
            hitPoint = collision.contacts[0].point;
            collisionObject = collision.gameObject;
            fixedJoint = gameObject.AddComponent<FixedJoint>();
            fixedJoint.connectedBody = collision.gameObject.GetComponent<Rigidbody>();

            grappleGun.Swing();
        }
    }

    public void DestroyJoint()
    {
        Destroy(fixedJoint);
    }
}
