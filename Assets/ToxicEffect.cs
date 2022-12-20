using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToxicEffect : MonoBehaviour
{
    public PlayerHealth playerTimer;
    public Collider playerCollider;
    public float effectRadius = 5f;
    // Start is called before the first frame update
    private void OnEnable()
    {
        StartCoroutine(ToxicHealth());
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator ToxicHealth()
    {
        yield return new WaitForSeconds(0);
        while (playerTimer.toxicEffectActive == true)
        {
            yield return new WaitForSeconds(0);
            Collider[] colliders = Physics.OverlapSphere(transform.position, effectRadius);
            foreach (Collider nearbyObjects in colliders)
            {
                if (nearbyObjects.CompareTag("Enemy"))
                {
                    FollowAI enemyDamage = nearbyObjects.GetComponent<FollowAI>();
                    {
                        if (enemyDamage != null)
                        {
                            object storedToxicDamage;
                            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.TOXICITY_DAMAGE, out storedToxicDamage) && (int)storedToxicDamage >= 1)
                                enemyDamage.TakeDamage(10 + (int)storedToxicDamage);
                            else
                                enemyDamage.TakeDamage(10);

                        }
                    }

                    if (enemyDamage == null)
                    {
                        DroneHealth droneDamage = nearbyObjects.GetComponent<DroneHealth>();
                        if (droneDamage != null)
                        {
                            object storedToxicDamage;
                            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.TOXICITY_DAMAGE, out storedToxicDamage) && (int)storedToxicDamage >= 1)
                                droneDamage.TakeDamage(10 + (int)storedToxicDamage);
                            else
                            droneDamage.TakeDamage(10);
                        }
                    }
                }

                if (nearbyObjects.CompareTag("Player"))
                {
                    if (nearbyObjects != playerCollider)
                    {
                        PlayerHealth playerDamage = nearbyObjects.GetComponent<PlayerHealth>();
                        if (playerDamage != null)
                        {
                            object storedToxicDamage;
                            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.TOXICITY_DAMAGE, out storedToxicDamage) && (int)storedToxicDamage >= 1)
                                playerDamage.TakeDamage(2 + (int)storedToxicDamage);
                            else
                            playerDamage.TakeDamage(2);
                        }
                    }
                }
            }
            yield return new WaitForSeconds(5);
        }
    }
}
