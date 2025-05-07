using UnityEngine;

namespace Snowy.UI.Effects
{
    public interface IAudioInteraction
    {
        public AudioClip HoverSound { get; set; }
        public AudioClip ClickSound { get; set; }
    }
}