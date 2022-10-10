using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Bullet_TD_Vr2 : PoolableObject
{
    public SphereCollider collider;
    protected List<iDamageable> Damageables = new List<iDamageable>();
    private Transform target;
    [Header("Bullet Speed")]
    public float speed = 70f;
    [Header("Bullet Impact VFX")]
    public GameObject impactEffect;
    [Header("Bullet Explosion Radius")]
    public float explosionRadius = 0f;
    [Header("Bullet Damage")]
    public int damage = 50;

    public float autoDestroyTime = 5f;
    public Rigidbody rigidbody;

    private const string Disable_Method_Name = "Disable";


    // GameState Pausing Mechanism
    private void Awake()
    {
        GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;
        collider = GetComponent<SphereCollider>();
    }
    // GameState Pausing Mechanism
    private void OnDestroy()
    {
        GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }
    // GameState Pausing Mechanism
    private void OnGameStateChanged(GameState newGameState)
    {
        enabled = newGameState == GameState.Gameplay;
    }

    private void OnTriggerEnter(Collider other)
    {
        iDamageable damageable;

        if (other.TryGetComponent<iDamageable>(out damageable))
        {
            damageable.TakeDamage(damage);

        }
        Disable();
    }

    private void Disable()
    {
        CancelInvoke(Disable_Method_Name);
        rigidbody.velocity = Vector3.zero;
        gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    /// Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (direction.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(direction.normalized * distanceThisFrame, Space.World);
        transform.LookAt(target);


    }


    void HitTarget()
    {
        //Damageables.TakeDamage(Damage);


        //casting the impact effect onto the bullet and then deleting it after 2seconds
        GameObject effectInstance = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);
        //Destroy the effect particle
        Destroy(effectInstance, 5f);

        if (explosionRadius > 0f)
        {
            Explode();

        }
        else
        {
            Damage(target);
        }


        //Destroy bullet
        Destroy(gameObject);
    }

    void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.tag == "Enemy")
            {
                Damage(collider.transform);
                //Damageables.TakeDamage(Damage);
            }
        }
    }

    protected virtual void Damage(Transform enemyGO)
    {
        //Enemy_TD e = enemyGO.GetComponent<Enemy_TD>();
        Enemigo enemigo = enemyGO.GetComponent<Enemigo>();

        if (enemigo != null)
        {
            enemigo.TakeDamage(damage);
            Debug.Log("Enemy should have taken damage");
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }




}
