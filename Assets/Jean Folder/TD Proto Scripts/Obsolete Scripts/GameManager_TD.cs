using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager_TD : MonoBehaviour
{
    public UI_Toggler_TD toggle;
    public static bool GameIsOver;
    public GameObject gameOverUI;
    public GameObject Winning_UI;
    public PausedMenue Pause;
 

    public string nextLevel = "Level02";
    public int levelToUnlock = 1;

    public SceneFader sceneFader;



     void Awake()
    {
        GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }
     void OnDestroy()
    {
        GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }
    private void OnGameStateChanged(GameState newGameState)
    {
        enabled = newGameState == GameState.Gameplay;
    }


    void Start()
    {
        GameIsOver = false;
    }



    void Update()
    {
        if(GameIsOver)
        {
            return;
        }

        if(PlayerStats.Lives <= 0)
        {
            EndGame();
        }

    }

    void EndGame()
    {
        GameIsOver = true;
        Debug.Log("Game Over!");

        
        toggle.GameWonOrLose();
        gameOverUI.SetActive(true);
        Time.timeScale = 0f;

    }

    public void WinLevel()
    {
        Debug.Log("Level Won!!!");
        PlayerPrefs.SetInt("levelReached", levelToUnlock);
        //sceneFader.FadeTo(nextLevel);
        toggle.GameWonOrLose();
        Winning_UI.SetActive(true);
        Time.timeScale = 0f;

    }



}



   
