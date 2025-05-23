﻿using Mirror;
using SnTerminal;
using UnityEngine;

namespace Actors.Player
{
    /// <summary>
    /// Camera Height. This component helps the camera always be in the correct location relative
    /// to the current character height. This means that no matter whether the character is crouching
    /// or not, the camera will be at the correct location.
    /// </summary>
    public class CameraHeight : NetworkBehaviour
    {
        #region FIELDS SERIALIZED

        [Header("References")]

        [Tooltip("The Character Controller component.")]
        [SerializeField]
        private CharacterController characterController;
        
        [Header("Settings")]
        [SerializeField] private Vector3 offset = new Vector3(0, 0, 0.19f);
        [Tooltip("The interpolation speed of the camera. Determines how smoothly the camera will " +
                 "transition to its new location.")]
        [SerializeField]
        private float interpolationSpeed = 12.0f;

        #endregion
        
        #region FIELDS
        
        /// <summary>
        /// Current height of the camera.
        /// </summary>
        private float height = 1.8f;
        
        #endregion
        
        #region UNITY

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!isOwned) enabled = false;
        }

        /// <summary>
        /// Update.
        /// </summary>
        private void Update()
        {
            if (!isOwned) return;
            
            //Check for missing references.
            if (characterController == null)
            {
                //Error Message.
                Terminal.Log(TerminalLogType.Error, $"Component {this.name} on GameObject {gameObject.name} has missing references, and will" +
                                                    $"not correctly function. Please fix this so the component can work properly!");
                
                //Return.
                return;
            }
            
            //Calculate the height from the top of the character controller at which to place the camera.
            //We do this in a somewhat lazy way, by just using the default height at which cameras are usually
            //placed.
            float heightTarget = characterController.height * 0.9f;
            //Interpolate the current height to the target height.
            height = Mathf.Lerp(height, heightTarget, interpolationSpeed * Time.deltaTime);

            //Move the camera!
            transform.localPosition = Vector3.up * height + offset;
        }
        
        #endregion
    }
}