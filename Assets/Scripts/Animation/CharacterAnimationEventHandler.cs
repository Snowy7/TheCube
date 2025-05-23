﻿

using Game;
using UnityEngine;

namespace Actors.Player
{
	/// <summary>
	/// Handles all the animation events that come from the character in the asset.
	/// </summary>
	public class CharacterAnimationEventHandler : MonoBehaviour
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

		/// <summary>
		/// Ejects a casing from the character's equipped weapon. This function is called from an Animation Event.
		/// </summary>
		private void OnEjectCasing()
		{
			//Notify the character.
			if(playerCharacter != null)
				playerCharacter.EjectCasing();
		}

		/// <summary>
		/// Fills the character's equipped weapon's ammunition by a certain amount, or fully if set to 0. This function is called
		/// from a Animation Event.
		/// </summary>
		private void OnAmmunitionFill(int amount = 0)
		{
			//Notify the character.
			if(playerCharacter != null)
				playerCharacter.FillAmmunition(amount);
		}
		/// <summary>
		/// Sets the character's knife active value. This function is called from an Animation Event.
		/// </summary>
		private void OnSetActiveKnife(int active)
		{
			//Notify the character.
			if(playerCharacter != null)
				playerCharacter.SetActiveKnife(active);
		}
		
		/// <summary>
		/// Spawns a grenade at the correct location. This function is called from an Animation Event.
		/// </summary>
		private void OnGrenade()
		{
			//Notify the character.
			if(playerCharacter != null)
				playerCharacter.Grenade();
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

		/// <summary>
		/// Bolt Animation Ended. This function is called from an Animation Event.
		/// </summary>
		private void OnAnimationEndedBolt()
		{
			//Notify the character.
			if(playerCharacter != null)
				playerCharacter.AnimationEndedBolt();
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

		/// <summary>
		/// Grenade Throw Animation Ended. This function is called from an Animation Event.
		/// </summary>
		private void OnAnimationEndedGrenadeThrow()
		{
			//Notify the character.
			if(playerCharacter != null)
				playerCharacter.AnimationEndedGrenadeThrow();
		}
		/// <summary>
		/// Melee Animation Ended. This function is called from an Animation Event.
		/// </summary>
		private void OnAnimationEndedMelee()
		{
			//Notify the character.
			if(playerCharacter != null)
				playerCharacter.AnimationEndedMelee();
		}

		/// <summary>
		/// Inspect Animation Ended. This function is called from an Animation Event.
		/// </summary>
		private void OnAnimationEndedInspect()
		{
			//Notify the character.
			if(playerCharacter != null)
				playerCharacter.AnimationEndedInspect();
		}
		/// <summary>
		/// Holster Animation Ended. This function is called from an Animation Event.
		/// </summary>
		private void OnAnimationEndedHolster()
		{
			//Notify the character.
			if(playerCharacter != null)
				playerCharacter.AnimationEndedHolster();
		}

		/// <summary>
		/// Sets the character's equipped weapon's slide back pose. This function is called from an Animation Event.
		/// </summary>
		private void OnSlideBack(int back)
		{
			//Notify the character.
			if(playerCharacter != null)
				playerCharacter.SetSlideBack(back);
		}

		#endregion
	}   
}