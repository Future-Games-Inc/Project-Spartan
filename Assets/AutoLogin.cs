using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoLogin : MonoBehaviour
{
    public WhiteLabelManager WhiteLabelManager;
    // Start is called before the first frame update
    void OnEnable()
    {
        WhiteLabelManager.AutoLogin();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
