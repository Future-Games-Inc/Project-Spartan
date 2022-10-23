using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MondaeSceneAsync : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(LoadScene(1));
    }

    IEnumerator LoadScene(int sceneIndex)
    {
        yield return new WaitForSeconds(0);
        SceneManager.LoadScene(sceneIndex);
    }
}

