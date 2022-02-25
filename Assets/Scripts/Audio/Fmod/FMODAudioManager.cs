using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
public class FMODAudioManager : MonoBehaviour, IAudioManager
{
    public float MusicVolume { get; set; } = 1;
    public float pitchMultiplier { get; set; } = 1;

    public void FadeMusic(MusicFiles newMusic, float duration, bool waitForOtherMusictoFadeOut = false)
    {
        
    }

    public IEnumerator FadeMusicVolume(float end, float duration)
    {
        return null;
    }

    public IEnumerator FadeVolume(AudioSource audioS, float begin, float end, float duration)
    {
        return null;
    }

    public SFXInstance GetSFXInstance(string key)
    {
        return null;
    }

    public void Initialize(AudioLibrary[] libraries)
    {
        
    }

    public void InitializeAudioSource(SFXInstance sfx)
    {
        
    }

    public void PauseMusic()
    {
        
    }

    public SFXInstance Play3DSound(string key, Transform parent, float volume = 1, float pitch = 1, bool loop = false, bool asInstance = true, float soundMaxDistance = 100, bool ignoreListenerVolume = false)
    {
        if (loop) RuntimeManager.PlayOneShotAttached(key, parent.gameObject);
        else RuntimeManager.PlayOneShot(key, parent.position);
        return null;
    }

    public void PlayMusic(MusicFiles music, float volume = 1)
    {
        // RuntimeManager.PlayOneShot(, transform.position);
    }

    public SFXInstance PlaySound(string key, float volume = 1, float pitch = 1, bool loop = false, bool ignoreListenerVolume = false)
    {
        RuntimeManager.PlayOneShot(key, transform.position);
        return null;
    }

    public SFXInstance PlayUISound(string key, float volume = 1, float pitch = 1)
    {
        RuntimeManager.PlayOneShot(key, transform.position);
        return null;
    }

    public void ResumeMusic()
    {
        
    }

    public void Stop3DSound(SFXInstance instance, bool destroy = true)
    {
        
    }

    public void StopMusic()
    {
        
    }

    public void StopSound(string key)
    {
        
    }
}
