using Snowy.Inspector;
using UnityEngine;

namespace Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class Test3DAudio : MonoBehaviour
    {
        public AudioClip clip; //make sure you assign an actual clip here in the inspector

        [InspectorButton("Play", 50f)]
        public void Play()
        {
            AudioSource.PlayClipAtPoint(clip, transform.position);
        }
    }
}