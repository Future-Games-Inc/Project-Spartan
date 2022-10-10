using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace PCScripts
{
    public class InputHandler : MonoBehaviour
    {


        public float horizontal;
        public float vertical;
        public float moveAmount;
        public float xAxisTurn;
        public float yAxisTurn;
        //public float mousey;

        XRIDefaultInputActions inputActions; //" playercontrols scripts"

        Vector2 movementInput;
        Vector2 turningInput;

        public void OnEnable()
        {
             if(inputActions == null)
            {
                inputActions = new XRIDefaultInputActions();
                inputActions.XRILeftHandLocomotion.Move.performed += inputActions => movementInput = inputActions.ReadValue<Vector2>();
                inputActions.XRIRightHandLocomotion.Move.performed += inputActions => turningInput = inputActions.ReadValue<Vector2>();

            }

            inputActions.Enable();

        }

        private void OnDisable()
        {
            inputActions.Disable();
        }

        public void TickInput( float delta)
        {
            MoveInput(delta);
        }

        private void MoveInput(float delta)
        {
            horizontal = movementInput.x;
            vertical = movementInput.y;

            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
            xAxisTurn = turningInput.x;
            yAxisTurn = turningInput.y;
        }


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }


}

