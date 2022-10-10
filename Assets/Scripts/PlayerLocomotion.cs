using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PCScripts
{
    public class PlayerLocomotion : MonoBehaviour
    {
        public Transform playerObject_Trn; // for the roation of the came object
        InputHandler InputHandler;
        Vector3 moveDirection;

        [HideInInspector] public Transform myTransform;

        public new Rigidbody rigidbody;
        public GameObject normalCamera;

        [Header("Stats")]
        [SerializeField] float movementSpeed = 5f;
        [SerializeField] float rotationSpeed = 10f;




        void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            InputHandler = GetComponent<InputHandler>();
            //playerObject_Trn = transform; // for the roation of the game object
            myTransform = transform;

        }

        public void Update()
        {
            float delta = Time.deltaTime;

            InputHandler.TickInput(delta);

            moveDirection = playerObject_Trn.forward * InputHandler.vertical;
            moveDirection += playerObject_Trn.right * InputHandler.horizontal;
            moveDirection.Normalize();

            float speed = movementSpeed;
            moveDirection *= speed;

            Vector3 projectVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);

            rigidbody.velocity = projectVelocity;

        }


        #region Movement
        Vector3 normalVector;
        Vector3 targetPosition;

        private void HandleRotation (float delta)
        {
            Vector3 targetDirection = Vector3.zero;
            float moveOverRide = InputHandler.moveAmount;

            targetDirection = playerObject_Trn.forward * InputHandler.vertical;
            targetDirection += playerObject_Trn.right * InputHandler.horizontal;
            targetDirection.Normalize();

            targetDirection.y = 0;

            if (targetDirection == Vector3.zero)
            {
                targetDirection = myTransform.forward;

            }

            float rs = rotationSpeed;

            Quaternion tr = Quaternion.LookRotation(targetDirection);
            Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta);

            myTransform.rotation = targetRotation;

        }


        #endregion 


    }



}

