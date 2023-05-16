using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[System.Serializable]
public class Haptic
{
    [Range(0, 1)]
    public float intensity;
    public float duration;

    public void TriggerHaptic(BaseInteractionEventArgs eventArgs)
    {
        if(eventArgs.interactorObject is XRBaseControllerInteractor controllerInteractor)
        {
            TriggerHaptic(controllerInteractor.xrController);
        }
    }

    public void TriggerHaptic(XRBaseController controller)
    {
        if (intensity > 0)
            controller.SendHapticImpulse(intensity, duration);
    }
}

public class HapticsInteractable : MonoBehaviour
{
    public Haptic hapticOnActivated;
    public Haptic hapticHoveredEntered;
    public Haptic hapticHoveredExited;
    public Haptic hapticSelectdEntered;
    public Haptic hapticSelectdExited;

    // Start is called before the first frame update
    void Start()
    {
        XRBaseInteractable interactable = GetComponent<XRBaseInteractable>();
        interactable.activated.AddListener(hapticOnActivated.TriggerHaptic);

        interactable.activated.AddListener(hapticHoveredEntered.TriggerHaptic);
        interactable.activated.AddListener(hapticHoveredExited.TriggerHaptic);
        interactable.activated.AddListener(hapticSelectdEntered.TriggerHaptic);
        interactable.activated.AddListener(hapticSelectdExited.TriggerHaptic);
    }
}
