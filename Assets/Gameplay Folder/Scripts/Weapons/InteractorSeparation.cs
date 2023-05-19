using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class InteractorSeparation : XRGrabInteractable
{
    [System.Obsolete]
    protected override void OnSelectEntered(XRBaseInteractor interactor)
    {
        base.OnSelectEntered(interactor);

        // Remove from parent GameObject
        transform.SetParent(null, true);
    }

    [System.Obsolete]
    protected override void OnSelectExited(XRBaseInteractor interactor)
    {
        base.OnSelectExited(interactor);

        // Remove from parent GameObject
        if (transform.parent != null)
            transform.SetParent(null, true);
    }
}
