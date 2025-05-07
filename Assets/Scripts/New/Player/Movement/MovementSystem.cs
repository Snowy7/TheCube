using UnityEngine;
using System;
using System.Collections;
using New.Utils;

namespace New.Player
{
    /// <summary>
    /// Handles all player movement including walking, sprinting, crouching, sliding, and jumping.
    /// Follows the Single Responsibility Principle by focusing solely on movement mechanics.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class MovementSystem : MonoBehaviour
    {
        #region Serialized Fields
        
        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float sprintSpeed = 8f;
        [SerializeField] private float crouchSpeed = 3f;
        [SerializeField] private float slideSpeed = 10f;
        [SerializeField] private float slideDuration = 1f;
        [SerializeField] private float slideCooldown = 1.5f;
        [SerializeField] private float airControl = 0.5f;
        [SerializeField] private float gravity = 20f;
        
        [Header("Jump Settings")]
        [SerializeField] private float jumpHeight = 2f;
        [SerializeField] private float jumpCooldown = 0.1f;
        [SerializeField] private float ceilingBounceMultiplier = 0.5f;
        [SerializeField] private LayerMask ceilingLayers;
        [SerializeField] private float ceilingCheckDistance = 1.2f;
        
        [Header("Crouch Settings")]
        [SerializeField] private float standingHeight = 2f;
        [SerializeField] private float crouchHeight = 1f;
        [SerializeField] private float crouchTransitionSpeed = 10f;
        
        [Header("Ground Check")]
        [SerializeField] private float slopeLimit = 45f;
        [SerializeField] private float stepOffset = 0.3f;
        [SerializeField] private float skinWidth = 0.08f;
        [SerializeField] private LayerMask groundLayers;
        [SerializeField] private float groundCheckDistance = 0.2f;
        
        #endregion
        
        #region State Variables
        
        private Vector2 movementInput;
        private MovementState currentState = MovementState.Normal;
        private bool isSprinting;
        private bool isCrouching;
        private bool isSliding;
        private float currentHeight;
        private float currentLeanAngle;
        private float currentLeanOffset;
        private Vector3 slideDirection;
        private float lastJumpTime;
        private float lastSlideTime;
        private bool canJump = true;
        private bool isGrounded;
        private bool wasGroundedLastFrame = true;
        private bool hasJumpedThisFrame = false;
        
        // Velocity components
        private Vector3 movementVelocity; // Horizontal movement
        private float verticalVelocity;   // Vertical movement (gravity, jumps)
        private Vector3 impactVelocity;   // External forces (bounces, knockbacks)
        
        private float impactDecay = 5f;
        
        #endregion
        
        #region References
        
        private CharacterController characterController;
        private Transform cameraTransform;
        
        #endregion
        
        #region Events
        
        /// <summary>
        /// Triggered when the movement state changes
        /// </summary>
        public event Action<MovementState> OnMovementStateChanged;
        
        /// <summary>
        /// Triggered when the player lands on the ground
        /// </summary>
        public event Action OnLanded;
        
        /// <summary>
        /// Triggered when the player jumps
        /// </summary>
        public event Action OnJumped;
        
        /// <summary>
        /// Triggered when the player starts sliding
        /// </summary>
        public event Action OnSlideStart;
        
        /// <summary>
        /// Triggered when the player stops sliding
        /// </summary>
        public event Action OnSlideEnd;
        
        #endregion
        
        #region Unity Lifecycle Methods
        
        private void Awake()
        {
            InitializeComponents();
        }
        
        private void Update()
        {
            // Process all movement calculations in order
            CheckGrounded();
            UpdateState();
            UpdateControllerHeight();
            CalculateMovementVelocity();
            CalculateVerticalVelocity();
            DecayImpactVelocity();
            
            // Apply the final combined velocity
            ApplyVelocity();
            
            // Reset per-frame flags
            hasJumpedThisFrame = false;
            wasGroundedLastFrame = isGrounded;
        }
        
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            HandleCollisions(hit);
        }
        
        # if UNITY_EDITOR
        
        private void OnDrawGizmos()
        {
            // Draw ground check ray for debugging
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, Vector3.down * groundCheckDistance);
            
            // Draw ceiling check ray for debugging
            Gizmos.color = Color.blue;
            var topPos = transform.position + Vector3.up * (standingHeight - 0.2f);
            Gizmos.DrawRay(topPos, Vector3.up * ceilingCheckDistance);
        }
        
        private void OnValidate()
        {
            // Ensure values are within reasonable ranges
            walkSpeed = Mathf.Max(0, walkSpeed);
            sprintSpeed = Mathf.Max(walkSpeed, sprintSpeed);
            crouchSpeed = Mathf.Max(0, crouchSpeed);
            slideSpeed = Mathf.Max(0, slideSpeed);
            jumpHeight = Mathf.Max(0, jumpHeight);
            gravity = Mathf.Max(0, gravity);
            standingHeight = Mathf.Max(0.1f, standingHeight);
            crouchHeight = Mathf.Clamp(crouchHeight, 0.1f, standingHeight);
        }
        
        #endif
        
        #endregion
        
        #region Initialization
        
        /// <summary>
        /// Initializes required components and configures the character controller
        /// </summary>
        private void InitializeComponents()
        {
            characterController = GetComponent<CharacterController>();
            if (characterController == null)
            {
                Debug.LogError("CharacterController component is required but not found!");
                this.enabled = false;
                return;
            }
            
            // Initialize state
            currentHeight = standingHeight;
            
            // Configure character controller
            characterController.slopeLimit = slopeLimit;
            characterController.stepOffset = stepOffset;
            characterController.skinWidth = skinWidth;
            characterController.height = currentHeight;
            characterController.center = new Vector3(0, currentHeight / 2, 0);
            
            lastJumpTime = -jumpCooldown;
            lastSlideTime = -slideCooldown;
        }
        
        /// <summary>
        /// Initializes the camera reference
        /// </summary>
        /// <param name="camera">The player camera transform</param>
        public void Initialize(Transform camera)
        {
            cameraTransform = camera;
            
            // Set initial camera position
            if (cameraTransform != null)
            {
                Vector3 cameraLocalPos = cameraTransform.localPosition;
                cameraLocalPos.y = currentHeight - 0.2f; // Slightly below top of controller
                cameraTransform.localPosition = cameraLocalPos;
            }
        }
        
        #endregion
        
        #region State Management
        
        /// <summary>
        /// Updates the current movement state based on player conditions
        /// </summary>
        private void UpdateState()
        {
            MovementState previousState = currentState;
            
            if (isSliding)
            {
                currentState = MovementState.Sliding;
            }
            else if (!isGrounded)
            {
                currentState = MovementState.Airborne;
            }
            else if (isCrouching)
            {
                currentState = MovementState.Crouching;
            }
            else if (isSprinting)
            {
                currentState = MovementState.Sprinting;
            }
            else
            {
                currentState = MovementState.Normal;
            }
            
            // Notify on state change
            if (previousState != currentState)
            {
                OnMovementStateChanged?.Invoke(currentState);
                
                // Handle landing
                if (previousState == MovementState.Airborne && isGrounded)
                {
                    OnLanded?.Invoke();
                }
            }
            
            // Check if slide should end
            if (isSliding)
            {
                float slideProgress = (Time.time - lastSlideTime) / slideDuration;
                if (slideProgress >= 1.0f)
                {
                    StopSliding();
                }
            }
        }
        
        #endregion
        
        #region Movement Calculations
        
        /// <summary>
        /// Checks if the character is grounded and updates the character controller state
        /// </summary>
        private void CheckGrounded()
        {
            // Check if the character is grounded
            isGrounded = Physics.Raycast(
                transform.position,
                Vector3.down,
                groundCheckDistance,
                groundLayers
            );
        }
        
        /// <summary>
        /// Calculates the horizontal movement velocity based on input and current state
        /// </summary>
        private void CalculateMovementVelocity()
        {
            // Get input direction relative to camera orientation
            Vector3 moveDirection = CalculateMoveDirection();
            
            // Set speed based on current state
            float currentSpeed = DetermineMovementSpeed();
            
            // Apply horizontal movement to velocity
            if (currentState != MovementState.Sliding)
            {
                // Normal movement control
                if (isGrounded)
                {
                    movementVelocity.x = moveDirection.x * currentSpeed;
                    movementVelocity.z = moveDirection.z * currentSpeed;
                }
                else
                {
                    // In air, add to existing velocity for more natural movement
                    movementVelocity.x = Mathf.Lerp(movementVelocity.x, moveDirection.x * currentSpeed, airControl * Time.deltaTime);
                    movementVelocity.z = Mathf.Lerp(movementVelocity.z, moveDirection.z * currentSpeed, airControl * Time.deltaTime);
                }
            }
            else
            {
                // Sliding physics - maintain direction but apply speed
                movementVelocity.x = slideDirection.x * currentSpeed;
                movementVelocity.z = slideDirection.z * currentSpeed;
            }
        }
        
        /// <summary>
        /// Calculates the movement direction based on input and camera orientation
        /// </summary>
        /// <returns>Normalized movement direction vector</returns>
        private Vector3 CalculateMoveDirection()
        {
            Vector3 moveDirection = Vector3.zero;
            
            if (movementInput.sqrMagnitude > 0.01f && cameraTransform != null)
            {
                // Convert input to world space direction based on camera orientation
                Vector3 forward = cameraTransform.forward;
                Vector3 right = cameraTransform.right;
                
                // Project vectors onto the horizontal plane
                forward.y = 0;
                right.y = 0;
                forward.Normalize();
                right.Normalize();
                
                // Calculate movement direction - this is in world space, not local
                moveDirection = right * movementInput.x + forward * movementInput.y;
                moveDirection.Normalize();
            }
            
            return moveDirection;
        }
        
        /// <summary>
        /// Determines the appropriate movement speed based on the current state
        /// </summary>
        /// <returns>The current movement speed</returns>
        private float DetermineMovementSpeed()
        {
            switch (currentState)
            {
                case MovementState.Sprinting:
                    return sprintSpeed;
                    
                case MovementState.Crouching:
                    return crouchSpeed;
                    
                case MovementState.Sliding:
                    // Use sliding speed with decay over time
                    float slideProgress = (Time.time - lastSlideTime) / slideDuration;
                    return Mathf.Lerp(slideSpeed, crouchSpeed, slideProgress);
                    
                case MovementState.Airborne:
                    // Reduced control in air
                    return walkSpeed * airControl;
                    
                default:
                    return walkSpeed;
            }
        }
        
        /// <summary>
        /// Calculates the vertical velocity component (gravity and jumps)
        /// </summary>
        private void CalculateVerticalVelocity()
        {
            // If we just jumped this frame, don't apply any other vertical velocity logic
            if (hasJumpedThisFrame)
            {
                return;
            }
            
            if (isGrounded)
            {
                // Apply a small downward force to keep grounded
                // Only reset vertical velocity if we're not moving upward (to prevent jump cancellation)
                if (verticalVelocity <= 0)
                {
                    verticalVelocity = -gravity * 0.5f * Time.deltaTime;
                    
                    // Reset the jump when grounded
                    if (currentState != MovementState.Sliding && !hasJumpedThisFrame)
                    {
                        canJump = true;
                    }
                }
                else
                {
                    // We're grounded but still have upward velocity (like at the start of a jump)
                    // Apply gravity to gradually reduce it
                    verticalVelocity -= gravity * Time.deltaTime;
                }
            }
            else
            {
                // Apply gravity when in air
                verticalVelocity -= gravity * Time.deltaTime;
            }
        }
        
        /// <summary>
        /// Gradually reduces impact velocity over time
        /// </summary>
        private void DecayImpactVelocity()
        {
            if (impactVelocity.magnitude > 0.2f)
            {
                impactVelocity = Vector3.Lerp(impactVelocity, Vector3.zero, impactDecay * Time.deltaTime);
            }
            else
            {
                impactVelocity = Vector3.zero;
            }
        }
        
        /// <summary>
        /// Applies the final calculated velocity to move the character
        /// </summary>
        private void ApplyVelocity()
        {
            // Combine all velocity components
            Vector3 finalVelocity = new Vector3(
                movementVelocity.x,
                verticalVelocity,
                movementVelocity.z
            );
            
            // Add any impact forces
            finalVelocity += impactVelocity;
            
            // Move the character
            characterController.Move(finalVelocity * Time.deltaTime);
        }
        
        #endregion
        
        #region Collision Handling
        
        /// <summary>
        /// Handles collisions with the environment
        /// </summary>
        /// <param name="hit">Information about the collision</param>
        private void HandleCollisions(ControllerColliderHit hit)
        {
            // Check for ceiling collision while moving upward
            if (verticalVelocity > 0 && Vector3.Dot(hit.normal, Vector3.down) > 0.7f)
            {
                // This is a ceiling hit while moving upward
                BounceOffCeiling(hit.normal);
            }
        }
        
        /// <summary>
        /// Applies a bounce effect when hitting a ceiling
        /// </summary>
        /// <param name="normal">The normal vector of the ceiling surface</param>
        private void BounceOffCeiling(Vector3 normal)
        {
            // Calculate bounce velocity
            Vector3 currentVelocity = new Vector3(movementVelocity.x, verticalVelocity, movementVelocity.z);
            Vector3 bounceVelocity = Vector3.Reflect(currentVelocity, normal) * ceilingBounceMultiplier;
            
            // Reduce horizontal component
            bounceVelocity.x *= 0.5f;
            bounceVelocity.z *= 0.5f;
            
            // Apply as impact force
            impactVelocity = bounceVelocity;
            
            // Reset vertical velocity
            verticalVelocity = 0;
        }
        
        #endregion
        
        #region Height and Posture Management
        
        /// <summary>
        /// Updates the controller height based on stance (standing, crouching, sliding)
        /// </summary>
        private void UpdateControllerHeight()
        {
            // Smoothly adjust height based on stance
            float targetHeight = isCrouching || isSliding ? crouchHeight : standingHeight;
            currentHeight = Mathf.Lerp(currentHeight, targetHeight, crouchTransitionSpeed * Time.deltaTime);
            
            // Update controller
            characterController.height = currentHeight;
            characterController.center = new Vector3(0, currentHeight / 2, 0);
            
            // Update camera position based on height
            if (cameraTransform != null)
            {
                Vector3 cameraLocalPos = cameraTransform.localPosition;
                cameraLocalPos.y = currentHeight - 0.2f; // Slightly below top of controller
                cameraTransform.localPosition = cameraLocalPos;
            }
        }
        
        #endregion
        
        #region Public Movement Controls
        
        /// <summary>
        /// Sets the movement input vector
        /// </summary>
        /// <param name="input">2D movement input (x,y)</param>
        public void SetMovementInput(Vector2 input)
        {
            movementInput = input;
        }
        
        /// <summary>
        /// Initiates a jump if conditions allow
        /// </summary>
        public void Jump()
        {
            // Check if jump is allowed
            if (!canJump) 
            {
                return;
            }
            
            if (!isGrounded) 
            {
                return;
            }
            
            if (Time.time - lastJumpTime < jumpCooldown) 
            {
                return;
            }
            
            // Calculate jump velocity from jump height using physics formula: v = sqrt(2 * g * h)
            float jumpVelocity = Mathf.Sqrt(2f * gravity * jumpHeight);
            
            // Check if there's a ceiling above before applying jump
            var topPos = transform.position + Vector3.up * (standingHeight - 0.2f);
            bool hitCeiling = Physics.Raycast(
                topPos,
                Vector3.up, 
                out RaycastHit ceilingHit, 
                ceilingCheckDistance, 
                ceilingLayers
            );
            
            if (hitCeiling)
            {
                // If we detect a ceiling immediately above, don't apply full jump force
                jumpVelocity *= 0.3f;
            }
            
            // Apply jump force directly to vertical velocity
            verticalVelocity = jumpVelocity;
            
            // Mark that we've jumped this frame to prevent immediate gravity application
            hasJumpedThisFrame = true;
            
            // Update jump state
            lastJumpTime = Time.time;
            canJump = false;
            
            // Trigger jump event
            OnJumped?.Invoke();
        }
        
        /// <summary>
        /// Starts sprinting if conditions allow
        /// </summary>
        public void StartSprinting()
        {
            if (currentState != MovementState.Crouching && currentState != MovementState.Sliding)
            {
                isSprinting = true;
            }
        }
        
        /// <summary>
        /// Stops sprinting
        /// </summary>
        public void StopSprinting()
        {
            isSprinting = false;
        }
        
        /// <summary>
        /// Starts crouching
        /// </summary>
        public void StartCrouching()
        {
            isCrouching = true;
            isSprinting = false;
        }
        
        /// <summary>
        /// Attempts to stop crouching if there's room to stand
        /// </summary>
        public void StopCrouching()
        {
            // Check if there's room to stand
            Vector3 standCheckStart = transform.position + 
                new Vector3(0, characterController.height / 2, 0);
            
            if (Physics.Raycast(standCheckStart, Vector3.up, standingHeight - characterController.height, ceilingLayers))
            {
                // Can't stand up yet, still obstructed
                return;
            }
            
            isCrouching = false;
        }
        
        /// <summary>
        /// Initiates a slide if conditions allow
        /// </summary>
        public void StartSliding()
        {
            if (!isGrounded) return;
            if (Time.time - lastSlideTime < slideCooldown) return;
            if (movementInput.sqrMagnitude < 0.1f) return; // Need to be moving to slide
            
            isSliding = true;
            isCrouching = true;
            lastSlideTime = Time.time;
            
            // Store current forward direction for slide
            slideDirection = CalculateMoveDirection();
            
            OnSlideStart?.Invoke();
        }
        
        /// <summary>
        /// Stops sliding
        /// </summary>
        public void StopSliding()
        {
            if (!isSliding) return;
            
            isSliding = false;
            // Stay crouched after sliding
            
            OnSlideEnd?.Invoke();
        }
        
        #endregion
        
        #region Public Properties
        
        /// <summary>
        /// Whether the character is currently moving
        /// </summary>
        public bool IsMoving => movementInput.sqrMagnitude > 0.01f;
        
        /// <summary>
        /// Whether the character is currently on the ground
        /// </summary>
        public bool IsGrounded => isGrounded;
        
        /// <summary>
        /// Whether the character is currently sprinting
        /// </summary>
        public bool IsSprinting => isSprinting;
        
        /// <summary>
        /// Whether the character is currently crouching
        /// </summary>
        public bool IsCrouching => isCrouching;
        
        /// <summary>
        /// Whether the character is currently sliding
        /// </summary>
        public bool IsSliding => isSliding;
        
        /// <summary>
        /// The current movement state
        /// </summary>
        public MovementState CurrentState => currentState;
        
        /// <summary>
        /// The current height of the character controller
        /// </summary>
        public float CurrentHeight => currentHeight;
        
        /// <summary>
        /// The current vertical velocity
        /// </summary>
        public float VerticalVelocity => verticalVelocity;
        
        #endregion
    }
}
