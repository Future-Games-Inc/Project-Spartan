using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG {
    /// <summary>
    /// A Grabbable object that can stick to objects and deal damage
    /// </summary>
    public class ArrowNet : MonoBehaviourPunCallbacks {
        Rigidbody rb;
        Grabbable grab;
        public bool Flying = false;
        public float ZVel = 0;

        public Collider ShaftCollider;
        AudioSource impactSound;

        float flightTime = 0f;
        float destroyTime = 5f; // Time in seconds to destroy arrow
        Coroutine queueDestroy;

        public Projectile ProjectileObject;

        public GameObject arrowOwner;

        // Get this value from the ProjectileObject
        float arrowDamage;

        public PlayerHealth playerHealth;

        public string Type = "Default";

        // Start is called before the first frame update
        void OnEnable() {
            rb = GetComponent<Rigidbody>();
            impactSound = GetComponent<AudioSource>();
            ShaftCollider = GetComponent<Collider>();
            grab = GetComponent<Grabbable>();

            if(ProjectileObject == null) {
                ProjectileObject = gameObject.AddComponent<Projectile>();
                ProjectileObject.Damage = 50;
                ProjectileObject.StickToObject = true;
                ProjectileObject.enabled = false;
            }

            arrowDamage = ProjectileObject.Damage;
        }

        void FixedUpdate() {

            bool beingHeld = grab != null && grab.BeingHeld;

            // Align arrow with velocity
            if (!beingHeld && rb != null && rb.velocity != Vector3.zero && Flying && ZVel > 0.02) {
                rb.rotation = Quaternion.LookRotation(rb.velocity);
            }

            ZVel = transform.InverseTransformDirection(rb.velocity).z;

            if (Flying) {
                flightTime += Time.fixedDeltaTime;
            }

            // Cancel Destroy if we just picked this up
            if(queueDestroy != null && grab != null && grab.BeingHeld) {
                StopCoroutine(queueDestroy);
            }
        }

        public void ShootArrow(Vector3 shotForce) {

            flightTime = 0f;
            Flying = true;

            transform.parent = null;

            rb.isKinematic = false;
            rb.useGravity = true;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.constraints = RigidbodyConstraints.None;
            rb.AddForce(shotForce, ForceMode.VelocityChange);
            StartCoroutine(ReEnableCollider());
            queueDestroy = StartCoroutine(QueueDestroy());
        }

        IEnumerator QueueDestroy() {
            yield return new WaitForSeconds(destroyTime);

            if (grab != null && !grab.BeingHeld && transform.parent == null) {
                PhotonNetwork.Destroy(this.gameObject);
            }
        }

        IEnumerator ReEnableCollider() {

            // Wait a few frames before re-enabling collider on bow shaft
            // This prevents the arrow from shooting ourselves, the bow, etc.
            // If you want the arrow to still have physics while attached,
            // parent a collider to the arrow near the tip
            int waitFrames = 3;
            for (int x = 0; x < waitFrames; x++) {
                yield return new WaitForFixedUpdate();
            }

            ShaftCollider.enabled = true;
        }

        private void OnCollisionEnter(Collision collision) 
        {
            float zVel = System.Math.Abs(transform.InverseTransformDirection(rb.velocity).z);
            bool doStick = true;
            // Check to stick to object
            if (!rb.isKinematic && Flying)
            {
                if (zVel > 0.02f)
                {
                    if (grab != null && grab.BeingHeld)
                    {
                        grab.DropItem(false, false);
                    }
                    if (doStick)
                    {
                        tryStickArrow(collision);
                    }

                    Flying = false;

                    playSoundInterval(2.462f, 2.68f);
                }
            }

            StartCoroutine(Destroy(destroyTime));
        }

        IEnumerator Destroy(float duration)
        {
            yield return new WaitForSeconds(duration);
            PhotonNetwork.Destroy(gameObject);
            // attach clip and play
            // audioSource.PlayOneShot(clip);
        }

        // Attach to collider
        void tryStickArrow(Collision collision)
        {

            Rigidbody colRigid = collision.collider.GetComponent<Rigidbody>();
            transform.parent = null; // Start out with arrow being in World space

            // If the collider is static then we don't need to do anything. Just stop it.
            if (collision.gameObject.isStatic)
            {
                rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
                rb.isKinematic = true;
            }
            // If the object has a Rigidbody
            else if (colRigid != null)
            {
                // Attach to non-kinematic rigidbody via FixedJoint
                if (!colRigid.isKinematic)
                {
                    FixedJoint joint = gameObject.AddComponent<FixedJoint>();
                    joint.connectedBody = colRigid;
                    joint.enableCollision = false;
                    joint.breakForce = float.MaxValue;
                    joint.breakTorque = float.MaxValue;
                }
                // Parent to kinematic rigidbody if scale is (1,1,1)
                else if (colRigid.isKinematic && collision.transform.localScale == Vector3.one)
                {
                    transform.SetParent(collision.transform);
                    rb.useGravity = false;
                    rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
                    rb.isKinematic = true;
                    rb.constraints = RigidbodyConstraints.FreezeAll;
                    rb.WakeUp();
                }
            }
            // If the object lacks a Rigidbody
            else
            {
                if (collision.transform.localScale == Vector3.one)
                {
                    transform.SetParent(collision.transform);
                    rb.constraints = RigidbodyConstraints.FreezeAll;
                }
                else
                {
                    rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
                    rb.useGravity = false;
                    rb.isKinematic = true;
                }
            }
        }

        void playSoundInterval(float fromSeconds, float toSeconds) {
            if (impactSound) {

                if (impactSound.isPlaying) {
                    impactSound.Stop();
                }

                impactSound.time = fromSeconds;
                impactSound.pitch = Time.timeScale;
                impactSound.Play();
                impactSound.SetScheduledEndTime(AudioSettings.dspTime + (toSeconds - fromSeconds));
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (arrowOwner != null)
            {
                playerHealth = arrowOwner.GetComponentInParent<PlayerHealth>();
            }

            if ((other.CompareTag("Enemy")))
            {
                // select custom functions for damage
                switch (Type)
                {
                    default:
                        DefaultDamageEnemy(other, arrowDamage);
                        break;
                }
            }

            if ((other.CompareTag("BossEnemy")))
            {
                // select custom functions for damage
                switch (Type)
                {
                    default:
                        DefaultDamageBossEnemy(other, arrowDamage);
                        break;
                }
            }

            if (other.CompareTag("Player") && other != arrowOwner)
            {
                // select custom functions for damage
                switch (Type)
                {
                    default:
                        DefaultDamagePlayer(other, arrowDamage);
                        break;
                }
            }

            if (other.CompareTag("Security"))
            {
                // select custom functions for damage
                switch (Type)
                {
                    default:
                        DefaultDamageSecurity(other, arrowDamage);
                        break;
                }
            }
            /// <summary> -------------------------------------------------------------------
            ///                           CUSTOME BULLET FUNCTIONS
            /// </summary> -------------------------------------------------------------------
            void DefaultDamageEnemy(Collider target, float arrowDamage)
            {
                FollowAI enemyDamageReg = target.GetComponent<FollowAI>();
                if (enemyDamageReg.Health <= (arrowDamage) && enemyDamageReg.alive == true && playerHealth != null)
                {
                    playerHealth.EnemyKilled("Normal");
                    enemyDamageReg.TakeDamage((int)arrowDamage);
                }
                else if (enemyDamageReg.Health > (10) && enemyDamageReg.alive == true && playerHealth != null)
                {
                    enemyDamageReg.TakeDamage((int)arrowDamage);
                }
                PhotonNetwork.Destroy(gameObject);
            }

            void DefaultDamageBossEnemy(Collider target, float damage)
            {
                FollowAI enemyDamageReg = target.GetComponent<FollowAI>();
                if (enemyDamageReg.Health <= (arrowDamage) && enemyDamageReg.alive == true && playerHealth != null)
                {
                    playerHealth.EnemyKilled("Boss");
                    enemyDamageReg.TakeDamage((int)arrowDamage);
                }
                else if (enemyDamageReg.Health > (10) && enemyDamageReg.alive == true && playerHealth != null)
                {
                    enemyDamageReg.TakeDamage((int)arrowDamage);
                }
                PhotonNetwork.Destroy(gameObject);
            }

            void DefaultDamagePlayer(Collider target, float damage)
            {
                PlayerHealth enemyDamageReg = target.GetComponent<PlayerHealth>();
                if (enemyDamageReg.Health <= (arrowDamage) && enemyDamageReg.alive == true && playerHealth != null)
                {
                    playerHealth.PlayersKilled();
                }
                enemyDamageReg.TakeDamage((int)arrowDamage);
                PhotonNetwork.Destroy(gameObject);
            }

            void DefaultDamageSecurity(Collider target, float damage)
            {
                DroneHealth enemyDamageReg = target.GetComponent<DroneHealth>();
                if (enemyDamageReg != null)
                    enemyDamageReg.TakeDamage((int)arrowDamage);
                else
                {
                    SentryDrone enemyDamageReg2 = other.GetComponent<SentryDrone>();
                    enemyDamageReg2.TakeDamage((int)arrowDamage);
                }
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
