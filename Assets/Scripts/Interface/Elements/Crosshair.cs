﻿using Actors.Player;
using Snowy;
using Snowy.Utils;
using SnTerminal;
using UnityEngine;

namespace Interface.Elements
{
    /// <summary>
    /// Crosshair UI Element. This class makes sure to change the Crosshair at runtime to make sure that it
    /// looks as intended, and functions with the rest of the gameplay elements.
    /// </summary>
    public class Crosshair : Element
    {
        #region FIELDS SERIALIZED

        [Header("References")]

        [Tooltip("Object to which all crosshair pieces are parented.")]
        [SerializeField, NotNull]
        private CanvasGroup crosshairCanvasGroup;

        [Tooltip("Little Dot!")]
        [SerializeField, NotNull]
        private CanvasGroup dotCanvasGroup;
        
        [Tooltip("This is the rect transform of the object that actually gets scaled to make the crosshair " +
                 "look bigger.")]
        [SerializeField, NotNull]
        private RectTransform mainRectTransform;

        [Header("Settings")]
        
        [Tooltip("Minimum and maximum scales for the crosshair.")]
        [SerializeField]
        private Vector2 minMaxScale = new Vector2(50.0f, 200.0f);
        
        [Tooltip("Default size of the crosshair. This is the size at which the crosshair stays when nothing is really " +
                 "happening.")]
        [SerializeField]
        private float defaultScale = 50.0f;
        
        [Header("Interpolation")]

        [Tooltip("Interpolation speed of the crosshair' size.")]
        [SerializeField]
        private float interpolationSpeed = 7.0f;

        [Tooltip("Interpolation speed of the dot's visibility.")]
        [SerializeField]
        private float interpolationSpeedDot = 50.0f;
        
        [Tooltip("Delta size interpolation settings.")]
        [SerializeField]
        private SpringSettings interpolationSizeDelta = SpringSettings.Default();

        [Header("Scale Additions")]
        
        [Tooltip("Value used to increase the crosshair' scale while jumping/falling.")]
        [SerializeField]
        private float jumpingScaleAddition = 50.0f;
        
        [Tooltip("Value used to increase the crosshair' scale while crouching.")]
        [SerializeField]
        private float crouchingScaleAddition = -20.0f;
        
        [Tooltip("Value used to increase the crosshair' scale while moving.")]
        [SerializeField]
        private float movementScaleAddition = 25.0f;

        [Header("Running")]

        [Tooltip("Determines the alpha value of the crosshair while the character is performing some action that disables it.")]
        [SerializeField]
        private float disabledVisibility = 0.6f;

        [Tooltip("Value used to increase the crosshair' scale while running.")]
        [SerializeField]
        private float runningScaleAddition = 15.0f;

        [Header("Spread")]

        [Tooltip("Animation curve dictating how the crosshair scales as the character shoots more and more.")]
        [SerializeField]
        private AnimationCurve spreadIncrease;

        #endregion

        #region FIELDS

        /// <summary>
        /// MovementBehaviour.
        /// </summary>
        private Movement movementBehaviour;

        /// <summary>
        /// Crosshair Local Scale.
        /// </summary>
        private float crosshairLocalScale;
        /// <summary>
        /// Crosshair Visibility.
        /// </summary>
        private float crosshairVisibility;
        /// <summary>
        /// Dot Visibility.
        /// </summary>
        private float dotVisibility;

        /// <summary>
        /// springCrosshairSizeDelta. Spring used to change the Crosshair's delta size.
        /// </summary>
        private Spring springCrosshairSizeDelta;

        #endregion
        
        #region METHODS

        /// <summary>
        /// Awake.
        /// </summary>
        public override void Init(FPSCharacter chr)
        {
            //Base.
            base.Init(chr);

            movementBehaviour = character.GetMovementBehaviour();
            
            //Initialize Spring.
            springCrosshairSizeDelta = new Spring();

            //Visibility.
            crosshairVisibility = 1.0f;
        }

        /// <summary>
        /// Tick.
        /// </summary>
        public  override void Tick()
        {
            //Check for missing references.
            if (crosshairCanvasGroup == null || dotCanvasGroup == null || mainRectTransform == null ||
                character == null)
            {
                //Reference Error.
                Terminal.Log("Reference Error", this, gameObject);
                
                //Return.
                return;
            }
            
            //Cache the Movement Behaviour component if needed.
            if (!movementBehaviour) movementBehaviour ??= character.GetMovementBehaviour();
            if (movementBehaviour == null)
            {
                //Reference Error.
                Terminal.Log("Reference Error", this, gameObject);
                
                //Return.
                return;
            }
            
            // Check if is holding a weapon
            if (!character.HasWeapon())
            {
                // Only show a dot.
                crosshairCanvasGroup.alpha = 0.0f;
                dotCanvasGroup.alpha = 1.0f;
                return;
            }

            //Get the shotsFired value from the character.
            int shotsFired = character.GetShotsFired();

            //Scale of the character's movement. We use this to scale the crosshair while moving around.
            float movementScale = character.GetInputMovement().sqrMagnitude * movementScaleAddition;
            //Size Target. This is the value that we're going to be interpolating to this frame.
            float sizeDeltaTarget = defaultScale + spreadIncrease.Evaluate(shotsFired);

            //Crosshair Scale Target.
            var crosshairLocalScaleTarget = 1.0f;
            //Crosshair Visibility.
            var crosshairVisibilityTarget = 1.0f;
            //Dot Scale Target.
            var dotVisibilityTarget = 1.0f;

            //We completely hide the crosshair, and also the dot, if the character doesn't want it visible.
            //In the future, we will likely change this to all be handled here, and not have any connection
            //to the character itself, but for now this works great.
            if (character.IsAiming())
                crosshairLocalScaleTarget = dotVisibilityTarget = crosshairVisibilityTarget = 0.0f;
            else
            {
                //Check if the character is performing some disabling action.
                bool isPerformingDisablingAction =
                    character.IsInspecting() || character.IsReloading() ||
                    character.IsMeleeing() || character.IsThrowingGrenade();

                //If the character's weapon is lowered, then we certainly do need to disable the crosshair.
                if (character.IsLowered())
                    isPerformingDisablingAction = true;
                        
                //Crosshair Visibility.
                crosshairVisibilityTarget = isPerformingDisablingAction ? disabledVisibility : 1.0f;
                
                sizeDeltaTarget = character.GetWeapon().GetCrosshairSize();
                
                /*//Falling Velocity. We add this to the crosshair's size to increase it while in the air.
                float fallingVelocity = (movementBehaviour.GetVelocity().y >= 0 ? Mathf.Clamp01(Mathf.Abs(movementBehaviour.GetVelocity().y)) : 1) * jumpingScaleAddition;
                //We add the crouching values, to reflect the crosshair's change when the character is crouched.
                sizeDeltaTarget += character.IsCrouching() ? crouchingScaleAddition : 0.0f;

                //Holstered.
                if (character.IsHolstered())
                {
                    //Hide Crosshair.
                    crosshairLocalScaleTarget = crosshairVisibilityTarget = 0.0f;
                    //Show Dot.
                    dotVisibilityTarget = 1.0f;
                }
                else
                {
                    //We check if the character is running.
                    if (character.IsRunning())
                    {
                        //Add falling movement.
                        sizeDeltaTarget += movementBehaviour.IsGrounded() ? default : fallingVelocity;
                    
                        //This changes the crosshair's alpha visibility. We do this because it looks
                        //cool.
                        crosshairVisibilityTarget = disabledVisibility;
                        //Scale.
                        crosshairLocalScaleTarget = 1.0f;
                        //Update the size too!
                        sizeDeltaTarget += runningScaleAddition;
                    }
                    //This checks if the character is just chilling.
                    //And if so, we just let the crosshair be more relaxed.
                    else
                    {
                        //We add either the ground movement, or the falling velocity. This depends on whether the character
                        //is on the ground, or not.
                        sizeDeltaTarget += movementBehaviour.IsGrounded() ? movementScale : fallingVelocity;

                        //Setup.
                        crosshairLocalScaleTarget = dotVisibilityTarget = 1.0f;

                        //Check if the character is performing some disabling action.
                        bool isPerformingDisablingAction =
                            character.IsInspecting() || character.IsReloading() ||
                            character.IsMeleeing() || character.IsThrowingGrenade();

                        //If the character's weapon is lowered, then we certainly do need to disable the crosshair.
                        if (character.IsLowered())
                            isPerformingDisablingAction = true;
                        
                        //Crosshair Visibility.
                        crosshairVisibilityTarget = isPerformingDisablingAction ? disabledVisibility : 1.0f;
                    }   
                }*/
            } 

            //Interpolate the dot visibility value to this frame's target.
            dotVisibility = Mathf.Lerp(dotVisibility, Mathf.Clamp01(dotVisibilityTarget), Time.deltaTime * interpolationSpeedDot);
            //Interpolate the crosshair local scale value to this frame's target.
            crosshairLocalScale = Mathf.Lerp(crosshairLocalScale, Mathf.Clamp01(crosshairLocalScaleTarget), Time.deltaTime * interpolationSpeed);
            //Interpolate the crosshair visibility value to this frame's target.
            crosshairVisibility = Mathf.Lerp(crosshairVisibility, Mathf.Clamp01(crosshairVisibilityTarget), Time.deltaTime * interpolationSpeed);

            //Clamp the crosshair size. Without this, it can get pretty crazy at times!
            sizeDeltaTarget = Mathf.Clamp(sizeDeltaTarget, minMaxScale.x, minMaxScale.y);

            //Update size delta end value.
            springCrosshairSizeDelta.UpdateEndValue(sizeDeltaTarget * Vector3.one);
            
            //Scale.
            mainRectTransform.sizeDelta = springCrosshairSizeDelta.Evaluate(interpolationSizeDelta);
            mainRectTransform.localScale = crosshairLocalScale * Vector3.one;
            
            //Alpha.
            crosshairCanvasGroup.alpha = crosshairVisibility;
            dotCanvasGroup.alpha = dotVisibility;
        }
        
        #endregion
    }
}