using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRAnimatorController : MonoBehaviour
{
    public float speedThreashold = 0.1f;
    [Range(0,1)]
    public float smoothing = 1;

    private Animator aniomator;
    private Vector3 previousPosition;
    private VRRig vrRig;


    // Start is called before the first frame update
    void Start()
    {
        aniomator = GetComponent<Animator>();
        vrRig = GetComponent<VRRig>();
        previousPosition = vrRig.head.vrTarget.position;
    }

    // Update is called once per frame
    void Update()
    {
        // compute the speed of the head

        Vector3 headsetSpeed = (vrRig.head.vrTarget.position - previousPosition) / Time.deltaTime;
        headsetSpeed.y = 0;

        // Local Speed
        Vector3 headsetLocalSpeed = transform.InverseTransformDirection(headsetSpeed);
        previousPosition = vrRig.head.vrTarget.position;

        // Set Animator Values
        float previoucDirectionX = aniomator.GetFloat("directionX");
        float previoucDirectionY = aniomator.GetFloat("directionY");


        aniomator.SetBool("IsMoving", headsetLocalSpeed.magnitude > speedThreashold);
        aniomator.SetFloat("directionX", Mathf.Lerp(previoucDirectionX, Mathf.Clamp(headsetLocalSpeed.x, -1, 1), smoothing));
        aniomator.SetFloat("directionY", Mathf.Lerp(previoucDirectionY, Mathf.Clamp(headsetLocalSpeed.z, -1, 1), smoothing));
    }
}
