using System.Collections;
using Umbrace.Unity.PurePool;
using UnityEngine;

public class ToxicEffect : MonoBehaviour
{
    public PlayerHealth player;
    public float effectRadius;
    public GameObject playerCharacter;


    // Start is called before the first frame update
    private void OnEnable()
    {
        

        if (this.CompareTag("toxicRadius"))
            StartCoroutine(ToxicHealth());
        else
            StartCoroutine(Leech());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = playerCharacter.transform.position;
    }

    IEnumerator ToxicHealth()
    {
        yield return new WaitForSeconds(0);
        while (true)
        {
            if (player.toxicEffectActive == true)
            {
                yield return new WaitForSeconds(0);
                Collider[] colliders = Physics.OverlapSphere(transform.position, effectRadius);
                foreach (Collider nearbyObjects in colliders)
                {
                    if (nearbyObjects.CompareTag("Enemy") || nearbyObjects.CompareTag("BossEnemy") || nearbyObjects.CompareTag("Security"))
                    {
                        if (nearbyObjects.TryGetComponent<FollowAI>(out var enemyDamage))
                        {
                            if (PlayerPrefs.HasKey("TOXICITY_DAMAGE") && PlayerPrefs.GetInt("TOXICITY_DAMAGE") >=1)
                                enemyDamage.TakeDamage(10 + PlayerPrefs.GetInt("TOXICITY_DAMAGE"));
                            else
                                enemyDamage.TakeDamage(10);
                        }

                        if (nearbyObjects.TryGetComponent<DroneHealth>(out var droneDamage))
                        {
                            if (PlayerPrefs.HasKey("TOXICITY_DAMAGE") && PlayerPrefs.GetInt("TOXICITY_DAMAGE") >= 1)
                                droneDamage.TakeDamage(10 + PlayerPrefs.GetInt("TOXICITY_DAMAGE"));

                            else
                                droneDamage.TakeDamage(10);
                        }

                        if (nearbyObjects.TryGetComponent<SentryDrone>(out var sentryDamage))
                        {
                            if (PlayerPrefs.HasKey("TOXICITY_DAMAGE") && PlayerPrefs.GetInt("TOXICITY_DAMAGE") >= 1)
                                sentryDamage.TakeDamage(10 + PlayerPrefs.GetInt("TOXICITY_DAMAGE"));

                            else
                                sentryDamage.TakeDamage(10);
                        }
                    }
                }
                yield return new WaitForSeconds(2);
            }
            yield return null;
        }
    }

    IEnumerator Leech()
    {
        yield return new WaitForSeconds(0);
        while (true)
        {
            if (player.leechEffect == true)
            {
                yield return new WaitForSeconds(0);
                Collider[] colliders = Physics.OverlapSphere(transform.position, effectRadius);
                foreach (Collider nearbyObjects in colliders)
                {
                    if (nearbyObjects.CompareTag("Enemy") || nearbyObjects.CompareTag("BossEnemy") || nearbyObjects.CompareTag("Security"))
                    {
                        if (nearbyObjects.TryGetComponent<FollowAI>(out var enemyDamage))
                        {
                            enemyDamage.TakeDamage(10);
                            player.AddHealth(10);
                        }

                        if (nearbyObjects.TryGetComponent<DroneHealth>(out var droneDamage))
                        {
                            droneDamage.TakeDamage(10);
                            player.AddHealth(10);
                        }
                    }
                }
                yield return new WaitForSeconds(2);
            }
            yield return null;
        }
    }
}
