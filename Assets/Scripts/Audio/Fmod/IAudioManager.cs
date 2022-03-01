using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// Interface for the audio manager
///</summary>
public interface IAudioManager {
    //-----------setup

    ///<summary>
    /// Initializes all the audioclips into child objects with audiosources.
    ///</summary>
    public void Initialize(AudioLibrary[] libraries);
    ///<summary>
    /// Creates and induvidual audioSource and adds it to the list of the handler.
    ///</summary>
    public void InitializeAudioSource(SFXInstance sfx);
    /// <summary>
    ///Plays a 2D sound in the game.
    ///</summary>
    ///<summary>
    /// THe pitch multiplier, only used in the time level so far
    ///</summary>
    public float pitchMultiplier {get; set;}

    ///<summary>
    /// Stops all audio. 
    ///</summary>
    public void StopAllAudio();



    //-----------sound

    public SFXInstance GetSFXInstance(string key);
    public SFXInstance PlaySound(string key, float volume = 1f, float pitch = 1f, bool loop = false, bool ignoreListenerVolume = false);
    ///<summary>
    /// Plays an UI sound that ignores the listener volume.
    ///</summary>
    public SFXInstance PlayUISound(string key, float volume = 1f, float pitch = 1f);    
    public SFXInstance Play3DSound(string key, Transform parent, float volume = 1f, float pitch = 1f, bool loop = false, bool asInstance = true, float soundMaxDistance = 100f,  bool ignoreListenerVolume = false);
    public void Stop3DSound(SFXInstance instance, bool destroy = true);
    ///<summary>
    ///Stops the 2D sound
    ///</summary>
    public void StopSound(string key);
    public IEnumerator FadeVolume(AudioSource audioS, float begin, float end, float duration);

    //-----------music

    ///<summary>
    /// Volume of the music
    ///</summary>
    public float MusicVolume {get; set;}
    ///<summary>
    ///Pauses the music
    ///</summary>
    public void PauseMusic();
    ///<summary>
    /// Resuimes the music, no specified music should be given
    ///</summary>
    public void ResumeMusic();
    ///<summary>
    ////Plays music that loops. Need ot select the music file though
    ///</summary>
    public void PlayMusic(MusicFiles music, float volume = 1f);
    ///<summary>
    ////Stops the music
    ///</summary>
    public void StopMusic();
    ///<summary>
    ///Fades one music to 0 and after that fades the new music in with the same volume.
    ///</summary>
    public void FadeMusic(MusicFiles newMusic, float duration, bool waitForOtherMusictoFadeOut = false);
    ///<summary>
    ///Internal enumerator that fades a AudooSource clip.
    ///</summary>
    public IEnumerator FadeMusicVolume( float end, float duration);

    ///<summary>
    /// Fades the ingame SFX and menu to open/close the pause men
    ///</summary>
    public void FadeListener(float val, float duration = .5f);
    public IEnumerator FadingListener(float val, float duration = .5f);
    public float AudioListenerVolume {get; set;}

}