using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadLobby());
    }

    // Update is called once per frame
    private IEnumerator LoadLobby()
    {
        yield return new WaitForSeconds(20);
        SceneManager.LoadScene(1);
    }
}
