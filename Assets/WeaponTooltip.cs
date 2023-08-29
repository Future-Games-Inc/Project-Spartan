using UnityEngine;
using UnityEngine.UI;

public class WeaponTooltip : MonoBehaviour
{
    void Update()
    {
        Transform rootParent = transform.root; // Get the root parent of the GameObject

        if (Mathf.Abs(rootParent.eulerAngles.x) > 0.001f) // Check if root's x rotation is not approximately equal to 0
        {
            Vector3 currentRotation = transform.eulerAngles;
            currentRotation.x = -rootParent.eulerAngles.x; // Set the x rotation to the inverse of the root's x rotation
            transform.eulerAngles = currentRotation;
        }
    }
}
