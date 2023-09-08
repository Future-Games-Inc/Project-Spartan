using InfimaGames.LowPolyShooterPack.Legacy;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BombScript : MonoBehaviour
{
    public bool explode = false;
    public bool routineStarted = false;
    public bool activated;

    public float elapsedTime;
    public float activationTime;
    public float radius;

    public Slider activationSlider;

    public GameObject smokePrefab;

    //How far the explosion will reach
    public float explosionRadius = 10f;

    public float Health = 100;

    public SpawnManager1 enemyCounter;

    private bool playerWithinRadius = false; // Track player's presence within the trigger


    // Start is called before the first frame update
    void OnEnable()
    {
        activationSlider.maxValue = activationTime;
        activationSlider.value = activationTime;
        enemyCounter = GameObject.FindGameObjectWithTag("spawnManager").GetComponent<SpawnManager1>();
    }

    // Update is called once per frame
    void Update()
    {
        //If the barrel is hit
        if (explode == true)
        {
            if (routineStarted == false)
            {
                //Start the explode coroutine
                StartCoroutine(Explode());
                routineStarted = true;
            }
        }

        // If the player is within the trigger, continuously update the vault activation
        if (playerWithinRadius)
        {
            if (!activated && playerWithinRadius)
            {
                elapsedTime += Time.deltaTime;
                float remainingTime = activationTime - elapsedTime;
                activationSlider.value = remainingTime;
                if (elapsedTime >= activationTime)
                {
                    activated = true;
                    Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
                    foreach (Collider collider in hitColliders)
                    {
                        if (collider.CompareTag("Player"))
                        {
                            collider.GetComponent<PlayerHealth>().BombNeutralized();
                            Destroy(gameObject);
                        }
                    }
                }
            }
            else if (!playerWithinRadius)
            {
                // Reset activation if the player is not within the trigger
                activated = false;
                elapsedTime = 0f;
                activationSlider.value = activationTime;
            }
        }
    }

    private IEnumerator Explode()
    {
        //Wait for set amount of time
        yield return new WaitForSeconds(0.25f);

        //Spawn the destroyed barrel prefab
        Instantiate(smokePrefab, transform.position,
                     transform.rotation);

        //Explosion force
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
        foreach (Collider hit in colliders)
        {
            //If the barrel explosion hits other barrels with the tag "ExplosiveBarrel"
            if (hit.transform.tag == "ExplosiveBarrel")
            {
                //Toggle the explode bool on the explosive barrel object
                hit.transform.gameObject.GetComponent<ExplosiveBarrelScript>().explode = true;
            }

            //If the explosion hit the tag "Target"
            //Toggle the isHit bool on the target object
            if (hit.gameObject.CompareTag("Enemy") || hit.gameObject.CompareTag("BossEnemy"))
            {
                FollowAI enemyHealth = hit.gameObject.GetComponent<FollowAI>();
                {
                    if (enemyHealth != null)
                        enemyHealth.TakeDamage(25);
                }
            }

            if (hit.gameObject.CompareTag("Security"))
            {
                DroneHealth droneEnemyHealth = hit.gameObject.GetComponent<DroneHealth>();
                {
                    if (droneEnemyHealth != null)
                        droneEnemyHealth.TakeDamage(30);
                    else
                    {
                        SentryDrone sentryDroneHealth = hit.gameObject.GetComponent<SentryDrone>();
                        sentryDroneHealth.TakeDamage(30);
                    }
                }
            }

            if (hit.gameObject.CompareTag("Player"))
            {
                PlayerHealth playerhealth = hit.gameObject.GetComponentInChildren<PlayerHealth>();
                {
                    if (playerhealth != null)
                        playerhealth.TakeDamage(15);
                }
            }

            //If the explosion hit the tag "GasTank"
            if (hit.GetComponent<Collider>().tag == "GasTank")
            {
                //If gas tank is within radius, explode it
                hit.gameObject.GetComponent<GasTankScript>().isHit = true;
                hit.gameObject.GetComponent<GasTankScript>().explosionTimer = 0.05f;
            }

            //enemyCounter.UpdateBombs();
            //Destroy the current barrel object
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyBullet") || other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
            Health -= 25;

            if (Health <= 0)
            {
                explode = true;
            }
        }

        if(other.CompareTag("Player"))
        {
            playerWithinRadius = true;
            if (!activated && playerWithinRadius)
            {
                elapsedTime += Time.deltaTime;
                float remainingTime = activationTime - elapsedTime;
                activationSlider.value = remainingTime;
                if (elapsedTime >= activationTime)
                {
                    activated = true;
                    Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
                    foreach (Collider collider in hitColliders)
                    {
                        if (collider.CompareTag("Player"))
                        {
                            collider.GetComponent<PlayerHealth>().BombNeutralized();
                            Destroy(gameObject);
                        }
                    }
                }
            }
            else if (!playerWithinRadius)
            {
                // Reset activation if the player is not within the trigger
                activated = false;
                elapsedTime = 0f;
                activationSlider.value = activationTime;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerWithinRadius = false;
            if (!activated && !playerWithinRadius)
            {
                elapsedTime += Time.deltaTime;
                float remainingTime = activationTime - elapsedTime;
                activationSlider.value = remainingTime;
                if (elapsedTime >= activationTime)
                {
                    activated = true;
                    Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
                    foreach (Collider collider in hitColliders)
                    {
                        if (collider.CompareTag("Player"))
                        {
                            collider.GetComponent<PlayerHealth>().BombNeutralized();
                            Destroy(gameObject);
                        }
                    }
                }
            }
            else if (playerWithinRadius)
            {
                // Reset activation if the player is not within the trigger
                activated = false;
                elapsedTime = 0f;
                activationSlider.value = activationTime;
            }
        }
    }
}
