using Photon.Pun;
using System.Collections;
using UnityEngine;

public class ToxicEffect : MonoBehaviour
{
    public PlayerHealth player;
    public Collider playerCollider;
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
                            object storedToxicDamage;
                            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.TOXICITY_DAMAGE, out storedToxicDamage) && (int)storedToxicDamage >= 1)
                                enemyDamage.TakeDamage(10 + (int)storedToxicDamage);
                            else
                                enemyDamage.TakeDamage(10);
                        }

                        if (nearbyObjects.TryGetComponent<DroneHealth>(out var droneDamage))
                        {
                            object storedToxicDamage;
                            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.TOXICITY_DAMAGE, out storedToxicDamage) && (int)storedToxicDamage >= 1)
                                droneDamage.TakeDamage(10 + (int)storedToxicDamage);

                            else
                                droneDamage.TakeDamage(10);
                        }
                    }

                    if (nearbyObjects.CompareTag("Player"))
                    {
                        if (nearbyObjects.gameObject != playerCharacter)
                        {
                            if (nearbyObjects.TryGetComponent<PlayerHealth>(out var playerDamage))
                            {
                                object storedToxicDamage;
                                if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.TOXICITY_DAMAGE, out storedToxicDamage) && (int)storedToxicDamage >= 1)
                                    playerDamage.TakeDamage(5 + (int)storedToxicDamage);
                                else
                                    playerDamage.TakeDamage(5);
                            }
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

                    if (nearbyObjects.CompareTag("Player"))
                    {
                        if (nearbyObjects.gameObject != playerCharacter)
                        {
                            if (nearbyObjects.TryGetComponent<PlayerHealth>(out var playerDamage))
                            {
                                playerDamage.TakeDamage(15);
                                player.AddHealth(15);
                            }
                        }
                    }
                }
                yield return new WaitForSeconds(2);
            }
            yield return null;
        }
    }
}
