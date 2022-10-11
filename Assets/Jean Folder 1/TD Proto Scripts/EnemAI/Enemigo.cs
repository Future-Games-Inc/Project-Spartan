using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemigo : PoolableObject , iDamageable
{
    public AttackRadius AttackRadius;
    public Animator Animator;

    public EnemyMovement movement;
    public NavMeshAgent agent;

    [Header("Enemy Health")]
    public float start_Health = 100;
    public float health;
    public Image healthBar;
    [Header("Enemy Death Effects")]
    public GameObject deathEffect;
    public int moneyGained = 50;

    private Coroutine lookCoroutine;
    private const string Attack_Trigger = "Attack";

    public delegate void DeathEvent (Enemigo enemy);
    public DeathEvent OnDie;

    private void Awake()
    {
        health = start_Health;
        AttackRadius.OnAttack += OnAttack;
    }

    private void Start()
    {
        //health = start_Health;
        Debug.Log("Start health is set at" + start_Health);
    }

    private void OnAttack(iDamageable target)
    {
        Animator.SetTrigger(Attack_Trigger);
        Debug.Log("Animator Trigger should have ran on attack of enemigo script ran. ");
      if(lookCoroutine != null)
        {

            StopCoroutine(lookCoroutine);
        }
        lookCoroutine = StartCoroutine(LookAt(target.GetTransform()));

    }

    private IEnumerator LookAt(Transform target)
    {
        Quaternion lookRotation  = Quaternion.LookRotation(target.position - transform.position);
        float time = 0;

        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, time);

            time += Time.deltaTime * 2;
            yield return null;
        }

        transform.rotation = lookRotation;

    }




    public override void OnDisable()
    {
        base.OnDisable();

        agent.enabled = false;
    }


    public Transform GetTransform()
    {
        return transform;

    }

    public void TakeDamage(int Damage)
    {
        health -= Damage;
        //start_Health -= Damage;
        healthBar.fillAmount = health / start_Health;

        if (health <= 0)
        {
            // Linking to wave spawner 
            OnDie?.Invoke(this);

            // Death Sequence
            //instantiate death VFX
            GameObject death_Effect = (GameObject)Instantiate(deathEffect, transform.position, Quaternion.identity);
            //Destroys the death effect
            Destroy(death_Effect, 5f);

         

            // Link to player stats money 
            PlayerStats.Money += moneyGained;

            // setting the game object to false. 
            gameObject.SetActive(false);
        }

    }
}
