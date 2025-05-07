using UnityEngine;
using System;
using New.Input;

namespace New.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        // References to subsystems
        [SerializeField] private HealthSystem healthSystem;
        [SerializeField] private MovementSystem movementSystem;
        [SerializeField] private InventorySystem inventorySystem;
        [SerializeField] private WeaponSystem weaponSystem;
        [SerializeField] private CameraSystem cameraSystem;
        [SerializeField] private LeanSystem leanSystem;
        
        [Header("Player Settings")]
        [SerializeField] private float interactionRange = 2.5f;
        
        private InputHandler inputHandler;
        private Rigidbody rb;
        private bool isPlayerActive = true;
        
        // Events
        public event Action OnPlayerDeath;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            
            // Auto-find components if not assigned
            healthSystem ??= GetComponentInChildren<HealthSystem>();
            movementSystem ??= GetComponentInChildren<MovementSystem>();
            inventorySystem ??= GetComponentInChildren<InventorySystem>();
            weaponSystem ??= GetComponentInChildren<WeaponSystem>();
            cameraSystem ??= GetComponentInChildren<CameraSystem>();
            leanSystem ??= GetComponentInChildren<LeanSystem>();
            inputHandler = GetComponent<InputHandler>();
            
            // Validate all required components exist
            ValidateComponents();
            
            // Initialize systems with references to each other where needed
            movementSystem.Initialize(cameraSystem.transform.parent);
            weaponSystem.Initialize(inventorySystem, cameraSystem);
            
            // Subscribe to events
            healthSystem.OnDeath += HandlePlayerDeath;
            
            // Lock cursor for FPS control
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        private void ValidateComponents()
        {
            if (healthSystem == null) Debug.LogError("PlayerController: HealthSystem not assigned!");
            if (movementSystem == null) Debug.LogError("PlayerController: MovementSystem not assigned!");
            if (inventorySystem == null) Debug.LogError("PlayerController: InventorySystem not assigned!");
            if (weaponSystem == null) Debug.LogError("PlayerController: WeaponSystem not assigned!");
            if (cameraSystem == null) Debug.LogError("PlayerController: CameraSystem not assigned!");
            if (leanSystem == null) Debug.LogError("PlayerController: LeanSystem not assigned!");
            if (inputHandler == null) Debug.LogError("PlayerController: InputHandler not found!");
        }
        
        private void OnEnable()
        {
            if (inputHandler != null)
            {
                // Connect input events to system methods
                inputHandler.OnMove += movementSystem.SetMovementInput;
                inputHandler.OnLook += cameraSystem.HandleLookInput;
                inputHandler.OnJump += movementSystem.Jump;
                inputHandler.OnSprintStart += movementSystem.StartSprinting;
                inputHandler.OnSprintEnd += movementSystem.StopSprinting;
                inputHandler.OnCrouchStart += HandleCrouchInput;
                inputHandler.OnCrouchEnd += movementSystem.StopCrouching;
                inputHandler.OnFire += weaponSystem.Fire;
                inputHandler.OnReload += weaponSystem.Reload;
                inputHandler.OnSwitchWeapon += weaponSystem.SwitchWeapon;
                inputHandler.OnLeanLeft += leanSystem.LeanLeft;
                inputHandler.OnLeanRight += leanSystem.LeanRight;
                inputHandler.OnLeanReset += leanSystem.ResetLean;
                inputHandler.OnInteract += TryInteract;
            }
        }
        
        private void OnDisable()
        {
            if (inputHandler != null)
            {
                // Disconnect input events
                inputHandler.OnMove -= movementSystem.SetMovementInput;
                inputHandler.OnLook -= cameraSystem.HandleLookInput;
                inputHandler.OnJump -= movementSystem.Jump;
                inputHandler.OnSprintStart -= movementSystem.StartSprinting;
                inputHandler.OnSprintEnd -= movementSystem.StopSprinting;
                inputHandler.OnCrouchStart -= HandleCrouchInput;
                inputHandler.OnCrouchEnd -= movementSystem.StopCrouching;
                inputHandler.OnFire -= weaponSystem.Fire;
                inputHandler.OnReload -= weaponSystem.Reload;
                inputHandler.OnSwitchWeapon -= weaponSystem.SwitchWeapon;
                inputHandler.OnLeanLeft -= leanSystem.LeanLeft;
                inputHandler.OnLeanRight -= leanSystem.LeanRight;
                inputHandler.OnLeanReset -= leanSystem.ResetLean;
                inputHandler.OnInteract -= TryInteract;
            }
        }
        
        private void HandleCrouchInput()
        {
            // If sprinting, initiate a slide; otherwise just crouch
            if (movementSystem.IsSprinting)
            {
                movementSystem.StartSliding();
            }
            else
            {
                movementSystem.StartCrouching();
            }
        }
        
        private void HandlePlayerDeath()
        {
            // Disable player controls
            SetPlayerActive(false);
            
            // Notify any listeners
            OnPlayerDeath?.Invoke();
            
            // Additional death handling (e.g., ragdoll, death animation, UI)
            Debug.Log("Player has died");
        }
        
        private void TryInteract()
        {
            if (Physics.Raycast(cameraSystem.transform.position, cameraSystem.transform.forward, out RaycastHit hit, interactionRange))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                interactable?.Interact(this);
            }
        }
        
        public void SetPlayerActive(bool active)
        {
            isPlayerActive = active;
            
            // Enable/disable subsystems
            movementSystem.enabled = active;
            cameraSystem.enabled = active;
            
            // Lock/unlock cursor based on active state
            Cursor.lockState = active ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !active;
        }
        
        // Interface for other systems to access player subsystems
        public HealthSystem Health => healthSystem;
        public MovementSystem Movement => movementSystem;
        public InventorySystem Inventory => inventorySystem;
        public WeaponSystem Weapons => weaponSystem;
        public CameraSystem Camera => cameraSystem;
    }
    
    // Interface for interactable objects
    public interface IInteractable
    {
        void Interact(PlayerController player);
    }
}
