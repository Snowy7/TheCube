using Snowy.UI.Effects;
using TMPro;
using UnityEngine;

namespace Snowy.UI
{
    public class SnText : TMP_Text, IAudioInteraction
    {
        public AudioClip HoverSound { get; set; }
        public AudioClip ClickSound { get; set; }
    }
}