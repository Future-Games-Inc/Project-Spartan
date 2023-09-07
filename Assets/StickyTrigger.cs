using UnityEngine;
using System.Collections;

public class StickyTrigger : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(Destroy());
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleCollider(other, true);
    }

    private void OnTriggerExit(Collider other)
    {
        HandleCollider(other, false);
    }

    private void HandleCollider(Collider other, bool isEntering)
    {
        if (other.CompareTag("Player"))
        {
            var playerMovement = other.GetComponentInParent<PlayerMovement>();
            playerMovement.currentSpeed = isEntering ? .8f : playerMovement.minSpeed;
        }
        else if (other.CompareTag("Enemy") || other.CompareTag("BossEnemy"))
        {
            var followAI = other.GetComponentInParent<FollowAI>();
            followAI.stickySpeed = isEntering ? .5f : 0f;
            followAI.stuck = isEntering;
        }
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(10);
        // Check for colliders before destroying
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1.0f); // You can adjust the radius as needed
        foreach (var collider in colliders)
        {
            HandleCollider(collider, false); // Reset their speed as if they exited the trigger
        }
        Destroy(gameObject);
    }
}
