using BNG;
using Photon.Pun;
using UnityEngine;

public class IntelScript : MonoBehaviourPunCallbacks
{
    public AudioClip pickupClip;
    public LayerMask groundLayer;
    private Rigidbody rb;
    public Grabbable grabbable;

    // Start is called before the first frame update
    void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        // Freeze X and Z initially
        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
    }

    // Update is called once per frame
    void Update()
    {
        CheckGroundDistance();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PickupSlot"))
        {
            other.GetComponentInParent<PlayerHealth>().IntelFound();
            PhotonNetwork.Destroy(gameObject);
        }
    }

    public void FreezeConstraints()
    {
        CheckGroundDistance();
    }

    private void CheckGroundDistance()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, groundLayer))
        {
            if (hit.distance < 0.2f  && !grabbable.BeingHeld)
            {
                // When the object is less than 0.2m from the ground, unfreeze X and Z
                rb.constraints &= ~RigidbodyConstraints.FreezePositionX;
                rb.constraints &= ~RigidbodyConstraints.FreezePositionZ;
            }
            else
            {
                rb.constraints &= RigidbodyConstraints.FreezePositionX;
                rb.constraints &= RigidbodyConstraints.FreezePositionZ;
            }
        }
    }

}
