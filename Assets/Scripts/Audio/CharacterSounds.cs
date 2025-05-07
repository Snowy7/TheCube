using System;
using Actors.Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Audio
{
    [Serializable] public struct TaggedSound
    {
        [TagSelector] public string groundTag;
        public AudioClip[] sounds;
    }
    
    public class CharacterSounds : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private FPSCharacter character;
        [SerializeField] private Collider footstepSourceCollider;
        
        [Header("Footstep Sounds")]
        [SerializeField] private AudioSource footstepSource;
        [SerializeField, ReorderableList] private TaggedSound[] footstepSounds;
        
        [Header("Jump & Land Sounds")]
        [SerializeField] private TaggedSound[] jumpSounds;
        [SerializeField] private TaggedSound[] landSounds;
        
        private string previousGroundTag;


        private void Start()
        {
            character.GetMovementBehaviour().OnLand += OnLand;
            character.GetMovementBehaviour().OnJump += OnJump;
        }

        public void OnFootstep()
        {
            PlayFootstepSound();
        }
        
        private void PlayFootstepSound()
        {
            if (footstepSounds == null || footstepSounds.Length == 0)
                return;
            
            var clip = GetRandomSound(footstepSounds);
            footstepSource.PlayOneShot(clip);
        }
        
        public void OnJump()
        {
            var clip = GetRandomSound(jumpSounds);
            footstepSource.PlayOneShot(clip);
        }
        
        public void OnLand()
        {
            var clip = GetRandomSound(landSounds);
            footstepSource.PlayOneShot(clip);
        }
        
        private AudioClip GetRandomSound(TaggedSound[] sounds)
        {
            var groundTag = character.GetGroundTag();
            var sound = Array.Find(sounds, x => x.groundTag == groundTag).sounds;
            if (sound == null || sound.Length == 0)
                sound = Array.Find(sounds, x => x.groundTag == previousGroundTag).sounds;
            
            if (sound == null || sound.Length == 0)
                return sounds[0].sounds[0];
            
            previousGroundTag = groundTag;
            return sound[Random.Range(0, sound.Length)];
        }
        
    }
}