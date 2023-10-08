using System.Collections;
using UnityEngine;

public class BlackoutBullet : MonoBehaviour
{
    [Header("Bullet Behavior ---------------------------------------------------")]
    public GameObject hitEffectPrefab;
    public GameObject smoke;

    private bool hasHit = false;
    public GameObject bulletOwner;
    public PlayerHealth playerHealth;
    public bool playerBullet = false;

    [Header("Bullet Effects ---------------------------------------------------")]
    public int damage = 10;
    public float blastRadius = 5.0f;

    public AudioSource audioSource;
    public AudioClip clip;

    public MeshRenderer objectRenderer;
    public float delay;

    [System.Serializable]
    public struct TargetInfo
    {
        public string Tag;
    }
    public TargetInfo[] Targets;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasHit)
        {
            hasHit = true;

            // Apply hit effect
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            Instantiate(smoke, transform.position, Quaternion.identity);

            Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);
            foreach (var target in Targets)
            {
                foreach (Collider collider in colliders)
                {
                    if (collider.CompareTag(target.Tag))
                    {
                        HandleDamage(collider, target);
                    }
                }
            }

            // Destroy bullet
            StartCoroutine(Destroy(delay));
        }
    }

    void HandleDamage(Collider collider, TargetInfo target)
    {
        float distance = Vector3.Distance(transform.position, collider.transform.position);
        int damage = CalculateDamage(distance);

        // Handle damage based on the target's tag
        switch (target.Tag)
        {
            case "Enemy":
            case "BossEnemy":
            case "Security":
                HandleEnemyDamage(collider, damage, target.Tag);
                return;
                // Add more cases as needed
        }
    }

    void HandleEnemyDamage(Collider collider, int damage, string enemyType)
    {
        FollowAI enemyDamageCrit2 = collider.GetComponent<FollowAI>();
        if (enemyDamageCrit2 != null)
        {
            if (enemyDamageCrit2.alive && enemyDamageCrit2.Health > damage)
            {
                enemyDamageCrit2.TakeDamage(damage);
                enemyDamageCrit2.EMPShock();
            }
            else if (enemyDamageCrit2.alive && enemyDamageCrit2.Health <= damage)
            {
                if (enemyType == "Enemy")
                {
                    playerHealth.EnemyKilled("Normal");
                    enemyDamageCrit2.TakeDamage(damage);
                }
                else if (enemyType == "Boss")
                {
                    playerHealth.EnemyKilled("Boss");
                    enemyDamageCrit2.TakeDamage(damage);
                }
            }
        }

        else if (enemyDamageCrit2 == null)
        {
            DroneHealth enemyDamageCrit = collider.GetComponentInParent<DroneHealth>();
            if (enemyDamageCrit != null)
            {
                if (enemyDamageCrit.Health <= (damage) && enemyDamageCrit.alive == true && enemyDamageCrit.gameObject.GetComponent<LootDrone>() != null)
                {
                    playerHealth.DroneKilled(enemyDamageCrit.gameObject);
                    enemyDamageCrit.TakeDamage(damage);
                }
                else
                    enemyDamageCrit.TakeDamage(damage);
            }
            else
            {
                SentryDrone enemyDamageCrit3 = collider.GetComponentInParent<SentryDrone>();
                enemyDamageCrit3.TakeDamage(damage);
            }
        }
    }

    int CalculateDamage(float distance)
    {
        int damageAct = (int)((1f - distance / blastRadius) * (damage/2));
        return Mathf.Clamp(damageAct, 0, 80);
    }


    IEnumerator Destroy(float delay)
    {
        objectRenderer.enabled = false;
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

}
