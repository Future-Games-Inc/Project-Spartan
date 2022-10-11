using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactorCoreHealth : CharacterStats, iDamageable
{
    [SerializeField]private PlayerStats player;

    [Header("Reactor Core Material Change")]
    public Material OriginalStatus;
    public Material MediumStatus;
    public Material CriticalStatus;
    [Header("Reactor Core Partical Effects")]
    public ParticleSystem electricDistargEffect;
    public ParticleSystem ExplosionEffect;
    MeshRenderer meshRenderer;
    private float atHalfHealth;
    private float quarterHealth;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindObjectOfType<PlayerStats>();

        maxHealth = SetMaxHealthFromHealthLevel();
        currentHealth = maxHealth;

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        atHalfHealth = maxHealth / 2;
        quarterHealth = maxHealth / 4;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    Transform iDamageable.GetTransform()
    {
        return transform;
    }

    private int SetMaxHealthFromHealthLevel()
    {
        // TODO: Create Formula to improve health upon level up of character. int 10 can be changed. 
        maxHealth = healthLevel * 10;
        return maxHealth;
    }


    void iDamageable.TakeDamage(int Damage)
    {
        currentHealth = currentHealth - Damage;

        CheckHealthStatus();
    }

    private void CheckHealthStatus()
    {
        if (currentHealth == atHalfHealth)
        {
            meshRenderer.material = MediumStatus;
            electricDistargEffect.Play();
        }
        if (currentHealth == quarterHealth)
        {
            meshRenderer.material = CriticalStatus;
        }

        if (currentHealth <= 0)
        {


            player.lifeLost();

            ExplosionEffect.Play();

            // Move Game object to previous position

            // gameObject.SetActive(false);

            //Destroy(gameObject);
            // Respawn new pillar or set point

            // restart level


            // trigger a penalty?

        }
    }
}
