using System;
using System.Collections;
using Actors.AI;
using Snowy.Engine;
using Snowy.Pool;
using Snowy.Utils;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    /// <summary>
    /// AudioType is an enum that represents the different types of audio in the game.
    /// </summary>
    public enum AudioType 
    {
        Master,
        Sfx,
        Music,
        VoiceChat,
        Ambient,
        UI,
        Dialogue,
        Other
    }

    /// <summary>
    /// AudioGroup is a struct that holds the audio type, the audio mixer group and the volume parameter.
    /// </summary>
    [Serializable] public struct AudioGroup
    {
        public AudioType type;
        public AudioMixerGroup mixerGroup;
        public string volumeParameter;
    }
    
    
    /// <summary>
    /// AudioManager is a singleton class that manages the audio in the game.
    /// </summary>
    public class AudioManager : MonoSingleton<AudioManager>
    {
        public override bool DestroyOnLoad => false;

        [Header("General")]
        // The pool size of the audio objects.
        [SerializeField, Range(0, 100)] private int poolSize = 10;
        
        [Header("Audio Groups")]
        // The audio groups that are used in the game.
        [SerializeField] private AudioGroup[] audioGroups;
        
        [Header("Default Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource uiSource;
        
        // The object pool of audio objects.
        private ObjectPool<AudioObject> m_pool;

        /// <summary>
        /// Creates a new audio source.
        /// </summary>
        /// <returns>AudioObject</returns>
        private static AudioObject CreateNewAudioSource()
        {
            var audioSource = new GameObject("AudioObject").AddComponent<AudioSource>().AddComponent<AudioObject>();
            audioSource.transform.SetParent(Instance.transform);
            return audioSource;
        }
        
        protected override void Awake()
        {
            m_pool = new ObjectPool<AudioObject>(CreateNewAudioSource, poolSize);
            
            base.Awake();
        }
        
        /// <summary>
        /// Plays an audio clip.
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="position"></param>
        /// <param name="type"></param>
        public void PlayAudioAtPosition(AudioClip clip, Vector3 position, AudioType type = AudioType.Master)
        {
            AudioObject audioObject = m_pool.Get();
            if (audioObject == null)
            {
                Debug.LogWarning("AudioObject is null.");
                return;
            }
            audioObject.transform.position = position;
            var audioGroup = GetAudioGroup(type);
            audioObject.SetMixerGroup(audioGroup.mixerGroup);
            audioObject.Play(clip);
        }
        
        /// <summary>
        /// Plays an audio clip at a given position.
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="position"></param>
        /// <param name="volume"></param>
        /// <param name="type"></param>
        public void PlayAudioAtPosition(AudioClip clip, Vector3 position, float volume, AudioType type = AudioType.Master)
        {
            AudioObject audioObject = m_pool.Get();
            
            audioObject.transform.position = position;
            var audioGroup = GetAudioGroup(type);
            audioObject.SetMixerGroup(audioGroup.mixerGroup);
            audioObject.Play(clip, volume);
        }
        
        /// <summary>
        /// Plays an audio clip at a given position.
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="volume"></param>
        /// <param name="aSource"></param>
        /// <param name="type"></param>
        public void PlayAudio(AudioClip clip, float volume, AudioSource aSource, AudioType type = AudioType.Master)
        {
            if (aSource == null)
            {
                Debug.LogWarning("AudioSource is null.");
                return;
            }
            var audioGroup = GetAudioGroup(type);
            aSource.Stop();
            aSource.outputAudioMixerGroup = audioGroup.mixerGroup;
            aSource.volume = volume;
            aSource.clip = clip;
            aSource.Play();
        }
        
        /// <summary>
        /// Plays a UI audio clip using the UI audio source.
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="volume"></param>
        public void PlayUIAudio(AudioClip clip, float volume = 1f)
        {
            PlayAudio(clip, volume, uiSource, AudioType.UI);
        }
        
        /// <summary>
        /// Gets the audio group based on the audio type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>AudioGroup</returns>
        private AudioGroup GetAudioGroup(AudioType type)
        {
            foreach (AudioGroup audioGroup in audioGroups)
            {
                if (audioGroup.type == type)
                {
                    return audioGroup;
                }
            }
            
            return audioGroups[0];
        }

        public void PlayAudioAtPositionWithDelay(AudioClip clip, Vector3 transformPosition, float volume, float delay, AudioType type = AudioType.Master)
        {
            StartCoroutine(PlayAudioAtPositionWithDelayCoroutine(clip, transformPosition, volume, delay, type));
        }
        
        IEnumerator PlayAudioAtPositionWithDelayCoroutine(AudioClip clip, Vector3 transformPosition, float volume, float delay, AudioType type = AudioType.Master)
        {
            yield return new WaitForSeconds(delay);
            PlayAudioAtPosition(clip, transformPosition, volume, type);
        }
        
        public void ReturnToPool(AudioObject audioObject)
        {
            m_pool.Release(audioObject);
        }
    }
}