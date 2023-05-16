using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    public string gameObjectTag;

    private void Start()
    {
        string tag = this.tag;
        gameObjectTag = tag;
        if (GameObject.FindGameObjectsWithTag(gameObjectTag).Length == 1)
            DontDestroyOnLoad(gameObject);
        else
            Destroy(this.gameObject);
    }
}
