using UnityEngine;

/// <summary>
/// Rotates the camera in the beginning of the scene to give
/// it a nice visual.
/// </summary>
public class SimpleRotate: MonoBehaviour
{
    [SerializeField]
    private Vector3 rotation;

    [SerializeField]
    private float rotateSpeed;
    void Update()
    {
        transform.Rotate(rotation * rotateSpeed * Time.deltaTime);
    }
}
