using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public TMP_Text roundsText;
    public string menueScene;

     void OnEnable()
    {
        roundsText.text = PlayerStats.Rounds.ToString();
    }

    public void Retry()
    {
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //SceneManger.LoadScene("Prototype Scene")

    }

    public void Menu()
    {
        SceneManager.LoadScene(menueScene);
        Debug.Log("Going to Menu");
    }


}
