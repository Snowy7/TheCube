using SnTerminal;
using UnityEngine.InputSystem;

namespace SnInput
{
    public class InputManager : InputManagerBase
    {
        // ignore never used warning
#pragma warning disable 67

        // static events
        public static event OnContextDelegate OnChatToggle;

        public static event OnContextDelegate OnLook;
        public static event OnContextDelegate OnMovement;
        public static event OnContextDelegate OnCrouch;
        public static event OnContextDelegate OnAim;
        public static event OnContextDelegate OnJump;
        public static event OnContextDelegate OnRun;
        public static event OnContextDelegate OnFire;
        public static event OnContextDelegate OnReload;
        public static event OnContextDelegate OnInspect;
        public static event OnContextDelegate OnInteract;
        public static event OnContextDelegate OnJoinPress;
        public static event OnContextDelegate OnInventoryNext;
        public static event OnContextDelegate OnInventoryNextWheel;
        public static event OnContextDelegate OnHolster;
        public static event OnContextDelegate OnGrenade;
        public static event OnContextDelegate OnMelee;
        public static event OnContextDelegate OnLockCursor;
        public static event OnContextDelegate OnToggleLaser;
        public static event OnContextDelegate OnToggleNextUI;
        public static event OnContextDelegate OnToggleInventory;
        public static event OnContextDelegate OnTogglePreviousUI;
        public static event OnContextDelegate OnToggleFriendsListUI;

        // end warning ignore
#pragma warning restore 67

        protected override void OnActionTriggered(InputAction.CallbackContext context)
        {
            if (Terminal.Instance && !Terminal.Instance.IsClosed)
                return;

            switch (context.action.name)
            {
                case "JoinPress":
                    OnJoinPress?.Invoke(context);
                    break;
                case "Look":
                    OnLook?.Invoke(context);
                    break;
                case "Movement":
                    OnMovement?.Invoke(context);
                    break;
                case "Fire":
                    OnFire?.Invoke(context);
                    break;
                case "Aim":
                    OnAim?.Invoke(context);
                    break;
                case "Reload":
                    OnReload?.Invoke(context);
                    break;
                case "Holster":
                    OnHolster?.Invoke(context);
                    break;
                case "Jump":
                    OnJump?.Invoke(context);
                    break;
                case "Grenade":
                    OnGrenade?.Invoke(context);
                    break;
                case "Melee":
                    OnMelee?.Invoke(context);
                    break;
                case "Run":
                    OnRun?.Invoke(context);
                    break;
                case "Inspect":
                    OnInspect?.Invoke(context);
                    break;
                case "Lock Cursor":
                    OnLockCursor?.Invoke(context);
                    break;
                case "Inventory Next":
                    OnInventoryNext?.Invoke(context);
                    break;
                case "Inventory Next Wheel":
                    OnInventoryNextWheel?.Invoke(context);
                    break;
                case "Crouch":
                    OnCrouch?.Invoke(context);
                    break;
                case "Toggle Laser":
                    OnToggleLaser?.Invoke(context);
                    break;
                case "Interact":
                    OnInteract?.Invoke(context);
                    break;
                case "ToggleInventory":
                    OnToggleInventory?.Invoke(context);
                    break;
                case "Next":
                    OnToggleNextUI?.Invoke(context);
                    break;
                case "Previous":
                    OnTogglePreviousUI?.Invoke(context);
                    break;
                case "Friends List":
                    OnToggleFriendsListUI?.Invoke(context);
                    break;
                case "OnChatToggle":
                    OnChatToggle?.Invoke(context);
                    break;
            }
        }

    }
}