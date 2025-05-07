using UnityEngine;

namespace Actors.Player
{
    public class TPAnimationEvents : MonoBehaviour
    {
        #region FIELDS

        /// <summary>
        /// Character Component Reference.
        /// </summary>
        private FPSCharacter playerCharacter;

        #endregion
        
        #region UNITY

        private void Awake()
        {
            //Grab a reference to the character component.
            playerCharacter = GetComponentInParent<FPSCharacter>();
        }

        #endregion
		
        #region ANIMATION
        
        private void OnAmmunitionFill(int amount = 0)
        {
        }

        private void OnGrenade()
        {
        }
        
        /// <summary>
        /// Sets the equipped weapon's magazine to be active or inactive! This function is called from an Animation Event.
        /// </summary>
        private void OnSetActiveMagazine(int active)
        {
            //Notify the character.
            if(playerCharacter != null)
                playerCharacter.SetActiveMagazine(active);
        }
        
        private void OnAnimationEndedBolt()
        {
        }
        
        /// <summary>
        /// Reload Animation Ended. This function is called from an Animation Event.
        /// </summary>
        private void OnAnimationEndedReload()
        {
            //Notify the character.
            if(playerCharacter != null)
                playerCharacter.AnimationEndedReload();
        }

        private void OnAnimationEndedGrenadeThrow()
        {
        }
        private void OnAnimationEndedMelee()
        {
        }

        private void OnAnimationEndedInspect()
        {
        }
        private void OnAnimationEndedHolster()
        {
        }
		
        private void OnEjectCasing()
        {
        }

        private void OnSlideBack()
        {
        }

        private void OnSetActiveKnife()
        {
        }

        /// <summary>
        /// Spawns a magazine! This function is called from an Animation Event.
        /// </summary>
        private void OnDropMagazine(int drop = 0)
        {
            //todo: Drop the magazine.
        }

        #endregion
    }   
}