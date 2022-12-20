using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GrappleGun : MonoBehaviour
{
    [Header("Bullet Info")]
    public GameObject bulletPrefab;
    Transform bulletTransform;
    public LayerMask targetLayer;
    public Rigidbody bulletRb;
    public float bulletSpeed;
    public GrappleBullet bulletScript;

    [Header("Gun Info")]
    public Transform barrelTransform;
    private string m_grappleButton;
    private Handness m_hand2 = Handness.Left;
    public bool grappled;

    [Header("Player Info")]
    SpringJoint springJoint;
    public GameObject playerGameObject;
    Transform playerTransform;
    public CharacterController characterController;
    public TrailRenderer trailRenderer;

    // Start is called before the first frame update
    void Start()
    {
        bulletTransform = bulletPrefab.transform;
        m_grappleButton = "XRI_" + m_hand2 + "_SecondaryButton";
        playerTransform = playerGameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(grappled)
        {
            characterController.enabled = false;
        }

        if(!grappled)
        {
            bulletTransform.position = barrelTransform.position;
            bulletTransform.forward = barrelTransform.forward;
            characterController.enabled = true;
        }

        if (Input.GetButtonDown(m_grappleButton))
        {
            FireRaycastIntoScene();
        }

        if (Input.GetButtonUp(m_grappleButton))
        {
            CancelGrapple();
        }
    }

    void FireRaycastIntoScene()
    {
        RaycastHit hit;

        if (Physics.Raycast(bulletTransform.position, bulletTransform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, targetLayer))
        {
            trailRenderer.enabled = true;
            bulletTransform.position = barrelTransform.position;
            bulletRb.velocity = bulletTransform.forward * bulletSpeed;
            grappled = true;
        }
    }

    public void CancelGrapple()
    {
        grappled = false;
        Destroy(springJoint);
        bulletScript.DestroyJoint();
    }

    public void Swing()
    {
        springJoint = playerGameObject.AddComponent<SpringJoint>();
        springJoint.connectedBody = bulletScript.collisionObject.GetComponent<Rigidbody>();
        springJoint.autoConfigureConnectedAnchor = false;
        springJoint.connectedAnchor = bulletScript.collisionObject.transform.InverseTransformPoint(bulletScript.hitPoint);
        springJoint.anchor = Vector3.zero;

        float disJointToPlayer = Vector3.Distance(playerTransform.position, bulletTransform.position);

        springJoint.maxDistance = disJointToPlayer * .6f;
        springJoint.minDistance = disJointToPlayer * .1f;

        springJoint.damper = 100f;
        springJoint.spring = 700f;
    }

}
