using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SocketedObjectController : MonoBehaviour
{
    public XRSocketInteractor socketInteractor;
    public float returnDelay = 10.0f;

    public GameObject currentSocketedObject;
    public GameObject targetSocketedObject;
    private Coroutine returnCoroutine;

    [System.Obsolete]
    private void Start()
    {
        socketInteractor.onSelectEntered.AddListener(OnObjectSocketed);
        socketInteractor.onSelectExited.AddListener(OnObjectUnsocketed);
    }

    private void OnObjectSocketed(XRBaseInteractable interactable)
    {
        currentSocketedObject = interactable.gameObject;
        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
            returnCoroutine = null;
        }
    }

    [System.Obsolete]
    private void OnObjectUnsocketed(XRBaseInteractable interactable)
    {
        currentSocketedObject = null;
        returnCoroutine = StartCoroutine(ReturnToSocketAfterDelay());
    }

    [System.Obsolete]
    private IEnumerator ReturnToSocketAfterDelay()
    {
        yield return new WaitForSeconds(returnDelay);
        if (currentSocketedObject == null)
        {
            XRGrabInteractable grabInteractable = targetSocketedObject.GetComponent<XRGrabInteractable>();
            if (grabInteractable == null || !grabInteractable.isSelected)
            {
                // Get the socket transform and move the object to that position
                Transform socketTransform = socketInteractor.attachTransform;
                targetSocketedObject.transform.position = socketTransform.position;
                targetSocketedObject.transform.rotation = socketTransform.rotation;
            }
        }
        returnCoroutine = null;
    }
}