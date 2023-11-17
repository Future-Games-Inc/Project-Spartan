using UnityEngine;

public class FollowRotation : MonoBehaviour
{
    public GameObject parent;
    public float distanceBelowParent = 1.0f; // The distance below the parent you want the gameObject to be

    void Update()
    {
        // Extract the parent's rotation angles
        Vector3 parentEulerAngles = parent.transform.rotation.eulerAngles;

        // Extract the child's rotation angles
        Vector3 childEulerAngles = transform.rotation.eulerAngles;

        // Set the child's rotation to match the parent's Y-axis rotation
        // while keeping the child's original X and Z rotations
        transform.rotation = Quaternion.Euler(new Vector3(childEulerAngles.x, parentEulerAngles.y, childEulerAngles.z));

        // Calculate the position below the parent based on distanceBelowParent
        Vector3 parentPosition = parent.transform.position;
        Vector3 newPosition = parentPosition - new Vector3(0, distanceBelowParent, 0);

        // Set the gameObject's position
        transform.position = newPosition;
    }
}

