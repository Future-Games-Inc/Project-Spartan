using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraReplaceShader : MonoBehaviour
{
    public Camera targetCamera;
    // Start is called before the first frame update
    void Start()
    {
        targetCamera.SetReplacementShader(Shader.Find("Unlit/Color"), "RenderType");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
