using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
public class FMODAudioManager : MonoBehaviour, IAudioManager
{
    public float MusicVolume { get; set; } = 1;
    public float pitchMultiplier { get; set; } = 1;
    public SFXInstance Music;

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

    private void OnEnable() {
        AudioSetting.OnSoundAltered += UpdateBanks;
    }

    private void OnDisable() {
        AudioSetting.OnSoundAltered -= UpdateBanks;
    }

    public void UpdateBanks() {
        RuntimeManager.GetBus("bus:/SFX").setVolume(AudioSetting.SFX);
        RuntimeManager.GetBus("bus:/UI").setVolume(AudioSetting.SFX);
        RuntimeManager.GetBus("bus:/Music").setVolume(AudioSetting.MUSIC);
    }

    public void PauseMusic()
    {
        if (Music != null) {
            Music.FMODInstance.setPaused(true);
        }
    }

    public void PlayMusic(MusicFiles music, float volume = 1)
    {
        string key = SFXFiles.MENU;

        switch (music) {
            case MusicFiles.planetarium:
            key = SFXFiles.Environment1;
            break;
            case MusicFiles.garden:
            key = SFXFiles.Environment2;
            break;
            case MusicFiles.planetarium_hidden_room:
            key = SFXFiles.Environment1;
            break;
        }
        Music = PlaySound(key, volume);
    }

    public SFXInstance Play3DSound(string key, Transform parent, float volume = 1, float pitch = 1, bool loop = false, bool asInstance = true, float soundMaxDistance = 100, bool ignoreListenerVolume = false)
    {
        SFXInstance instance = PlaySound(key, volume, pitch, loop, ignoreListenerVolume);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(instance.FMODInstance, parent); 
        instance.FMODInstance.start();
        instance.FMODInstance.release();       
        return instance;
    }


    public SFXInstance PlaySound(string key, float volume = 1, float pitch = 1, bool loop = false, bool ignoreListenerVolume = false)
    {
        FMOD.Studio.EventInstance instance = RuntimeManager.CreateInstance(key);
        float test = 1f;
        instance.getParameterByName("test", out test);
        if (pitch != 1) instance.setPitch(pitch);
        if (volume != 1) instance.setVolume(volume);
        instance.start();
        instance.release();
        return new SFXInstance() {FMODInstance = instance, isFMOD = true};
    }

    public SFXInstance PlayUISound(string key, float volume = 1, float pitch = 1)
    {
        RuntimeManager.PlayOneShot(key);
        return null;
    }

    public void ResumeMusic()
    {
        if (Music != null) {
            Music.FMODInstance.setPaused(false);
        }

    }

    public void Stop3DSound(SFXInstance instance, bool destroy = true)
    {
        instance.FMODInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void StopMusic()
    {
        if (Music != null) {
            Music.FMODInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

    }

    public void StopSound(string key)
    {
        
    }
}
