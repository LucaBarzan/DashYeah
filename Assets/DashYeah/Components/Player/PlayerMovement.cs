using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashYeah.Components.Player
{
    [System.Serializable]
    public class PlayerMovement
    {
        // Inpector exposed private varibles
        [Header("Basics")]
        [SerializeField] private float movementSpeed = 30.0f;
        [SerializeField] private float gravityScale = 5.0f;

        [Header("Jump")]
        [SerializeField] private float jumpHeight = 5.0f;

        [Header("Dash")]
        [SerializeField] private float dashDistance = 5.0f;
        [SerializeField] private LayerMask dashCollisionLayers;

        // Private variables
        private Object.Entity.Player player;
        private CharacterController characterController;
        private Vector3 velocity;
        private float gravity;
        private float initialJumpVelocity;
        private readonly StateMachine stateMachine = new StateMachine();


        // Private const
        private const float GROUNDED_GRAVITY = -0.1f;
        private const int STATE_BASIC_MOVEMENT = 0;
        private const int STATE_DASH = 1;


        public void Setup(Object.Entity.Player player)
        {
            this.player = player;
            SetupMovementVariables();
            characterController = player.GetCharacterController();

            stateMachine.AddEnterState(STATE_BASIC_MOVEMENT, BasicMovement_Enter);
            stateMachine.AddUpdateState(STATE_BASIC_MOVEMENT, BasicMovement_Update);
            stateMachine.AddExitState(STATE_BASIC_MOVEMENT, BasicMovement_Exit);

            stateMachine.AddEnterState(STATE_DASH, Dash_Enter);
            stateMachine.AddUpdateState(STATE_DASH, Dash_Update);
            stateMachine.AddExitState(STATE_DASH, Dash_Exit);
        }

        public void Update()
        {

#if UNITY_EDITOR
            // Update the inspector values every frame to make it easier for the Designers to tweek the values in real time
            SetupMovementVariables();
#endif
            stateMachine.UpdateState();
        }

        private void SetupMovementVariables()
        {
            gravity = Physics.gravity.y * gravityScale;
            initialJumpVelocity = Mathf.Sqrt(jumpHeight * -2.0f * gravity);
        }

        #region Movement States

        #region Basic Movement

        void BasicMovement_Enter()
        {

        }

        void BasicMovement_Update()
        {
            HandleMovement();
        }

        void BasicMovement_Exit()
        {

        }

        void HandleMovement()
        {
            // Get the input direction
            Vector2 movementDirection = player.GetInputDirection();

            // Add speed to the movement direction
            movementDirection *= movementSpeed;

            // Apply the new movement direction to the velocity
            velocity = new Vector3(movementDirection.x, velocity.y, movementDirection.y);

            // Apply a default little gravity if it is grounded
            if (characterController.isGrounded && velocity.y < 0.0f)
            {
                velocity.y = GROUNDED_GRAVITY;
            }

            // Add gravity to the velocity
            velocity.y += gravity * Time.deltaTime;

            // Apply the velocity
            characterController.Move(velocity * Time.deltaTime);
        }

        public void Jump()
        {
            // Only can jump if the player is on the ground
            if (!characterController.isGrounded)
                return;

            // Add gravity to the velocity
            velocity.y = initialJumpVelocity;
        }

        #endregion // Basic Movement

        #region Dash State

        void Dash_Enter()
        {

        }

        void Dash_Update()
        {
            /*
            //Get the direction that the player is moving
            Vector3 direction = Transform.forward * inputDirection;

            //Get the radius of the character to simulate in the capsule raycast
            float radius = characterController.radius;

            //Do the sphere raycast
            RaycastHit[] hits = Physics.SphereCastAll(Transform.position, radius, direction, dashDistance, dashCollisionLayers);

            //Get the closest collision if there was one
            float shortestDashDistance = dashDistance;
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].distance < shortestDashDistance)
                    shortestDashDistance = hits[i].distance;
            }

            //Add the Dash position to the player position
            velocity = direction * shortestDashDistance;
            */
        }

        void Dash_Exit()
        {

        }

        public void Dash()
        {
            stateMachine.CurrentState = STATE_DASH;
        }

        #endregion // Dash State

        #endregion // Movement States
    }
}