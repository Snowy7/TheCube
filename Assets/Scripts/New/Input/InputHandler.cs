using UnityEngine;
using System;
using UnityEngine.InputSystem;

namespace New.Input
{
    public class InputHandler : MonoBehaviour
    {
        // Events for different input actions
        public event Action<Vector2> OnMove;
        public event Action<Vector2> OnLook;
        public event Action OnJump;
        public event Action OnSprintStart;
        public event Action OnSprintEnd;
        public event Action OnCrouchStart;
        public event Action OnCrouchEnd;
        public event Action OnFire;
        public event Action OnFireReleased;
        public event Action OnReload;
        public event Action<int> OnSwitchWeapon;
        public event Action OnLeanLeft;
        public event Action OnLeanRight;
        public event Action OnLeanReset;
        public event Action OnInteract;

        [Header("Input Settings")] [SerializeField]
        private float lookSensitivity = 1.0f;

        [SerializeField] private bool invertY = false;

        private PlayerInput playerInput;
        private InputAction moveAction;
        private InputAction lookAction;
        private InputAction jumpAction;
        private InputAction sprintAction;
        private InputAction crouchAction;
        private InputAction fireAction;
        private InputAction reloadAction;
        private InputAction weaponSwapAction;
        private InputAction leanLeftAction;
        private InputAction leanRightAction;
        private InputAction interactAction;

        private void Awake()
        {
            // Create a new PlayerInput component if not already added
            playerInput = GetComponent<PlayerInput>();
            if (playerInput == null)
            {
                playerInput = gameObject.AddComponent<PlayerInput>();
                playerInput.defaultActionMap = "Player";
            }

            // Get references to the actions
            moveAction = playerInput.actions["Move"];
            lookAction = playerInput.actions["Look"];
            jumpAction = playerInput.actions["Jump"];
            sprintAction = playerInput.actions["Sprint"];
            crouchAction = playerInput.actions["Crouch"];
            fireAction = playerInput.actions["Fire"];
            reloadAction = playerInput.actions["Reload"];
            weaponSwapAction = playerInput.actions["MouseWheel"];
            leanLeftAction = playerInput.actions["LeanLeft"];
            leanRightAction = playerInput.actions["LeanRight"];
            interactAction = playerInput.actions["Interact"];
        }

        private void OnEnable()
        {
            // Subscribe to input events
            jumpAction.performed += OnJumpPerformed;
            sprintAction.performed += OnSprintPerformed;
            sprintAction.canceled += OnSprintCanceled;
            crouchAction.performed += OnCrouchPerformed;
            crouchAction.canceled += OnCrouchCanceled;
            fireAction.performed += OnFirePerformed;
            fireAction.canceled += OnFireCanceled;
            reloadAction.performed += OnReloadPerformed;
            weaponSwapAction.performed += OnWeaponSwapPerformed;
            leanLeftAction.performed += OnLeanLeftPerformed;
            leanLeftAction.canceled += OnLeanCanceled;
            leanRightAction.performed += OnLeanRightPerformed;
            leanRightAction.canceled += OnLeanCanceled;
            interactAction.performed += OnInteractPerformed;
        }

        private void OnDisable()
        {
            // Unsubscribe from input events
            jumpAction.performed -= OnJumpPerformed;
            sprintAction.performed -= OnSprintPerformed;
            sprintAction.canceled -= OnSprintCanceled;
            crouchAction.performed -= OnCrouchPerformed;
            crouchAction.canceled -= OnCrouchCanceled;
            fireAction.performed -= OnFirePerformed;
            fireAction.canceled -= OnFireCanceled;
            reloadAction.performed -= OnReloadPerformed;
            weaponSwapAction.performed -= OnWeaponSwapPerformed;
            leanLeftAction.performed -= OnLeanLeftPerformed;
            leanLeftAction.canceled -= OnLeanCanceled;
            leanRightAction.performed -= OnLeanRightPerformed;
            leanRightAction.canceled -= OnLeanCanceled;
            interactAction.performed -= OnInteractPerformed;
        }

        private void Update()
        {
            // Continuous inputs are processed every frame
            Vector2 moveInput = moveAction.ReadValue<Vector2>();
            Vector2 lookInput = lookAction.ReadValue<Vector2>() * lookSensitivity;

            // Apply invert Y if needed
            if (invertY)
            {
                lookInput.y = -lookInput.y;
            }

            // Broadcast inputs
            OnMove?.Invoke(moveInput);
            OnLook?.Invoke(lookInput);
        }

        #region Input Event Handlers

        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            OnJump?.Invoke();
        }

        private void OnSprintPerformed(InputAction.CallbackContext context)
        {
            OnSprintStart?.Invoke();
        }

        private void OnSprintCanceled(InputAction.CallbackContext context)
        {
            OnSprintEnd?.Invoke();
        }

        private void OnCrouchPerformed(InputAction.CallbackContext context)
        {
            OnCrouchStart?.Invoke();
        }

        private void OnCrouchCanceled(InputAction.CallbackContext context)
        {
            OnCrouchEnd?.Invoke();
        }

        private void OnFirePerformed(InputAction.CallbackContext context)
        {
            OnFire?.Invoke();
        }

        private void OnFireCanceled(InputAction.CallbackContext context)
        {
            OnFireReleased?.Invoke();
        }

        private void OnReloadPerformed(InputAction.CallbackContext context)
        {
            OnReload?.Invoke();
        }

        private void OnWeaponSwapPerformed(InputAction.CallbackContext context)
        {
            // Get the weapon index from the scroll wheel or number keys
            float scrollValue = context.ReadValue<float>();
            int weaponIndex = scrollValue > 0 ? 1 : -1;

            OnSwitchWeapon?.Invoke(weaponIndex);
        }

        private void OnLeanLeftPerformed(InputAction.CallbackContext context)
        {
            OnLeanLeft?.Invoke();
        }

        private void OnLeanRightPerformed(InputAction.CallbackContext context)
        {
            OnLeanRight?.Invoke();
        }

        private void OnLeanCanceled(InputAction.CallbackContext context)
        {
            OnLeanReset?.Invoke();
        }

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            OnInteract?.Invoke();
        }

        #endregion

        public void SetLookSensitivity(float sensitivity)
        {
            lookSensitivity = sensitivity;
        }

        public void SetInvertY(bool invert)
        {
            invertY = invert;
        }
    }
}