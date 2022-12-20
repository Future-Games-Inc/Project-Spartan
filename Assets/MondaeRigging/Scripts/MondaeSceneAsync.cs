using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MondaeSceneAsync : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(LoadSceneAsync(1));
    }

    IEnumerator LoadSceneAsync(int sceneIndex)
    {
        yield return new WaitForSeconds(1);
        var asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        asyncLoad.allowSceneActivation = false;
        var hasWaitLoad = true;

        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress == 0.9f)
            {
                if (hasWaitLoad)
                {
                    hasWaitLoad = false;
                    yield return WaitLoad(asyncLoad);
                }
            }

            yield return null;
        }
    }

    IEnumerator WaitLoad(AsyncOperation asyncLoad)
    {
        yield return new WaitForSeconds(10);
        asyncLoad.allowSceneActivation = true;
    }
}
