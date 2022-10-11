using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Pixelplacement;

public class ShrinkSocketWithTagCheck : XRSocketInteractor
{
    // sizing
    public float targetSize = 0.25f;
    public float sizingDuration = 0.25f;

    // Runttime
    private Vector3 originalScale = Vector3.one;
    private Vector3 objectSize = Vector3.zero;

    //  prevents ranfom object from being selected;
    private bool canSelect = false;

    public string targetTag = string.Empty;

    public override bool CanHover(XRBaseInteractable interactable)
    {
        return base.CanHover(interactable) && MatchUsingTag(interactable);

    }

    public override bool CanSelect(XRBaseInteractable interactable)
    {
        return base.CanSelect(interactable) && MatchUsingTag(interactable);
    }


    private bool MatchUsingTag(XRBaseInteractable interactable)
    {

        return interactable.CompareTag(targetTag);
    }



    protected override void OnHoverEntering(XRBaseInteractable interactable)
    {
        base.OnHoverEntering(interactable);

        if (interactable.isSelected)
        {
            canSelect = true;
        }
    }

    protected override void OnHoverExiting(XRBaseInteractable interactable)
    {
        base.OnHoverExiting(interactable);

        if (!selectTarget)
        {
            canSelect = false;
        }

    }

    protected override void OnSelectEntering(XRBaseInteractable interactable)
    {
        base.OnSelectEntering(interactable);
        StoreObjectSizeScale(interactable);
    }


    protected override void OnSelectEntered(XRBaseInteractable interactable)
    {
        base.OnSelectEntered(interactable);
        TweenSizeToSocket(interactable);
    }

    protected override void OnSelectExited(XRBaseInteractable interactable)
    {
        base.OnSelectExited(interactable);
        SetOriginalScale(interactable);
    }


    private void StoreObjectSizeScale(XRBaseInteractable interactable)
    {
        //Find objects size
        objectSize = FindObjectSize(interactable.gameObject);
        originalScale = interactable.transform.localScale;

    }

    private Vector3 FindObjectSize(GameObject objectToMeasure)
    {
        Vector3 size = Vector3.one;

        if (objectToMeasure.TryGetComponent(out MeshFilter meshFilter))
        {
            size = ColliderMeasurer.Instance.Measure(meshFilter.mesh);
        }


        return size;
    }

    private void TweenSizeToSocket(XRBaseInteractable interactable)
    {
        // Find the new scale based on the size of the collider and scale
        Vector3 targetScale = FindTargetScale();
        // tween to our new scale
        Tween.LocalScale(interactable.transform, targetScale, sizingDuration, 0);
    }

    private Vector3 FindTargetScale()
    {
        // Figure out new scale, we assume the object is orginally uniformly scales
        float largetSize = FindLargestSize(objectSize);
        float scaleDifference = targetSize / largetSize;



        return Vector3.one * scaleDifference;
    }

    private void SetOriginalScale(XRBaseInteractable interactable)
    {
        // not necessary, but prevents an error when exitng play


        if (interactable)
        {
            //restore the object to original scale
            interactable.transform.localScale = originalScale;
            //reset just in case
            originalScale = Vector3.one;
            objectSize = Vector3.one;
        }


    }


    private float FindLargestSize(Vector3 value)
    {
        float largestSize = Mathf.Max(value.x, value.y);
        largestSize = Mathf.Max(largestSize, value.z);

        return largestSize;
    }


    public override XRBaseInteractable.MovementType? selectedInteractableMovementTypeOverride
    {
        // move while ignoring physics and no smoothing. 
        get { return XRBaseInteractable.MovementType.Instantaneous; }
    }

    //is the socket active, and object is being held by different interactor
    public override bool isSelectActive => base.isSelectActive;

    private void OnDrawGizmos()
    {
        // draw the approximate sice of the socketed object
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, targetSize * 0.5f);
    }
}
