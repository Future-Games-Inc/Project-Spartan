using UnityEngine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;

public class NetworkGrenade : MonoBehaviourPunCallbacks
{
    public string EnemyTag = "Enemy";
    public string BossEnemyTag = "BossEnemy";
    public string SecurityTag = "Security";
    public string PlayerTag = "Player";
    public float explosionRadius = 5f;
    public int maxDamage = 80;
    public float activationDelay = 0.25f;
    public float explosionDelay = 5f;

    private bool activated = false;

    public GameObject player;
    public PlayerHealth playerHealth;
    public GameObject explosionEffect;
    public GameObject Display;
    public Rigidbody rb;

    public float throwForce;
    public float throwUpwardForce;

    public bool showGizmos;

    public string Type = "";

    public AudioSource audioSource;
    void Start()
    {

    }

    // Call this method to activate the grenade
    //public void ActivateGrenade()
    //{
    //    if (!activated)
    //    {
    //        StartCoroutine(ActivateDelayed());
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LeftHand") || other.CompareTag("RightHand"))
        {

        }
    }

    public void Throw()
    {
        if (rb.useGravity == false)
            rb.useGravity = true;
        if (rb.isKinematic == true)
            rb.isKinematic = false;
        Camera cam = player.GetComponentInChildren<Camera>();
        Vector3 forceToAdd = cam.transform.forward * throwForce * throwUpwardForce;
        rb.AddForce(forceToAdd);
        photonView.RPC("GrenadeSound", RpcTarget.All);
        StartCoroutine(ExplodeDelayed());
    }

    IEnumerator ActivateDelayed()
    {
        yield return new WaitForSeconds(activationDelay);
        photonView.RPC("EnableGrenade", RpcTarget.All);
    }

    IEnumerator ExplodeDelayed()
    {
        yield return new WaitForSeconds(explosionDelay);
        photonView.RPC("Explode", RpcTarget.All);
    }

    [PunRPC]
    void EnableGrenade()
    {
        //activated = true;
        Display.SetActive(true);
    }

    [PunRPC]
    void GrenadeSound()
    {
        //activated = true;
        audioSource.Play();
    }

    [PunRPC]
    [System.Obsolete]
    void Explode()
    {
        explosionEffect.SetActive(true);
        if (Type == "Prox")
        {
            playerHealth = player.GetComponent<PlayerHealth>();
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag(EnemyTag))
                {
                    float distance = Vector3.Distance(transform.position, collider.transform.position);
                    int damage = (int)((1f - distance / explosionRadius) * maxDamage);
                    FollowAI enemyDamageCrit = collider.GetComponent<FollowAI>();
                    if (enemyDamageCrit.Health <= damage && enemyDamageCrit.alive == true && playerHealth != null)
                    {
                        playerHealth.EnemyKilled("Normal");
                        enemyDamageCrit.TakeDamage(damage);
                    }
                    else if (enemyDamageCrit.Health > damage && enemyDamageCrit.alive == true && playerHealth != null)
                    {
                        enemyDamageCrit.TakeDamage(damage);
                    }
                }

                else if (collider.CompareTag(BossEnemyTag))
                {
                    float distance = Vector3.Distance(transform.position, collider.transform.position);
                    int damage = (int)((1f - distance / explosionRadius) * maxDamage);
                    FollowAI enemyDamageCrit = collider.GetComponent<FollowAI>();
                    if (enemyDamageCrit.Health <= damage && enemyDamageCrit.alive == true && playerHealth != null)
                    {
                        playerHealth.EnemyKilled("BossEnemy");
                        enemyDamageCrit.TakeDamage(damage);
                    }
                    else if (enemyDamageCrit.Health > damage && enemyDamageCrit.alive == true && playerHealth != null)
                    {
                        enemyDamageCrit.TakeDamage(damage);
                    }
                }

                else if (collider.CompareTag(SecurityTag))
                {
                    float distance = Vector3.Distance(transform.position, collider.transform.position);
                    int damage = (int)((1f - distance / explosionRadius) * maxDamage);
                    DroneHealth enemyDamageCrit = collider.GetComponent<DroneHealth>();
                    if (enemyDamageCrit != null)
                        enemyDamageCrit.TakeDamage(damage);
                    else
                    {
                        SentryDrone enemyDamageCrit2 = collider.GetComponent<SentryDrone>();
                        enemyDamageCrit2.TakeDamage(damage);
                    }
                }

                else if (collider.CompareTag(PlayerTag))
                {
                    float distance = Vector3.Distance(transform.position, collider.transform.position);
                    int damage = (int)((1f - distance / explosionRadius) * maxDamage);
                    PlayerHealth enemyDamageCrit = collider.GetComponent<PlayerHealth>();
                    if (enemyDamageCrit.Health <= damage && enemyDamageCrit.alive == true && playerHealth != null && collider.transform.root.gameObject != player)
                    {
                        playerHealth.PlayersKilled();
                        enemyDamageCrit.TakeDamage(damage);
                    }
                    else if (enemyDamageCrit.Health > damage && enemyDamageCrit.alive == true && playerHealth != null)
                    {
                        enemyDamageCrit.TakeDamage(damage);
                    }
                }
            }

            StartCoroutine(Destroy(0.2f));
        }
        else if (Type == "Smoke")
            StartCoroutine(Destroy(15));
    }

    IEnumerator Destroy(float delay)
    {
        yield return new WaitForSeconds(delay);
        // Destroy the grenade object across the network
        photonView.RPC("DestroyGrenade", RpcTarget.All);
    }

    [PunRPC]
    void DestroyGrenade()
    {
        // Destroy the grenade object locally
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    void RPC_Trigger()
    {
        if (rb.useGravity == false)
            rb.useGravity = true;
        if (rb.isKinematic == true)
            rb.isKinematic = false;
    }
}
