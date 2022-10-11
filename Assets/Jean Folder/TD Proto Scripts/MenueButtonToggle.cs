using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class MenueButtonToggle : MonoBehaviour
{

    public InputActionReference left2DAxisClick;
    [SerializeField] private UnityEvent left2DAxisClickEvent;

    //XRIDefaultInputActions leftButtonPress;

    private void Awake()
    {
        left2DAxisClick.action.started += left2DAxisPressed;
    }

    private void OnDestroy()
    {
        left2DAxisClick.action.started -= left2DAxisPressed;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void left2DAxisPressed(InputAction.CallbackContext context)
    {
        left2DAxisClickEvent.Invoke();
        Debug.Log("Action Input Controller is pausing the game");
    }


}
