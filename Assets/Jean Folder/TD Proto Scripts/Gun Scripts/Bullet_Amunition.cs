 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Amunition : MonoBehaviour
{
    public Rigidbody rb;
    public GameObject explosion;
    public LayerMask whatIsEnemies;

    [Range(0f, 1f)]
    public float bounciness;
    public bool useGravity;
    
    //Damage
    public int explostionDamage;
    public float explosionRange;
    // LifeTime
    public int maxCollisions;
    public float maxLifeTime;
    public bool explodeOnTouch = true;


     int collisions;
    PhysicMaterial physicMaterial;

    private Transform target;


    // Start is called before the first frame update
    void Start()
    {
        SetUp();
    }

    // Update is called once per frame

    void Update()
    {
        

        //when to explode;
        if(collisions > maxCollisions)
        {
            Explode();
        }

        // Cound doen lifetime
        maxLifeTime -= Time.deltaTime;
        if(maxLifeTime <= 0)
        {
            Explode();
        }
    }


    public void Seek(Transform _target)
    {
        target = _target;

    }


   

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("On collidion Enter activated when hit the enemy");
        if (collision.collider.CompareTag("Bullet"))
        {
            return;
        }

        collisions++;

        if(collision.collider.CompareTag("Enemy") && explodeOnTouch)
        {
            Explode();
        }
        
    }


    private void Explode()
    {
        if(explosion = null)
        {
            Debug.Log("explosion did equal to null so we poped here ");
            Instantiate(explosion, transform.position, Quaternion.identity);
        }

        Collider[] enemies = Physics.OverlapSphere(transform.position, explosionRange, whatIsEnemies);

        foreach(Collider collider in enemies)
        {
            if(collider.tag == "Enemy")
            {
                //Passing through the game object transform. 
                //Damage(target);

                //passing trhough the collision target 
                Debug.Log("collider list was ran and we are about to damage ");
                Damage(collider.transform);


            }
        }

        //for(int i = 0; i < enemies.Length; i++)
        //{
        //    enemies[i].GetComponent<Enemigo>().TakeDamage(explostionDamage);

        //}
        Invoke("Delay", 0.05f);
         
    }


    void Damage(Transform enemyGo)
    {
        Debug.Log("Damage script was ranned.  ");
        Enemigo enemigo = enemyGo.GetComponent<Enemigo>();

        if(enemigo !=null)
        {
            enemigo.TakeDamage(explostionDamage);

        }

    }


    private void Delay()
    {
        Destroy(gameObject);
    }

    public void SetUp()
    {
        physicMaterial = new PhysicMaterial();
        physicMaterial.bounciness = bounciness;
        physicMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
        physicMaterial.bounceCombine = PhysicMaterialCombine.Maximum;
        //assign material to collider
        GetComponent<SphereCollider>().material = physicMaterial;

        //set gravity
        rb.useGravity = useGravity;

    }



    //adding this part
    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("Ontrigger Enter activated when hit the enemy");
        iDamageable damageable;

        collisions++;

        if (other.CompareTag("Enemy") && explodeOnTouch)
        {
            Debug.Log("Compated the enemy tagg and ran exploded ");



            //Passing through the game object transform. 
            //Damage(target);

            //passing trhough the collision target 
            Debug.Log("collider list was ran and we are about to damage ");
            Damage(other.transform);



        }

        if (other.TryGetComponent<iDamageable>(out damageable))
        {
            Debug.Log("got the i damagable and damaged them. ");
            damageable.TakeDamage(explostionDamage);

        }

    }

}
