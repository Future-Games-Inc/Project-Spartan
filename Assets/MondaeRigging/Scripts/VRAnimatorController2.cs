using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRAnimatorController1 : MonoBehaviour
{
    public float speedThreshold = 0.1f;
    [Range(0,1)]
    public float m_smoothing = 1;
    private Animator m_anim;
    public Vector3 m_previousPos;
    private VRRig1 m_vrRig;

    // Start is called before the first frame update
    void Start()
    {
        m_anim = GetComponent<Animator>();
        m_vrRig = GetComponent<VRRig1>();
    }

    // Update is called once per frame
    void Update()
    {
        //Compute the spped
        Vector3 headsetSpeed = (m_vrRig.m_head.m_vrTarget.position - m_previousPos) / Time.deltaTime;
        headsetSpeed.y = 0;

        //Local speed
        Vector3 headsetLocalSpeed = transform.InverseTransformDirection(headsetSpeed);
        m_previousPos = m_vrRig.m_head.m_vrTarget.position;

        //Set Animator values
        float previousDirectionX = m_anim.GetFloat("DirectionX");
        float previousDirectionY = m_anim.GetFloat("DirectionY");

        m_anim.SetBool("isMoving", headsetLocalSpeed.magnitude > speedThreshold);
        m_anim.SetFloat("DirectionX", Mathf.Lerp(previousDirectionX, Mathf.Clamp(headsetLocalSpeed.x, -1, 1), m_smoothing));
        m_anim.SetFloat("DirectionY", Mathf.Lerp(previousDirectionY, Mathf.Clamp(headsetLocalSpeed.z, -1, 1), m_smoothing));
    }
}
