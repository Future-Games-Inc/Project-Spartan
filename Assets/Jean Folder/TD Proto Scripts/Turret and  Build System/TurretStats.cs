using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretStats : CharacterStats, iDamageable
{
    public GameObject deathEffect;
    public HealthBar healthBar;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        maxHealth = SetMaxHealthFromHealthLevel();

    }

    void Update()
    {
    }

    private int SetMaxHealthFromHealthLevel()
    {
        // TODO: Create Formula to improve health upon level up of hits character. int 10 can be changed. 
        maxHealth = healthLevel * 10;
        return maxHealth;
    }


    public void TakeDamage(int Damage)
    {
        currentHealth = currentHealth - Damage;
        healthBar.SetCurrentHealth(currentHealth);

        if (currentHealth <= 0)
        {

            // Death Sequence
            //instantiate death VFX
            GameObject death_Effect = (GameObject)Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(death_Effect, 5f);
            gameObject.SetActive(false);
            Destroy(gameObject);

            //Destroys the death effect
            // setting the game object to false. 

        }

    }

    public Transform GetTransform()
    {
        return transform;
    }
}

