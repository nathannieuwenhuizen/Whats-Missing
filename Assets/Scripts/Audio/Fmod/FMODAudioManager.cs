using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
public class FMODAudioManager : MonoBehaviour, IAudioManager
{
    public float MusicVolume { 
       get => Music.Volume;
       set => Music.Volume = value;
    }
    public float pitchMultiplier { get; set; } = 1;
    private float audioListenerVolume = 1f;
    public float AudioListenerVolume { 
        get{
            return audioListenerVolume;
        } set {
            audioListenerVolume = Mathf.Clamp(value, 0, 1);
            UpdateBanks();
        } }

    SFXInstance IAudioManager.Music => Music;

    public SFXInstance Music;

    public void FadeMusic(MusicFiles newMusic, float duration, bool waitForOtherMusictoFadeOut = false)
    {
        StartCoroutine(FadeMusicVolume(newMusic, duration));
    }

    public IEnumerator FadeMusicVolume(MusicFiles newMusicFile, float duration)
    {
        float end = 0;
        float begin = Music.Volume;
        SFXInstance newMusic = PlaySound(GetKey(newMusicFile), end);
        float index = 0;
        while (index < duration)
        {
            index += Time.deltaTime;
            Music.Volume =  Mathf.Lerp(begin, end, index / duration);
            newMusic.Volume =  Mathf.Lerp(end, begin, index / duration);
            yield return new WaitForFixedUpdate();
        }

        Music.Volume = end;
        Music.Stop();
        newMusic.Volume = begin;
        Music = newMusic;
    }

    public IEnumerator FadeVolume(SFXInstance audioS, float begin, float end, float duration)
    {
        float index = 0;

        while (index < duration)
        {
            index += Time.deltaTime;
            audioS.FMODInstance.setVolume(Mathf.Lerp(begin, end, index / duration));
            yield return new WaitForFixedUpdate();
        }
        audioS.FMODInstance.setVolume(end);
    }

    public SFXInstance GetSFXInstance(string key)
    {
        return null;
    }

    public void Initialize(AudioLibrary[] libraries)
    {
        AudioListenerVolume = 1f;
        
    }

    public void InitializeAudioSource(SFXInstance sfx)
    {
        
    }

    private void OnEnable() {
        AudioSetting.OnSoundAltered += UpdateBanks;
    }

    private void OnDisable() {
        AudioSetting.OnSoundAltered -= UpdateBanks;
        if (Music != null) Music.Stop(true);
    }

    public void UpdateBanks() {
        RuntimeManager.GetBus("bus:/SFX").setVolume(AudioSetting.SFX * audioListenerVolume);
        RuntimeManager.GetBus("bus:/UI").setVolume(AudioSetting.SFX);
        RuntimeManager.GetBus("bus:/Music").setVolume(AudioSetting.MUSIC * audioListenerVolume);
    }

    public void PauseMusic()
    {
        if (Music != null) {
            Music.FMODInstance.setPaused(true);
        }
    }

    public void PlayMusic(MusicFiles music, float volume = 1)
    {
        Music = PlaySound(GetKey(music), volume);
    }

    private string GetKey(MusicFiles music) {
        string key = SFXFiles.MENU;

        switch (music) {
            case MusicFiles.planetarium:
            key = SFXFiles.Environment1;
            break;
            case MusicFiles.garden:
            key = SFXFiles.Environment2;
            break;
            case MusicFiles.tower:
            key = SFXFiles.Environment3;
            break;
            case MusicFiles.planetarium_hidden_room:
            key = SFXFiles.HIDDEN_ROOM;
            break;
            case MusicFiles.boss:
            key = SFXFiles.Environment4;
            break;
            case MusicFiles.EndlessHallway:
            key = SFXFiles.Environment5;
            break;
        }
        return key;
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
        instance.setParameterByName("test", test);
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
        // RuntimeManager.GetBus("bus:SFX").gete
    }

    private Coroutine audioListenerCoroutine;

    public void FadeListener(float val, float duration = 0.5F)
    {
        if (audioListenerCoroutine != null) StopCoroutine(audioListenerCoroutine);
        audioListenerCoroutine = StartCoroutine(FadingListener(val, duration));    
    }

    public IEnumerator FadingListener(float val, float duration = .5f) {
        float start = AudioListenerVolume;
        float index = 0;
        while ( index < duration) {
            AudioListenerVolume = Mathf.Lerp(start, val , index / duration);
            index += Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }
        AudioListenerVolume = val;
    }

    public IEnumerator FadeMusicVolume(float end, float duration)
{
        throw new System.NotImplementedException();
    }

    public void StopAllAudio()
    {
        RuntimeManager.GetBus("bus:/SFX").stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        RuntimeManager.GetBus("bus:/Music").stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

    }
}
