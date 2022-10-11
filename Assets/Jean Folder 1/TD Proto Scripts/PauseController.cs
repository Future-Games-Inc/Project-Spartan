using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseController : MonoBehaviour
{
    //Creating a variable object thingy that contains InputActionReference of the PauseButton we will assign. 
    public InputActionReference pauseButton = null;

    // Subscribing 
    private void Awake()
    {
        pauseButton.action.performed += pauseButtonMethod;
    }
    //  Desubscritbing
    private void OnDestroy()
    {
        pauseButton.action.performed -= pauseButtonMethod;
    }

    void Update()
    {
        //if the action is perfomed. 
        if(pauseButton.action.IsPressed())
            // We will change the game state 
        {
            GameState currentGameState = GameStateManager.Instance.currentGameState;
            GameState newGameState = currentGameState == GameState.Gameplay ? GameState.Paused : GameState.Gameplay;

            GameStateManager.Instance.SetState(newGameState);
        }
    }

    private void pauseButtonMethod(InputAction.CallbackContext obj)
    {
        throw new NotImplementedException();
    }

}
