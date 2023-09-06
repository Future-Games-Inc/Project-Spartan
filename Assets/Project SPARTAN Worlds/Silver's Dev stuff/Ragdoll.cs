using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    Rigidbody[] rigidbodies;
    Animator animator;
    public void SetUp()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        animator = GetComponent<Animator>();
        SetActive(false);
    }

    public void SetActive(bool state)
    {
        if (!state)
        {
            foreach(var rigidBody in rigidbodies)
            {
                rigidBody.isKinematic = true;
            }
            animator.enabled = true;
        } 
        else
        {
            foreach (var rigidBody in rigidbodies)
            {
                rigidBody.isKinematic = false;
            }
            animator.enabled = false;
        }
    }
}
