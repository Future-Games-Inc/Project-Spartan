using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DecoyPlayer : MonoBehaviour
{
    public GameObject decoyDeath;
    public Animator animator;
    public bool active = true;
    public NavMeshAgent agent;
    public Transform targetTransform;
    private float TurnSpeed = 5f;
    private bool isLookingAtPlayer = false;


    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(DestroyDecoy());
    }

    private void Update()
    {
        LookatTarget(1, 3f);

        if (isLookingAtPlayer)
        {
            Vector3 direction = targetTransform.position - transform.position;
            direction.y = 0;
            Quaternion desiredRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * TurnSpeed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet") && active || other.CompareTag("EnemyBullet") && active)
        StartCoroutine(DecoyKilled());
    }

    public void LookatTarget(float duration, float RotationSpeed = 0.5f)
    {
        TurnSpeed = RotationSpeed;
        IEnumerator start()
        {
            isLookingAtPlayer = true;
            yield return new WaitForSeconds(duration);
            isLookingAtPlayer = false;
        }
        StartCoroutine(start());
    }

    IEnumerator DestroyDecoy()
    {
        yield return new WaitForSeconds(15);
        StartCoroutine(DecoyKilled());
    }

    IEnumerator DecoyKilled()
    {
        Instantiate(decoyDeath, transform.position, Quaternion.identity);
        agent.isStopped = true;
        active = false;
        animator.SetTrigger("Death");
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
}
