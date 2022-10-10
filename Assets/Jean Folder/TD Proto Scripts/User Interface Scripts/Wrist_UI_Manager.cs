using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wrist_UI_Manager : MonoBehaviour
{

    public enum UIStateMethod {Gamewon, GameLoss, MenueScreen, GamePlay}
    public UIStateMethod UI_HandState = UIStateMethod.GamePlay;

    public GameObject playerHealth_UI;
    public GameObject waveCountDown_UI;
    public GameObject livesCountDown_UI;
    public GameObject money_UI;
    public GameObject builder_UI_Canvas;

    public GameObject menu_UI_Canvas;

    public GameObject winScreen_UI_Canvas;
    public GameObject looseScreen_UI_Canvas;

    private bool levelWonComplete;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(UI_HandState == UIStateMethod.GamePlay)
        {
            if(levelWonComplete == true)
            {
                SetHandUIStateTo_GameWonState();
            }
            else
            {
                playerHealth_UI.SetActive(true);
                waveCountDown_UI.SetActive(true);
                livesCountDown_UI.SetActive(true);
                money_UI.SetActive(true);
                builder_UI_Canvas.SetActive(true);

                menu_UI_Canvas.SetActive(false);

                winScreen_UI_Canvas.SetActive(false);
                looseScreen_UI_Canvas.SetActive(false);

            }

        }
        else if(UI_HandState == UIStateMethod.MenueScreen)
        {
            playerHealth_UI.SetActive(false);
            waveCountDown_UI.SetActive(false);
            livesCountDown_UI.SetActive(false);
            money_UI.SetActive(false);
            builder_UI_Canvas.SetActive(false);

            menu_UI_Canvas.SetActive(true);

            winScreen_UI_Canvas.SetActive(false);
            looseScreen_UI_Canvas.SetActive(false);
        }
        else if (UI_HandState == UIStateMethod.GameLoss)
        {
            playerHealth_UI.SetActive(false);
            waveCountDown_UI.SetActive(false);
            livesCountDown_UI.SetActive(false);
            money_UI.SetActive(false);
            builder_UI_Canvas.SetActive(false);

            menu_UI_Canvas.SetActive(false);

            winScreen_UI_Canvas.SetActive(false);
            looseScreen_UI_Canvas.SetActive(true);

        }
        else if (UI_HandState == UIStateMethod.Gamewon)
        {
            playerHealth_UI.SetActive(false);
            waveCountDown_UI.SetActive(false);
            livesCountDown_UI.SetActive(false);
            money_UI.SetActive(false);
            builder_UI_Canvas.SetActive(false);

            menu_UI_Canvas.SetActive(false);

            winScreen_UI_Canvas.SetActive(true);
            looseScreen_UI_Canvas.SetActive(false);


        }
    }



    public void SetHandUIStateTo_GameWonState( )
    {
        
        UI_HandState = UIStateMethod.Gamewon;
        levelWonComplete = true;

    }


    public void SeHandUIStateTo_GameLoss()
    {
        UI_HandState = UIStateMethod.GameLoss;
    }


    public void SeHandUIStateTo_MenueScreen()
    {
        UI_HandState = UIStateMethod.MenueScreen;
    }


    public void SeHandUIStateTo_GamePlay()
    {
        UI_HandState = UIStateMethod.GamePlay;
    }


}
