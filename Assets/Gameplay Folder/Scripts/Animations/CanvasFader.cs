using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasFader : MonoBehaviour
{
    public CanvasGroup canvasGroup;

    public bool fadeIn = false;
    public bool fadeOut = false;
    // Start is called before the first frame update
    void Start()
    {
        fadeIn = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(fadeIn)
        {
            if(canvasGroup.alpha <1)
            {
                canvasGroup.alpha += Time.deltaTime/10;
                if(canvasGroup.alpha >= 1)
                {
                    fadeIn= false;
                }
            }
        }

        if (fadeOut)
        {
            if (canvasGroup.alpha >= 0)
            {
                canvasGroup.alpha -= Time.deltaTime;
                if (canvasGroup.alpha == 0)
                {
                    fadeOut = false;
                }
            }
        }
    }
}
