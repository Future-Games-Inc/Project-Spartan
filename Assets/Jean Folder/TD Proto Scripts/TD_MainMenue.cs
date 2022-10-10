using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TD_MainMenue : MonoBehaviour
{
    public string levelToLoad = "MainLevel";//TODO: Change this to the main level to load


    public void Play()
    {
        SceneManager.LoadScene(levelToLoad);

    }


    public void Quit()
    {
        Debug.Log("QUITING GAME");

        Application.Quit();


    }
}
