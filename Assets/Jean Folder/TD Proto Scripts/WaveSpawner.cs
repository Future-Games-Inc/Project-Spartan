using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveSpawner : MonoBehaviour
{
    public static int enemiesAlive = 0;

    public Wave_TD[] waves;

    public Transform enemyPrefab;
    public Transform spawnPoint;

    public TMP_Text waveCountDownText;

    public float timeBetweenWaves = 5f;
    public float countDown = 4f;
    public float enemInLineWaitTime = 0.5f;

    public int waveIndex = 0;

    public GameManager_TD gameManager_TD;

    private void Awake()
    {
        GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState newGameState)
    {
        enabled = newGameState == GameState.Gameplay;
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // if enemies are still alive return 
        if(enemiesAlive > 0)
        {
            return;
        }


        // when all enemies are dead continue with count down for the next wave. 
        if(countDown <= 0f)
        {
            StartCoroutine(SpawnWave());
            countDown = timeBetweenWaves;
            return;
        }
        countDown -= Time.deltaTime;

        countDown = Math.Clamp(countDown, 0f, Mathf.Infinity);

        waveCountDownText.text = countDown.ToString("f2");


    }

    IEnumerator SpawnWave()
    {
        PlayerStats.Rounds++;

        Wave_TD wave = waves[waveIndex];

        enemiesAlive = wave.count;

        for (int i = 0; i < wave.count ; i++)
        {
            SpawnEnemy(wave.enemyPrefab);
            yield return new WaitForSeconds(1f/wave.rate);
        }
        waveIndex++;

        if(waveIndex == waves.Length)
        {
            Debug.Log("YOU HAVE SURVIDED");

            gameManager_TD.WinLevel();
            //TODO: Link this to a UI to display on the hand UI that you have completd the level. 
            this.enabled = false;

        }

    }

     void SpawnEnemy(GameObject enemyPrefab)
    {
        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

    }
}
