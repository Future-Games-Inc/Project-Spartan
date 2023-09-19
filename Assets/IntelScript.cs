using BNG;
using System.Collections;
using UnityEngine;

public class IntelScript : MonoBehaviour
{
    public AudioClip pickupClip;
    public LayerMask groundLayer;
    private Rigidbody rb;
    public Grabbable grabbable;
    public bool contact;


    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(NoContact());
    }

    IEnumerator NoContact()
    {
        yield return new WaitForSeconds(10);
        if (contact == false)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LeftHand") || other.CompareTag("RightHand"))
        {
            contact = true;
        }

        if (other.CompareTag("PickupSlot"))
        {
            other.GetComponentInParent<PlayerHealth>().IntelFound();
            Destroy(gameObject);
        }
    }
}
