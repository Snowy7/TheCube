

using Audio;
using UnityEngine;

namespace Actors.Player
{
    /// <summary>
    /// Play Sound Behaviour. Plays an AudioClip using our custom AudioManager!
    /// </summary>
    public class PlaySoundBehaviour : StateMachineBehaviour
    {
        #region FIELDS SERIALIZED
        
        [Header("Setup")]
        
        [Tooltip("AudioClip to play!")]
        [SerializeField]
        private AudioClip clip;

        #endregion

        #region UNITY

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            AudioManager.Instance.PlayAudioAtPosition(clip, animator.transform.position);
        }

        #endregion
    }
}