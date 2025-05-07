using Snowy.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SnInput
{
    public abstract class InputManagerBase : MonoSingleton<InputManagerBase>
    {
#pragma warning disable 67

        public override bool DestroyOnLoad => false;

        // delegates
        public delegate void OnControlsChangedDelegate(PlayerInput playerInput);
        public delegate void OnContextDelegate(InputAction.CallbackContext context);

        // static events
        public static event OnControlsChangedDelegate onControlsChanged;
        
        // properties
    
        public PlayerInput PlayerInput { get; private set; }
        
        // end warning ignore
#pragma warning restore 67
        
        protected override void Awake()
        {
            base.Awake();

            PlayerInput = GetComponent<PlayerInput>();

            if (PlayerInput == null)
            {
                Debug.LogError("PlayerInput component not found.");
            }

            PlayerInput.onControlsChanged += OnControlsChanged;
        
            // switch input behavior
            PlayerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
            PlayerInput.actions.Enable();
            PlayerInput.onActionTriggered += OnActionTriggered;
        }
        
        protected virtual void OnControlsChanged(PlayerInput playerInput)
        {
            Debug.Log("Controls changed to: " + playerInput.currentControlScheme);
            onControlsChanged?.Invoke(playerInput);
        }

        protected abstract void OnActionTriggered(InputAction.CallbackContext context);
    
        public virtual void SwitchToUIControls()
        {
            PlayerInput.SwitchCurrentActionMap("UI");
        }
    
        public virtual void SwitchToGameControls()
        {
            PlayerInput.SwitchCurrentActionMap("Gameplay");
        }
    }
}