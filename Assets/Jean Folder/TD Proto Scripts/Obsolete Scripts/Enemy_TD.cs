using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy_TD : MonoBehaviour
{
    public float speed = 10f;

    private Transform target;
    private int wavepointIndex = 0;

    public float startHealth = 100;
    private float health;

    public int moneyGained = 50;


    public GameObject deathEffect; 

    [Header("Health Bar Dont Remove")]
    public Image healthBar;

    private void Awake()
    {
        GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }

   


    private void Start()
    {
        target = Waypoints.points[0];
        health = startHealth;
    }

    public void TakeDamage(int amount)
    {
        health -= amount;

        healthBar.fillAmount = health / startHealth;

        if(health <=0)
        {
            Die();
        }

    }

    private void Die()
    {
        //TODO: Attach an enemy Death effect
        GameObject death_Effect = (GameObject) Instantiate(deathEffect, transform.position, Quaternion.identity); 
        Destroy(death_Effect, 5f);
        PlayerStats.Money += moneyGained;
        Debug.Log("Money gained1");


        //keeping track of enemies alive by calling wave spawner script. 
        WaveSpawner.enemiesAlive--;
        Destroy(gameObject);
    }

     void Update()
    {

        Vector3 direction = target.position - transform.position; //direction vector
        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);

        if(Vector3.Distance(transform.position, target.position) <= 0.4f)
        {
            GetNextWayPoiint();

        }


    }

    void GetNextWayPoiint()
    {
        if(wavepointIndex >= Waypoints.points.Length - 1)
        {
            EndPath();
            return;

        }

        wavepointIndex++;
        target = Waypoints.points[wavepointIndex];

    }

    void  EndPath()
    {

        PlayerStats.Lives--;
        //keeping track of enemies alive by calling wave spawner script. 
        WaveSpawner.enemiesAlive--;
        Destroy(gameObject);
    }


    private void OnGameStateChanged( GameState newGameState)
    {
        enabled = newGameState == GameState.Gameplay;
    }

}
