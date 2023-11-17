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
        if (collision.gameObject.CompareTag("GrapplePoint") && grappleGun.grappled)
        {
            hitPoint = collision.contacts[0].point;
            collisionObject = collision.gameObject;
            if (gameObject.GetComponent<FixedJoint>() == null)
                fixedJoint = gameObject.AddComponent<FixedJoint>();
            if(collisionObject.GetComponent<Rigidbody>() == null)
            {
                var rb = collisionObject.AddComponent<Rigidbody>();
                rb.useGravity = false;
                rb.isKinematic = true;
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            }
            fixedJoint.connectedBody = collisionObject.GetComponent<Rigidbody>();

            grappleGun.Swing();
        }
    }

    public void DestroyJoint()
    {
        FixedJoint[] fixedJointList = gameObject.GetComponents<FixedJoint>();
        foreach (FixedJoint fixedJoint in fixedJointList)
        {
            Destroy(fixedJoint);
        }
    }
}
