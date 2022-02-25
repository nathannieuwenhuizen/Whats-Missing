using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODAudioManager : MonoBehaviour, IAudioManager
{
    public float MusicVolume { get; set; } = 1;
    public float pitchMultiplier { get; set; } = 1;

    public void FadeMusic(MusicFiles newMusic, float duration, bool waitForOtherMusictoFadeOut = false)
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator FadeMusicVolume(float end, float duration)
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator FadeVolume(AudioSource audioS, float begin, float end, float duration)
    {
        throw new System.NotImplementedException();
    }

    public SFXInstance GetSFXInstance(string key)
    {
        throw new System.NotImplementedException();
    }

    public void Initialize(AudioLibrary[] libraries)
    {
        throw new System.NotImplementedException();
    }

    public void InitializeAudioSource(SFXInstance sfx)
    {
        throw new System.NotImplementedException();
    }

    public void PauseMusic()
    {
        throw new System.NotImplementedException();
    }

    public SFXInstance Play3DSound(string key, Transform parent, float volume = 1, float pitch = 1, bool loop = false, bool asInstance = true, float soundMaxDistance = 100, bool ignoreListenerVolume = false)
    {
        throw new System.NotImplementedException();
    }

    public void PlayMusic(MusicFiles music, float volume = 1)
    {
        throw new System.NotImplementedException();
    }

    public SFXInstance PlaySound(string key, float volume = 1, float pitch = 1, bool loop = false, bool ignoreListenerVolume = false)
    {
        throw new System.NotImplementedException();
    }

    public SFXInstance PlayUISound(string key, float volume = 1, float pitch = 1)
    {
        throw new System.NotImplementedException();
    }

    public void ResumeMusic()
    {
        throw new System.NotImplementedException();
    }

    public void Stop3DSound(SFXInstance instance, bool destroy = true)
    {
        throw new System.NotImplementedException();
    }

    public void StopMusic()
    {
        throw new System.NotImplementedException();
    }

    public void StopSound(string key)
    {
        throw new System.NotImplementedException();
    }
}
