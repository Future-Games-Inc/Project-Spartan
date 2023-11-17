using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRKB;

public class KeyboardBridge : MonoBehaviour
{
    protected KeyboardBehaviour _keyboard;
    protected KeyBehaviour _key;

    // Start is called before the first frame update
    void Start()
    {
        _keyboard = GetComponentInParent<KeyboardBehaviour>();

        _key = GetComponent<KeyBehaviour>();
    }

    public void OnKeyboardButtonClick()
    {
        _keyboard.PressKey(_key, null, true);
    }
}
