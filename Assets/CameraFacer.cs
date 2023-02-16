using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class CameraFacer : MonoBehaviour
{
    public Transform target;
    public TrackedPoseDriver driver;
    public float rotationSpeed = 5f;
    public float stopRotationThreshold = 0.01f;

    private bool rotating = false;


    private void Start()
    {
        StartCoroutine(Pivot());
    }
    private void Update()
    {
        if (rotating)
        {
            driver.enabled = false;
            Vector3 targetDirection = target.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            float angle = Quaternion.Angle(transform.rotation, targetRotation);

            if (angle <= stopRotationThreshold)
            {
                rotating = false;
                driver.enabled = true;
                return;
            }

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    IEnumerator Pivot()
    {
        yield return new WaitForSeconds(2);
        driver.enabled = false;
        yield return new WaitForSeconds(3);
        rotating = true;
    }
}
