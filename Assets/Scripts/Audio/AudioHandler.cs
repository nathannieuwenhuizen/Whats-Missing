using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioHandler : Singleton<AudioHandler>
{

    private IAudioManager am;

    [SerializeField]
    private AudioLibrary[] libraries;
    


    public float MusicVolume {
        get => am.MusicVolume;
        set {
            am.MusicVolume = value;
        }
    }

    public float PitchMultiplier {
        get => am.pitchMultiplier;
        set => am.pitchMultiplier = value;
    }
    public IAudioManager AudioManager {
        get => am;
    }

    protected override void Awake()
    {
        base.Awake();
        am = gameObject.AddComponent<FMODAudioManager>();
        am.Initialize(libraries);
    }


    public void FadeListener(float val, float duration = .5f) {
        am.FadeListener(val, duration);
    }

    /// <summary>
    ///Plays a 2D sound in the game.
    ///</summary>
    public SFXInstance PlaySound(string key, float volume = 1f, float pitch = 1f, bool loop = false, bool ignoreListenerVolume = false)
    {
        return am.PlaySound(key, volume, pitch, loop, ignoreListenerVolume);
    }

    ///<summary>
    /// Plays an UI sound that ignores the listener volume.
    ///</summary>
    public SFXInstance PlayUISound(string key, float volume = 1f, float pitch = 1f) {
        return am.PlayUISound(key, volume, pitch);
    }

    public SFXInstance Play3DSound(string key, Transform parent, float volume = 1f, float pitch = 1f, bool loop = false, bool asInstance = true, float soundMaxDistance = 100f,  bool ignoreListenerVolume = false) {
        return am.Play3DSound(key, parent, volume, pitch, loop, asInstance, soundMaxDistance, ignoreListenerVolume);
    }
    public void Stop3DSound(SFXInstance instance, bool destroy = true) {
        am.Stop3DSound(instance, destroy);
    }

    ///<summary>
    ///Stops the 2D sound
    ///</summary>
    public void StopSound(string key)
    {
        am.StopSound(key);
    }
    ///<summary>
    ////Plays music that loops.
    ///</summary>
    public void PlayMusic(MusicFiles music, float volume = 1f)
    {
        am.PlayMusic(music, volume);
    }
    ///<summary>
    ////Stops the music
    ///</summary>
    public void StopMusic()
    {
        am.StopMusic();
    }


    ///<summary>
    ///Fades one music to 0 and after that fades the new music in with the same volume.
    ///</summary>
    public void FadeMusic(MusicFiles newMusic, float duration, bool waitForOtherMusictoFadeOut = false)
    {
        am.FadeMusic(newMusic, duration, waitForOtherMusictoFadeOut);
    }

    public void PauseMusic() {
        am.PauseMusic();
    }

    public void ResumeMusic() {
        am.ResumeMusic();
    }

    ///<summary>
    ///Internal enumerator that fades a AudooSource clip.
    ///</summary>
    public IEnumerator FadeVolume(AudioSource audioS, float begin, float end, float duration)
    {
        yield return am.FadeVolume(audioS, begin, end, duration);
    }

    public IEnumerator FadeMusicVolume(float end, float duration)
    {
        yield return am.FadeMusicVolume(end, duration);
    }

}
