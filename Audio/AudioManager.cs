using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class AudioManager : MonoBehaviour
    {
        #region Singleton
        public static AudioManager Instance;

        private void Awake()
        {

            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
        }
        #endregion

        public float fastMusicFadeSpeed = 0.25f, slowMusicFadeSpeed = 2.0f;

        public Sound[] sounds;

        public AudioSource musicSource;
        public float minMusicVolume = 0.3f;
        public float maxMusicVolume = 0.6f;

        public AudioSource[] allAudioSources;
        List<AudioSource> ambientAudioSources = new List<AudioSource>();

        public AudioMixerGroup musicVolumeMixer, ambientVolumeMixer, sfxVolumeMixer;

        private void Start()
        {
            allAudioSources = GetComponents<AudioSource>();

            foreach (var source in allAudioSources)
            {

                if (source.outputAudioMixerGroup == ambientVolumeMixer)
                {
                    ambientAudioSources.Add(source);
                }

            }

            foreach (Sound s in sounds)
            {
                s.source = Instance.gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop;
                s.source.outputAudioMixerGroup = sfxVolumeMixer;
            }

            StopAll();
            PlayAmbience();
        }

        void PlayAmbience()
        {
            foreach (var ambience in ambientAudioSources)
            {
                if (ambience.isPlaying) return;

                ambience.loop = true;
                ambience.Play();
            }
        }

        public void Play(string name)
        {
            Sound s = Array.Find(sounds, sound => sound.name == name);
            if (s == null)
                return;
            s.source.PlayOneShot(s.clip);
        }

        public void Stop(string name)
        {
            Sound s = Array.Find(sounds, sound => sound.name == name);
            if (s == null)
                return;
            s.source.Stop();
        }

        public void StopAll()
        {
            var allSounds = GetComponents<AudioSource>();
            foreach (var sound in allSounds)
            {
                sound.Stop();
            }

        }
    }
}
