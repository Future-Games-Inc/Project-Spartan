using System.Collections;
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

    [Header("Gun Info")]
    public Transform barrelTransform;
    public InputActionProperty rightThumbstickPress;
    public bool grappled;
    public bool targetHit;
    public GameObject sight;
    public AudioSource audioSource;
    public AudioClip grappleClip;

    [Header("Player Info")]
    SpringJoint springJoint;
    public GameObject playerGameObject;
    Transform playerTransform;
    public CharacterController characterController;
    public PlayerMovement movement;
    public bool grappledAvailable;

    public GameObject grappleIcon;
    public Rigidbody playerRb;

    // Start is called before the first frame update
    void OnEnable()
    {
        bulletTransform = bulletPrefab.transform;
        playerTransform = playerGameObject.transform;
        sight.SetActive(false);
        StartCoroutine(Recharge());
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

        if (rightThumbstickPress.action.ReadValue<float>() >= .78f && !grappled && grappledAvailable)
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
            if (hit.collider.gameObject.CompareTag("GrapplePoint") && grappledAvailable)
            {
                targetHit = true;
                sight.SetActive(true);
            }
            else
                sight.SetActive(false);
        }

        if(!grappledAvailable)
            sight.SetActive(false);

        grappleIcon.SetActive(grappledAvailable);
    }

    void FireRaycastIntoScene()
    {
        if (grappledAvailable)
        {
            if (targetHit)
            {
                grappled = true;
                grappledAvailable = false;
                bulletTransform.position = barrelTransform.position;
                bulletRb.velocity = barrelTransform.forward * bulletSpeed;
                StartCoroutine(DestroyHook());
            }
            else
            {
                CancelGrapple();
            }
        }
    }

    IEnumerator DestroyHook()
    {
        yield return new WaitForSeconds(2);
        CancelGrapple();
        yield return new WaitForSeconds(10);
        grappledAvailable = true;
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
        if (playerRb)
        {
            Destroy(playerRb);
        }
    }

    public void Swing()
    {
        if (playerGameObject.GetComponent<Rigidbody>() == null)
        {
            playerRb = playerGameObject.AddComponent<Rigidbody>();
            playerRb.constraints = RigidbodyConstraints.FreezeRotation;
            playerRb.useGravity = false; // Optional: This depends on if you want gravity to affect the player during the swing.
        }

        sight.SetActive(false);
        audioSource.PlayOneShot(grappleClip);
        characterController.enabled = false;
        movement.enabled = false;
        if (playerGameObject.GetComponent<SpringJoint>() == null)
            springJoint = playerGameObject.AddComponent<SpringJoint>();
        springJoint.connectedBody = bulletScript.collisionObject.GetComponent<Rigidbody>();
        springJoint.autoConfigureConnectedAnchor = false;
        springJoint.connectedAnchor = bulletScript.collisionObject.transform.InverseTransformPoint(bulletScript.hitPoint);
        springJoint.anchor = Vector3.zero;

        float disJointToPlayer = Vector3.Distance(playerTransform.position, bulletTransform.position);

        springJoint.maxDistance = disJointToPlayer * .1f;
        springJoint.minDistance = disJointToPlayer * .05f;

        springJoint.damper = 450f;
        springJoint.spring = 500f;
    }

    IEnumerator Recharge()
    {
        yield return new WaitForSeconds(20);
        grappledAvailable = true;
    }

}
