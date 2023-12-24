//Copyright 2022, Infima Games. All Rights Reserved.

using UnityEngine;
using System.Collections;

namespace InfimaGames.LowPolyShooterPack.Legacy
{
    public class ExplosiveBarrelScript : MonoBehaviour
    {

        float randomTime;
        bool routineStarted = false;

        //Used to check if the barrel 
        //has been hit and should explode 
        public bool explode = false;

        [Header("Prefabs")]
        //The explosion prefab
        public GameObject explosionPrefab;
        //The destroyed barrel prefab
        public GameObject destroyedBarrelPrefab;

        [Header("Customizable Options")]
        //Minimum time before the barrel explodes
        public float minTime = 0.05f;
        //Maximum time before the barrel explodes
        public float maxTime = 0.25f;

        [Header("Explosion Options")]
        //How far the explosion will reach
        public float explosionRadius = 10f;

        public float Health = 100;

        private void Update()
        {
            //Generate random time based on min and max time values
            randomTime = Random.Range(minTime, maxTime);

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
        }

        private IEnumerator Explode()
        {
            //Wait for set amount of time
            yield return new WaitForSeconds(randomTime);

            //Spawn the destroyed barrel prefab
            Instantiate(destroyedBarrelPrefab, transform.position,
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

                //Raycast downwards to check the ground tag
                RaycastHit checkGround;
                if (Physics.Raycast(transform.position, Vector3.down, out checkGround, 50))
                {
                    //Instantiate explosion prefab at hit position
                    Instantiate(explosionPrefab, checkGround.point,
                        Quaternion.FromToRotation(Vector3.forward, checkGround.normal));
                }

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
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
        }
    }
}