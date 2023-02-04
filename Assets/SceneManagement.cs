using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(1);
    }

    public void FiringRange()
    {
        SceneManager.LoadScene(8);
    }

    public void Demo()
    {
        SceneManager.LoadScene(7);
    }
}
