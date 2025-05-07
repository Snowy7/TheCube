using UnityEngine;

namespace Snowy.UIAnimator.Scripts
{
    [CreateAssetMenu(fileName = "ButtonClips", menuName = "Snowy/UI/Buttons Audio Clips")]
    public class ButtonClips : ScriptableObject
    {
        public AudioClip buttonClick;
        public AudioClip buttonDisabledClick;
        public AudioClip buttonHover;
    }
}