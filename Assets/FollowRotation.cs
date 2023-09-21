using UnityEngine;

public class FollowRotation : MonoBehaviour
{
    public GameObject parent;
    void Update()
    {
        // Extract the parent's rotation angles
        Vector3 parentEulerAngles = parent.transform.rotation.eulerAngles;

        // Extract the child's rotation angles
        Vector3 childEulerAngles = transform.rotation.eulerAngles;

        // Set the child's rotation to match the parent's Y-axis rotation
        // while keeping the child's original X and Z rotations
        transform.rotation = Quaternion.Euler(new Vector3(childEulerAngles.x, parentEulerAngles.y, childEulerAngles.z));
    }
}
