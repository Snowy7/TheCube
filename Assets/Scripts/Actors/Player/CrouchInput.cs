using System;
using Actors.Player;
using SnTerminal;
using UnityEngine;
using UnityEngine.InputSystem;
using SnInput;

namespace Actors.Player
{
    public class CrouchInput : MonoBehaviour
    {
        #region FIELDS SERIALIZED

        [Header("References")]

        [Tooltip("The character's CharacterBehaviour component.")]
        [SerializeField, NotNull]
        private FPSCharacter character;
        
        [Tooltip("The character's MovementBehaviour component.")]
        [SerializeField, NotNull]
        private Movement movement;

        [Header("Settings")]

        [Tooltip("If true, the crouch button has to be held to keep crouching.")]
        [SerializeField]
        private bool holdToCrouch;
        
        #endregion

        #region FIELDS
        
        /// <summary>
        /// holding. If true, the player is holding the crouching button.
        /// </summary>
        private bool holding;

        #endregion

        #region UNITY

        private void Start()
        {
            //Check that all our references are correctly assigned.
            if (character == null || movement == null)
            {
                //ReferenceError.
                Terminal.Log(TerminalLogType.Error, "Reference Error", this, this.gameObject);

                //Return.
                return;
            }
            
        }

        private void OnEnable()
        {
            //Subscribe to the crouch input.
            InputManager.OnCrouch += Crouch;
        }
        
        private void OnDisable()
        {
            //Unsubscribe from the crouch input.
            InputManager.OnCrouch -= Crouch;
        }

        private void OnDestroy()
        {
            //Unsubscribe from the crouch input.
            InputManager.OnCrouch -= Crouch;
        }

        /// <summary>
        /// Update.
        /// </summary>
        private void Update()
        {
            //Change the crouching state based on whether we're holding if we need to.
            //We only do this for hold-crouch, otherwise we don't even bother with this.
            if(holdToCrouch)
                movement.TryCrouch(holding);
        }

        #endregion
        
        #region INPUT

        /// <summary>
        /// Crouch. Calling this from the new Unity Input component will directly make the character
        /// crouch/un-crouch depending on its state.
        /// Keep in mind that this method is called from an input event, so it doesn't have any direct references.
        /// </summary>
        public void Crouch(InputAction.CallbackContext context)
        {
            //Check that all our references are correctly assigned.
            if (character == null || movement == null)
            {
                //ReferenceError.
                Terminal.Log(TerminalLogType.Error, "Reference Error", this, this.gameObject);

                //Return.
                return;
            }
            
            //Block while the cursor is unlocked.
            if (!character.IsCursorLocked())
                return;

            //Switch.
            switch (context.phase)
            {
                //Started.
                case InputActionPhase.Started:
                    holding = true;
                    break;
                //Performed.
                case InputActionPhase.Performed:
                    //TryToggleCrouch.
                    if(!holdToCrouch)
                        movement.TryToggleCrouch();
                    break;
                //Canceled.
                case InputActionPhase.Canceled:
                    holding = false;
                    break;
            }
        }

        #endregion
    }
}