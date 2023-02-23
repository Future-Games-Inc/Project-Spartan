using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrappleGun : MonoBehaviour
{
    [Header("Bullet Info")]
    public GameObject bulletPrefab;
    Transform bulletTransform;
    public LayerMask targetLayer;
    public Rigidbody bulletRb;
    public float bulletSpeed;
    public GrappleBullet bulletScript;
    private Material originalMaterial;

    [Header("Gun Info")]
    public Transform barrelTransform;
    public InputActionProperty rightThumbstickPress;
    public bool grappled;
    public bool targetHit;
    private GameObject lastHit;
    public GameObject sight;

    [Header("Player Info")]
    SpringJoint springJoint;
    public GameObject playerGameObject;
    Transform playerTransform;
    public CharacterController characterController;
    public Material hitMaterial;
    public PlayerMovement movement;

    // Start is called before the first frame update
    void Start()
    {
        bulletTransform = bulletPrefab.transform;
        playerTransform = playerGameObject.transform;
        sight.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (grappled)
        {
            characterController.enabled = false;
            movement.enabled = false;
            sight.SetActive(false);
        }

        if (!grappled)
        {
            bulletTransform.position = barrelTransform.position;
            bulletTransform.forward = barrelTransform.forward;
            characterController.enabled = true;
            movement.enabled = true;
        }

        if (rightThumbstickPress.action.ReadValue<float>() >= .78f && !grappled)
        {
            FireRaycastIntoScene();
        }

        if (rightThumbstickPress.action.ReadValue<float>() < .77f && grappled)
        {
            grappled = false;
            CancelGrapple();
        }


        RaycastHit hit;
        targetHit = false;

        if (Physics.Raycast(barrelTransform.position, barrelTransform.forward, out hit, Mathf.Infinity, targetLayer))
        {
            Debug.Log("Always");
            Debug.DrawRay(barrelTransform.position, barrelTransform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            if (hit.collider.gameObject.CompareTag("GrapplePoint"))
            {
                targetHit = true;
                sight.SetActive(true);
                if (lastHit != hit.collider.gameObject)
                {
                    if (lastHit != null)
                    {
                        MeshRenderer rendererOld = lastHit.GetComponent<MeshRenderer>();
                        rendererOld.material = originalMaterial;
                    }

                    Debug.Log("Hit");
                    MeshRenderer renderer = hit.collider.gameObject.GetComponent<MeshRenderer>();
                    if (renderer != null)
                    {
                        originalMaterial = renderer.material;
                        renderer.material = hitMaterial;
                        lastHit = hit.collider.gameObject;
                    }
                }
            }
        }

        if (lastHit != null && !targetHit)
        {
            MeshRenderer rendererOld = lastHit.GetComponent<MeshRenderer>();
            rendererOld.material = originalMaterial;
            lastHit = null;
            sight.SetActive(false);
        }
    }

    void FireRaycastIntoScene()
    {
        if (targetHit)
        {
            Debug.Log("Fired");
            grappled = true;
            bulletTransform.position = barrelTransform.position;
            bulletRb.velocity = barrelTransform.forward * bulletSpeed;
        }
        else
        {
            CancelGrapple();
        }
    }

    public void CancelGrapple()
    {
        sight.SetActive(false);
        grappled = false;
        SpringJoint[] springJointList = playerGameObject.GetComponents<SpringJoint>();
        foreach (SpringJoint springJoint in springJointList)
        {
            Destroy(springJoint);
        }
        bulletScript.DestroyJoint();
    }

    public void Swing()
    {
        sight.SetActive(false);
        characterController.enabled = false;
        movement.enabled = false;
        if (playerGameObject.GetComponent<SpringJoint>() == null)
            springJoint = playerGameObject.AddComponent<SpringJoint>();
        springJoint.connectedBody = bulletScript.collisionObject.GetComponent<Rigidbody>();
        springJoint.autoConfigureConnectedAnchor = false;
        springJoint.connectedAnchor = bulletScript.collisionObject.transform.InverseTransformPoint(bulletScript.hitPoint);
        springJoint.anchor = Vector3.zero;

        float disJointToPlayer = Vector3.Distance(playerTransform.position, bulletTransform.position);

        springJoint.maxDistance = disJointToPlayer * .4f;
        springJoint.minDistance = disJointToPlayer * .05f;

        springJoint.damper = 300f;
        springJoint.spring = 1000f;
    }

}
