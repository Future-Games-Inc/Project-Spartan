using System.Collections;
using Umbrace.Unity.PurePool;
using UnityEngine;

namespace BNG
{
    /// <summary>
    /// A Grabbable object that can stick to objects and deal damage
    /// </summary>
    public class ArrowNet : MonoBehaviour
    {
        Rigidbody rb;
        Grabbable grab;
        public bool Flying = false;
        public float ZVel = 0;

        public TargetInfo[] Targets;

        public float explosionRadius = 5f;
        public int maxDamage = 30;
        public int maxEMPDamage = 20;
        public float explosionDelay = 2f;
        public float EMPDelay = 3f;

        public GameObject player;
        public GameObject explosionEffect;

        public Collider ShaftCollider;
        AudioSource impactSound;

        float flightTime = 0f;
        float destroyTime = 5f; // Time in seconds to destroy arrow
        Coroutine queueDestroy;

        public Projectile ProjectileObject;
        public GameObject bubble;

        public GameObject arrowOwner;
        public GameObject surgeBubble;

        // Get this value from the ProjectileObject
        float arrowDamage;

        public PlayerHealth playerHealth;
        public bool activated = false;

        public string Type = "Default";


        public struct TargetInfo
        {
            public string Tag;
        }

        // Start is called before the first frame update
        void OnEnable()
        {
            

            Targets = new TargetInfo[3];  // Initializing the array with 3 elements
            Targets[0] = new TargetInfo { Tag = "Enemy" };  // Populating individual elements
            Targets[1] = new TargetInfo { Tag = "BossEnemy" };
            Targets[2] = new TargetInfo { Tag = "Security" };

            rb = GetComponent<Rigidbody>();
            impactSound = GetComponent<AudioSource>();
            ShaftCollider = GetComponent<Collider>();
            grab = GetComponent<Grabbable>();

            if (ProjectileObject == null)
            {
                ProjectileObject = gameObject.AddComponent<Projectile>();
                ProjectileObject.Damage = 15;
                ProjectileObject.StickToObject = true;
                ProjectileObject.enabled = false;
            }

            arrowDamage = ProjectileObject.Damage;
        }

        void FixedUpdate()
        {

            bool beingHeld = grab != null && grab.BeingHeld;

            // Align arrow with velocity
            if (!beingHeld && rb != null && rb.velocity != Vector3.zero && Flying && ZVel > 0.02)
            {
                rb.rotation = Quaternion.LookRotation(rb.velocity);
            }

            ZVel = transform.InverseTransformDirection(rb.velocity).z;

            if (Flying)
            {
                gameObject.GetComponent<TrailRenderer>().enabled = true;
                flightTime += Time.fixedDeltaTime;
            }

            // Cancel Destroy if we just picked this up
            if (queueDestroy != null && grab != null && grab.BeingHeld)
            {
                StopCoroutine(queueDestroy);
            }
        }

        public void ShootArrow(Vector3 shotForce)
        {

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

        IEnumerator QueueDestroy()
        {
            yield return new WaitForSeconds(destroyTime);

            if (grab != null && !grab.BeingHeld && transform.parent == null)
            {
                Destroy(this.gameObject);
            }
        }

        IEnumerator ReEnableCollider()
        {

            // Wait a few frames before re-enabling collider on bow shaft
            // This prevents the arrow from shooting ourselves, the bow, etc.
            // If you want the arrow to still have physics while attached,
            // parent a collider to the arrow near the tip
            int waitFrames = 3;
            for (int x = 0; x < waitFrames; x++)
            {
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
            Destroy(gameObject);
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

            if (Type == "Bomb")
            {
                StartCoroutine(ExplodeDelayed());
            }
            else if (Type == "EMP")
            {
                StartCoroutine(EMPDelayed());
            }
        }

        void playSoundInterval(float fromSeconds, float toSeconds)
        {
            if (impactSound)
            {

                if (impactSound.isPlaying)
                {
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
                DefaultDamageEnemy(other, arrowDamage);
            }

            if ((other.CompareTag("BossEnemy")))
            {
                // select custom functions for damage
                DefaultDamageBossEnemy(other, arrowDamage);
            }

            if (other.CompareTag("Security"))
            {
                // select custom functions for damage
                DefaultDamageSecurity(other, arrowDamage);
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
                if (Type == "Default")
                    Destroy(gameObject);
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
                if (Type == "Default")
                    Destroy(gameObject);
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
                if (Type == "Default")
                    Destroy(gameObject);
            }
        }

        int CalculateDamage(float distance)
        {
            return (int)((1f - distance / explosionRadius) * maxDamage);
        }

        int CalculateEMPDamage(float distance)
        {
            return (int)((1f - distance / explosionRadius) * maxEMPDamage);
        }

        public IEnumerator ExplodeDelayed()
        {
            yield return new WaitForSeconds(explosionDelay);
            explosionEffect.SetActive(true);

            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
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

            float delay = 2f;
            StartCoroutine(Destroy(delay));
        }

        public IEnumerator EMPDelayed()
        {
            yield return new WaitForSeconds(EMPDelay);
            explosionEffect.SetActive(true);
            StartCoroutine(GrowBubbleCoroutine());
            StartCoroutine(DestroyBubbleCoroutine());

            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (var target in Targets)
            {
                foreach (Collider collider in colliders)
                {
                    if (collider.CompareTag(target.Tag))
                    {
                        HandleEMP(collider, target);
                    }
                }
            }

            float delay = 2f;
            StartCoroutine(Destroy(delay));
        }

        IEnumerator GrowBubbleCoroutine()
        {
            surgeBubble = Instantiate(bubble, transform.position, Quaternion.identity);
            while (true)
            {
                float scaleIncrease = 8f * Time.deltaTime;
                surgeBubble.transform.localScale += new Vector3(scaleIncrease, scaleIncrease, scaleIncrease);

                yield return null;
            }
        }

        IEnumerator DestroyBubbleCoroutine()
        {
            yield return new WaitForSeconds(1.25f);
            StopCoroutine(GrowBubbleCoroutine());
            Destroy(surgeBubble);
        }

        void HandleDamage(Collider collider, TargetInfo target)
        {
            float distance = Vector3.Distance(transform.position, collider.transform.position);
            int damage = CalculateDamage(distance);

            // Based on the tag, handle damage appropriately. This is a simplified version, extend as needed.
            switch (target.Tag)
            {
                case "Enemy":
                    // Handle enemy damage
                    FollowAI enemyDamageCrit = collider.GetComponentInParent<FollowAI>();
                    if (enemyDamageCrit.Health <= damage && enemyDamageCrit.alive == true && playerHealth != null)
                    {
                        playerHealth.EnemyKilled("Normal");
                        enemyDamageCrit.TakeDamage(damage);
                    }
                    else if (enemyDamageCrit.Health > damage && enemyDamageCrit.alive == true && playerHealth != null)
                    {
                        enemyDamageCrit.TakeDamage(damage);
                    }
                    break;
                case "BossEnemy":
                    // Handle boss enemy damage
                    FollowAI BossenemyDamageCrit = collider.GetComponentInParent<FollowAI>();
                    if (BossenemyDamageCrit.Health <= damage && BossenemyDamageCrit.alive == true && playerHealth != null)
                    {
                        playerHealth.EnemyKilled("BossEnemy");
                        BossenemyDamageCrit.TakeDamage(damage);
                    }
                    else if (BossenemyDamageCrit.Health > damage && BossenemyDamageCrit.alive == true && playerHealth != null)
                    {
                        BossenemyDamageCrit.TakeDamage(damage);
                    }
                    break;
                case "Security":
                    // Handle security damage
                    DroneHealth DroneenemyDamageCrit = collider.GetComponentInParent<DroneHealth>();
                    if (DroneenemyDamageCrit != null)
                        DroneenemyDamageCrit.TakeDamage(damage);
                    else
                    {
                        SentryDrone SentryenemyDamageCrit2 = collider.GetComponentInParent<SentryDrone>();
                        SentryenemyDamageCrit2.TakeDamage(damage);
                    }
                    break;
            }
        }

        void HandleEMP(Collider collider, TargetInfo target)
        {
            float distance = Vector3.Distance(transform.position, collider.transform.position);
            int damage = CalculateEMPDamage(distance);

            // Based on the tag, handle damage appropriately. This is a simplified version, extend as needed.
            switch (target.Tag)
            {
                case "Enemy":
                    // Handle enemy damage
                    FollowAI enemyDamageCrit = collider.GetComponentInParent<FollowAI>();
                    if (enemyDamageCrit.Health <= damage && enemyDamageCrit.alive == true && playerHealth != null)
                    {
                        playerHealth.EnemyKilled("Normal");
                        enemyDamageCrit.TakeDamage(damage);
                    }
                    else if (enemyDamageCrit.Health > damage && enemyDamageCrit.alive == true && playerHealth != null)
                    {
                        enemyDamageCrit.TakeDamage(damage);
                        enemyDamageCrit.EMPShock();
                    }
                    break;
                case "BossEnemy":
                    // Handle boss enemy damage
                    FollowAI BossenemyDamageCrit = collider.GetComponentInParent<FollowAI>();
                    if (BossenemyDamageCrit.Health <= damage && BossenemyDamageCrit.alive == true && playerHealth != null)
                    {
                        playerHealth.EnemyKilled("BossEnemy");
                        BossenemyDamageCrit.TakeDamage(damage);
                    }
                    else if (BossenemyDamageCrit.Health > damage && BossenemyDamageCrit.alive == true && playerHealth != null)
                    {
                        BossenemyDamageCrit.TakeDamage(damage);
                        BossenemyDamageCrit.EMPShock();
                    }
                    break;
                case "Security":
                    // Handle security damage
                    DroneHealth DroneenemyDamageCrit = collider.GetComponentInParent<DroneHealth>();
                    if (DroneenemyDamageCrit != null)
                        DroneenemyDamageCrit.TakeDamage(damage * 2);
                    else
                    {
                        SentryDrone SentryenemyDamageCrit2 = collider.GetComponentInParent<SentryDrone>();
                        SentryenemyDamageCrit2.TakeDamage(damage * 2);
                    }
                    break;
            }
        }
    }
}
