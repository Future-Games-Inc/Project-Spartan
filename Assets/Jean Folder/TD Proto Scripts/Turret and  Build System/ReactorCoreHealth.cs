using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactorCoreHealth : CharacterStats, iDamageable
{
    [SerializeField] private PlayerStats player;

    [Header("Reactor Core Material Change")]
    public MeshRenderer reactorMesh;
    public Material OriginalStatus;
    public Material MediumStatus;
    public Material CriticalStatus;
    [Header("Reactor Core Partical Effects")]
    public ParticleSystem electricDistargEffect;
    public ParticleSystem ExplosionEffect;
    MeshRenderer meshRenderer;
    private float atHalfHealth;
    private float quarterHealth;

    [SerializeField] private GameObject reactorSpawnPoint;
    [SerializeField] private Transform reactor;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindObjectOfType<PlayerStats>();

        maxHealth = SetMaxHealthFromHealthLevel();
        currentHealth = maxHealth;

        reactorMesh = GetComponent<MeshRenderer>();
        atHalfHealth = maxHealth / 2;
        quarterHealth = maxHealth / 4;
    }

    // Update is called once per frame
    void Update()
    {
        CheckHealthStatus();
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
        if (currentHealth <= atHalfHealth)
        {
            reactorMesh.material = MediumStatus;
            electricDistargEffect.Play();
        }
        if (currentHealth <= quarterHealth)
        {
            reactorMesh.material = CriticalStatus;
        }

        if (currentHealth <= 0)
        {
            ReactorDeath();

            player.lifeLost();
            player.PlayerDeath();


        }
    }

    private void ReactorDeath()
    {
        ExplosionEffect.Play();

        reactor.transform.position = reactorSpawnPoint.transform.position;
        electricDistargEffect.Stop();
        ExplosionEffect.Stop();
        maxHealth = SetMaxHealthFromHealthLevel();
        currentHealth = maxHealth;
        reactorMesh.material = OriginalStatus;
    }


}

