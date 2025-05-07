

using System;
using Audio;
using Game;
using Inventory;
using UnityEngine;
using Utilities;
using AudioType = Audio.AudioType;

namespace Actors.Player
{
    /// <summary>
    /// Helper StateMachineBehaviour that allows us to more easily play a specific weapon sound.
    /// </summary>
    public class PlaySoundCharacterBehaviour : StateMachineBehaviour
    {
        /// <summary>
        /// Type of weapon sound.
        /// </summary>
        private enum SoundType
        {
            //Character Actions.
            GrenadeThrow, Melee,
            //Holsters.
            Holster, Unholster,
            //Normal Reloads.
            Reload, ReloadEmpty,
            //Cycled Reloads.
            ReloadOpen, ReloadInsert, ReloadClose,
            //Firing.
            Fire, FireEmpty,
            //Bolt.
            BoltAction
        }

        #region FIELDS SERIALIZED

        [Header("Setup")]
        
        [Tooltip("Delay at which the audio is played.")]
        [SerializeField]
        private float delay;
        
        [Tooltip("Type of weapon sound to play.")]
        [SerializeField]
        private SoundType soundType;
        
        [Tooltip("Volume of the audio clip.")]
        [SerializeField, Range(0f, 1f)]
        private float volume = 1f;

        #endregion

        #region FIELDS

        /// <summary>
        /// Player Character.
        /// </summary>
        private FPSCharacter playerCharacter;

        /// <summary>
        /// Player Inventory.
        /// </summary>
        private PlayerInventory playerInventory;

        #endregion
        
        #region UNITY

        /// <summary>
        /// On State Enter.
        /// </summary>
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //We need to get the character component.
            playerCharacter ??= animator.GetComponentInParent<FPSCharacter>();

            //Get Inventory.
            playerInventory ??= playerCharacter.GetInventory();

            //Try to get the equipped weapon's Weapon component.
            if (!(playerInventory.GetEquipped() is { } weaponBehaviour))
                return;

            #region Select Correct Clip To Play

            //Switch.
            AudioClip clip = soundType switch
            {
                //Grenade Throw.
                SoundType.GrenadeThrow => playerCharacter.GetAudioClipsGrenadeThrow().GetRandom(),
                //Melee.
                SoundType.Melee => playerCharacter.GetAudioClipsMelee().GetRandom(),
                
                //Holster.
                SoundType.Holster => weaponBehaviour.GetAudioClipHolster(),
                //Unholster.
                SoundType.Unholster => weaponBehaviour.GetAudioClipUnholster(),
                
                //Reload.
                SoundType.Reload => weaponBehaviour.GetAudioClipReload(),
                //Reload Empty.
                SoundType.ReloadEmpty => weaponBehaviour.GetAudioClipReloadEmpty(),
                
                //Reload Open.
                SoundType.ReloadOpen => weaponBehaviour.GetAudioClipReloadOpen(),
                //Reload Insert.
                SoundType.ReloadInsert => weaponBehaviour.GetAudioClipReloadInsert(),
                //Reload Close.
                SoundType.ReloadClose => weaponBehaviour.GetAudioClipReloadClose(),
                
                //Fire.
                SoundType.Fire => weaponBehaviour.GetAudioClipFire(),
                //Fire Empty.
                SoundType.FireEmpty => weaponBehaviour.GetAudioClipFireEmpty(),
                
                //Bolt Action.
                SoundType.BoltAction => weaponBehaviour.GetAudioClipBoltAction(),
                
                //Default.
                _ => default
            };

            #endregion

            //Play with some delay. Granted, if the delay is set to zero, this will just straight-up play!
            //OLD: SoundManager.Instance.PlayOneShotDelayed(clip, delay, volume);
            AudioManager.Instance.PlayAudioAtPositionWithDelay(clip, animator.transform.position, volume, delay, AudioType.Sfx);
        }
        
        #endregion
    }
}