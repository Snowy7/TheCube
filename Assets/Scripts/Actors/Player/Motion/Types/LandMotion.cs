

using System;
using Snowy;
using SnTerminal;
using UnityEngine;

namespace Actors.Player
{
    /// <summary>
    /// LandMotion. This component plays the landing curves when a character lands.
    /// </summary>
    public class LandMotion: Motion
    {
        #region FIELDS SERIALIZED
        
        [Tooltip("Reference to the character's FeelManager component.")]
        [SerializeField, NotNull]
        private FeelManager feelManager;

        [Tooltip("Reference to the character's MovementBehaviour component.")]
        [SerializeField, NotNull]
        private Movement movementBehaviour;
        
        [Tooltip("The character's Animator component.")]
        [SerializeField, NotNull]
        private Animator characterAnimator;

        [Title("Settings")]

        [Tooltip("The type of this motion.")]
        [SerializeField]
        private MotionType motionType;
        
        #endregion
        
        #region FIELDS
        
        /// <summary>
        /// The location spring.
        /// </summary>
        private readonly Spring springLocation = new Spring();
        /// <summary>
        /// The rotation spring.
        /// </summary>
        private readonly Spring springRotation = new Spring();

        /// <summary>
        /// Represents the curves currently being played by this component.
        /// </summary>
        private ACurves playedCurves;

        /// <summary>
        /// Time.time at which the character last landed on the ground.
        /// </summary>
        private float landingTime;
        
        #endregion
        
        #region METHODS

        private void Start()
        {
            if (movementBehaviour != null)
                movementBehaviour.OnLand += OnLand;
        }
        
        private void OnLand()
        {
            //Set the landing time.
            landingTime = Time.time;
        }

        /// <summary>
        /// Tick.
        /// </summary>
        public override void Tick()
        {
            //Check References.
            if (feelManager == null || movementBehaviour == null)
            {
                //ReferenceError.
                Terminal.Log(TerminalLogType.Error, "ReferenceError", this);

                //Return.
                return;
            }
            
            //Get Feel.
            Feel feel = feelManager.Preset.GetFeel(motionType);
            if (feel == null)
            {
                //ReferenceError.
                Terminal.Log(TerminalLogType.Error, "ReferenceError", this);
                
                //Return.
                return;
            }
            
            //Location.
            Vector3 location = default;
            //Rotation.
            Vector3 rotation = default;

            //We start playing the landing curves.
            playedCurves = feel.GetState(characterAnimator).LandingCurves;

            //Time where we evaluate the landing curves.
            float evaluateTime = Time.time - landingTime;
                
            //Evaluate Location Curves.
            location += playedCurves.LocationCurves.EvaluateCurves(evaluateTime);
            //Evaluate Rotation Curves.
            rotation += playedCurves.RotationCurves.EvaluateCurves(evaluateTime);

            //Evaluate Location Curves.
            springLocation.UpdateEndValue(location);
            //Evaluate Rotation Curves.
            springRotation.UpdateEndValue(rotation);
        }
        
        #endregion
        
        #region FUNCTIONS

        /// <summary>
        /// GetLocation.
        /// </summary>
        public override Vector3 GetLocation()
        {
            //Check References.
            if (playedCurves == null)
                return default;

            //Return.
            return springLocation.Evaluate(playedCurves.LocationSpring);
        }
        /// <summary>
        /// GetEulerAngles.
        /// </summary>
        public override Vector3 GetEulerAngles()
        {
            //Check References.
            if (playedCurves == null)
                return default;
            
            //Return.
            return springRotation.Evaluate(playedCurves.RotationSpring);
        }
        
        #endregion
    }
}