using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.CoreUtils;

[System.Serializable]
public class VRMap1
{
    public Transform m_vrTarget;
    public Transform m_rigTarget;
    public Vector3 m_trackingPositionOffset;
    public Vector3 m_trackingRotationOffset;

    public void VRMapping()
    {
        m_rigTarget.position = m_vrTarget.TransformPoint(m_trackingPositionOffset);
        m_rigTarget.rotation = m_vrTarget.rotation * Quaternion.Euler(m_trackingRotationOffset);
    }
}

public class VRRig1 : MonoBehaviour
{
    public VRMap1 m_head;
    public VRMap1 m_leftHand;
    public VRMap1 m_rightHand;
    public XROrigin rig;

    public Transform m_headConstraint;
    public Vector3 m_headBodyOffset;
    public float m_turnSmoothness;

    // Start is called before the first frame update
    void Start()
    {
        //m_headBodyOffset = transform.position - m_headConstraint.position;

        rig = FindObjectOfType<XROrigin>();

        m_head.m_vrTarget = rig.GetComponent<RigSetup>().m_xrCamera.transform;
        m_head.m_vrTarget.position = m_head.m_vrTarget.transform.position;

        m_leftHand.m_vrTarget = rig.GetComponent<RigSetup>().m_lefthand.transform;
        m_leftHand.m_vrTarget.position = m_leftHand.m_vrTarget.transform.position;

        m_rightHand.m_vrTarget = rig.GetComponent<RigSetup>().m_rightHand.transform;
        m_rightHand.m_vrTarget.position = m_rightHand.m_vrTarget.position;

        GetComponent<VRAnimatorController1>().m_previousPos = m_head.m_vrTarget.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = m_headBodyOffset + m_headConstraint.position;
        transform.forward = Vector3.Lerp(transform.forward,
            Vector3.ProjectOnPlane(m_headConstraint.up, Vector3.up).normalized, Time.deltaTime * m_turnSmoothness);

        m_head.VRMapping();
        m_leftHand.VRMapping();
        m_rightHand.VRMapping();
    }
}
