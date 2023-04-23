using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRFootIK1 : MonoBehaviour
{

    private Animator m_anim;
    public Vector3 m_footOffset;
    [Range(0,1)]
    public float m_rightFootPosWeight = 1;
    [Range(0, 1)]
    public float m_rightFootRotWeight = 1;
    [Range(0, 1)]
    public float m_leftFootPosWeight = 1;
    [Range(0, 1)]
    public float m_leftFootRotWeight = 1;

    // Start is called before the first frame update
    void Start()
    {
        m_anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void OnAnimatorIK(int layerIndex)
    {
        Vector3 rightFootPos = m_anim.GetIKPosition(AvatarIKGoal.RightFoot);
        RaycastHit hit;

        bool hasHit = Physics.Raycast(rightFootPos + Vector3.up, Vector3.down, out hit);
        if(hasHit)
        {
            m_anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, m_rightFootPosWeight);
            m_anim.SetIKPosition(AvatarIKGoal.RightFoot, hit.point + m_footOffset);

            Quaternion footRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, hit.normal), hit.normal);
            m_anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, m_rightFootRotWeight);
            m_anim.SetIKRotation(AvatarIKGoal.RightFoot, footRotation);
        }
        else
        {
            m_anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0);
        }

        Vector3 leftFootPos = m_anim.GetIKPosition(AvatarIKGoal.LeftFoot);

        hasHit = Physics.Raycast(leftFootPos + Vector3.up, Vector3.down, out hit);
        if (hasHit)
        {
            m_anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, m_leftFootPosWeight);
            m_anim.SetIKPosition(AvatarIKGoal.LeftFoot, hit.point + m_footOffset);

            Quaternion leftFootRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, hit.normal), hit.normal);
            m_anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, m_leftFootRotWeight);
            m_anim.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootRotation);
        }
        else
        {
            m_anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0);
        }
    }
}
