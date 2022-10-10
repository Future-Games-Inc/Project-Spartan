using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToggleWristUI : MonoBehaviour
{
    public GameObject wristUI;
    public InputActionReference toggleButton;
    // Start is called before the first frame update
    void Awake()
    {
        toggleButton.action.started +=_=>ToggleUI();
    }
    void ToggleUI() {
        wristUI.SetActive(!wristUI.activeSelf);

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
