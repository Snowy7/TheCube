using UnityEngine;

namespace Snowy.AI.Sensors
{
    public enum SoundType
    {
        Danger,
        Alert,
        Normal
    }
    
    /// <summary>
    /// Interface for sound receivers.
    /// </summary>
    public interface ISoundReceiver
    {
        /// <summary>
        /// Called when a sound is heard by a random sender.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="soundType"></param>
        void OnSoundHeard(Transform source, SoundType soundType = SoundType.Normal);
    }
}