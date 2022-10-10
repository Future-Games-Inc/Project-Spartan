using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Toggler_TD : MonoBehaviour
{
    public GameObject playerHealth_UI;
    public GameObject waveCountDown_UI;
    public GameObject livesCountDown_UI;
    public GameObject money_UI;
    public GameObject builderCanvas_UI;

    public bool winLoseLock = false;
    public bool isCanvasOn = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    public void TogglingWristUI()
    {
       
      if(winLoseLock == false)
        {
            isCanvasOn = !isCanvasOn;
            playerHealth_UI.SetActive(isCanvasOn);
            waveCountDown_UI.SetActive(isCanvasOn);
            livesCountDown_UI.SetActive(isCanvasOn);
            money_UI.SetActive(isCanvasOn);
            builderCanvas_UI.SetActive(isCanvasOn);
        }
        else
        {
            return;
        }
       

    }

    public void GameWonOrLose()
    {
        winLoseLock = true;

        
        playerHealth_UI.SetActive(false);
        waveCountDown_UI.SetActive(false);
        livesCountDown_UI.SetActive(false);
        money_UI.SetActive(false);
        builderCanvas_UI.SetActive(false);



    }


}
