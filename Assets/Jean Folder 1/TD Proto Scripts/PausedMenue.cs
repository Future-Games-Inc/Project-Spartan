using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PausedMenue : MonoBehaviour
{
    public GameObject userInterFacePauseMenue;
    public string MainMenueScene;
    public string LevelOneScene;
    public bool StartTime;



    public void Toggle()
    {
        userInterFacePauseMenue.SetActive(!userInterFacePauseMenue.activeSelf);

        if (userInterFacePauseMenue.activeSelf)
        {
            
            Time.timeScale = 0f;



        }
        else
        {
            Time.timeScale = 1f;

        }

    }


    public void Retry()
    {

        if(Time.timeScale == 0)
        {
            PlayerStats.Rounds = 0;

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Time.timeScale = 1f;
        }

        else
        {
            PlayerStats.Rounds = 0;

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Time.timeScale = 1f;
        }


    }

    public void Menu()
    {
        
        SceneManager.LoadScene(MainMenueScene);
        Time.timeScale = 1f;
    }


}
