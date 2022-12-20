using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerStats : CharacterStats, iDamageable
{
    public static int Money;
    [Header("Cash")]
    public int startMoney = 400;
    public static int Lives;
    [Header("Lives")]
    public int startLives = 3;

    public HealthBar healthBar;
    [Header("Wave Rounds Completed")]
    public static int Rounds;

    [SerializeField] private Transform player;
    [SerializeField] private GameObject reactorLocation;
    [SerializeField] private Vector3 reactorSpawnPointOffset;

    private void Start()
    {
        healthBar = FindObjectOfType<HealthBar>();
        maxHealth = SetMaxHealthFromHealthLevel();
        healthBar.SetMaxHealth(maxHealth);
        currentHealth = maxHealth;
        Money = startMoney;
        Lives = startLives;


        Rounds = 0;
    }

    private void Update()
    {

    }

    private int SetMaxHealthFromHealthLevel()
    {
        // TODO: Create Formula to improve health upon level up of character. int 10 can be changed. 
        maxHealth = healthLevel * 10;
        return maxHealth;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("EnemyBullet"))
        {
            TakeDamage(5);
            CheckHealthStatus();

        }
    }

    public void TakeDamage(int Damage)
    {
        currentHealth = currentHealth - Damage;


        CheckHealthStatus();

    }

    private void CheckHealthStatus()
    {
        healthBar.SetCurrentHealth(currentHealth);
        if (currentHealth <= 0)
        {
            Lives -= 1;

            // Respawn new pillar or set point
            PlayerDeath();

            // trigger a penalty?
            if (Lives == 0)
            {
                //TODO: Trigger Game Over

            }

        }
    }

    public void PlayerDeath()
    {
        player.transform.position = reactorLocation.transform.position + reactorSpawnPointOffset;
        maxHealth = SetMaxHealthFromHealthLevel();
        healthBar.SetMaxHealth(maxHealth);
        currentHealth = maxHealth;
    }

    public void lifeLost()
    {
        Lives = Lives - 1;
        currentHealth = 0;
    }

    public Transform GetTransform()
    {

        return transform;
    }
}