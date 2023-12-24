using UnityEngine;

public class SocketInteractor : MonoBehaviour
{
    private Vector3 originalScale;
    private bool isObjectInSocket = false;

    private void OnEnable()
    {
        originalScale = transform.localScale;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isObjectInSocket && other.CompareTag("PickupStorage"))
        {
            // Object is placed in the socket interactor
            isObjectInSocket = true;
            transform.localScale = originalScale * 0.5f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isObjectInSocket && other.CompareTag("PickupStorage"))
        {
            // Object is removed from the socket interactor
            isObjectInSocket = false;
            transform.localScale = originalScale;
        }
    }
}
