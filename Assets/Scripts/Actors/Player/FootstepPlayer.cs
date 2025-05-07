using Audio;
using UnityEngine;

namespace Actors.Player
{
public class FootstepPlayer : MonoBehaviour
    {
        #region FIELDS SERIALIZED
        
        [Header("References")]

        [Tooltip("The character's Behaviour component.")]
        [SerializeField, NotNull]
        private FPSCharacter character;
        
        [Tooltip("The character's sound manager.")]
        [SerializeField, NotNull]
        private CharacterSounds characterSounds;
        
        [Tooltip("The character's Animator component.")]
        [SerializeField, NotNull]
        private Animator characterAnimator;

        [Header("Footstep Settings")]
        [Tooltip("The delay between each footstep sound when walking.")]
        [SerializeField]
        private float walkFootstepDelay = 0.5f;
        
        [Tooltip("The delay between each footstep sound when running.")]
        [SerializeField]
        private float runFootstepDelay = 0.3f;
        
        [Header("Settings")]
        [Tooltip("Minimum magnitude of the movement velocity at which the audio clips will start playing.")]
        [SerializeField]
        private float minVelocityMagnitude = 1.0f;
        
        #endregion
        
        private float lastFootstepTime;
        
        #region UNITY

        /// <summary>
        /// Update.
        /// </summary>
        private void Update()
        {
            //Check for missing references.
            if (character == null)
            {
                //Reference Error.
                Debug.LogError("Missing reference to the character's Behaviour component.", this);
                
                //Return.
                return;
            }
            
            //Check if we're moving on the ground. We don't need footsteps in the air.
            if (character.IsGrounded() && !character.IsCrouching() && character.GetVelocity().sqrMagnitude > minVelocityMagnitude)
            {
                if (Time.time - lastFootstepTime > GetFootstepDelay())
                {
                    //Play the footstep sound.
                    characterSounds.OnFootstep();
                    
                    //Update the last footstep time.
                    lastFootstepTime = Time.time;
                }
            }
        }

        private float GetFootstepDelay()
        {
            return character.IsRunning() ? runFootstepDelay : walkFootstepDelay;
        }

        #endregion
    }
}