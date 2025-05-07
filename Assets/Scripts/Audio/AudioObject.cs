using Snowy.Pool;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    public class AudioObject : MonoBehaviour, IPoolable
    {
        private AudioSource m_audioSource;
        public AudioSource audioSource
        {
            get
            {
                if (m_audioSource == null)
                {
                    m_audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
                }

                return m_audioSource;
            }
        }
        
        public void Reinit()
        {
            audioSource.Stop();
            audioSource.clip = null;
            gameObject.SetActive(true);
        }

        public void CleanUp()
        {
            audioSource.Stop();
            audioSource.clip = null;
            gameObject.SetActive(false);
        }
        
        public void SetMixerGroup(AudioMixerGroup mixerGroup)
        {
            audioSource.outputAudioMixerGroup = mixerGroup;
        }
        
        public void Play(AudioClip clip)
        {
            audioSource.clip = clip;
            audioSource.Play();
            audioSource.volume = 1f;
            
            // on complete, return to pool
            if (clip != null)
            {
                Invoke(nameof(ReturnToPool), clip.length);
            }
        }
        
        public void Play(AudioClip clip, float volume)
        {
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.Play();
            
            // on complete, return to pool
            if (clip != null)
            {
                Invoke(nameof(ReturnToPool), clip.length);
            }
        }

        private void ReturnToPool()
        {
            // release
            AudioManager.Instance.ReturnToPool(this);
        }
    }
}