﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioHandler : Singleton<AudioHandler>
{

    private Coroutine audioListenerCoroutine;

    [SerializeField]
    private AudioLibrary[] libraries;
    
    private List<SFXInstance> soundEffectInstances;

    private List<MusicInstance> musicClips;

    [HideInInspector]
    public AudioSource MusicSource;
    private bool musicIsFading = false;

    public float MusicVolume {
        get => musicVolume;
        set {
            musicVolume = value;
            MusicSource.volume = value * AudioSetting.MUSIC;
        }
    }
    public float pitchMultiplier = 1f;
    private float musicVolume = 1f;

    private bool mute = false;
    public bool Mute
    {
        set
        {
            mute = value;
        }
        get
        {
            return mute;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        MusicSource = GetComponent<AudioSource>();
        MusicSource.loop = true;

        Initialize();
    }

    public float AudioListenerVolume {
        get { return AudioListener.volume; }
        set { AudioListener.volume = value; }
    }

    public void FadeListener(float val, float duration = .5f) {
        if (audioListenerCoroutine != null) {
            StopCoroutine(audioListenerCoroutine);
        }
        audioListenerCoroutine = StartCoroutine(FadingListener(val, duration));
    }
    private IEnumerator FadingListener(float val, float duration = .5f) {
        float start = AudioListenerVolume;
        float index = 0;
        while ( index < duration) {
            AudioListenerVolume = Mathf.Lerp(start, val , index / duration);
            index += Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }
        AudioListenerVolume = val;
    }

    ///<summary>
    /// Initializes all the audioclips into child objects with audiosources.
    ///</summary>
    private void Initialize()
    {
        soundEffectInstances = new List<SFXInstance>();
        musicClips = new List<MusicInstance>();

        foreach(AudioLibrary libary in libraries) {
            
            foreach (SFXInstance sfx in libary.soundEffectInstances)
            {
                InitializeAudioSource(sfx);
            }
            foreach (MusicInstance music in libary.musicClips)
            {
                if (!musicClips.Contains(music)) {
                    musicClips.Add(music);
                }
            }
        }
    }

    ///<summary>
    /// Creates and induvidual audioSource and adds it to the list of the handler.
    ///</summary>
    private void InitializeAudioSource(SFXInstance sfx) {
        // if (soundEffectInstances.Contains(sfx)) return;

        if (sfx.Clip.Length > 0)
        {
            soundEffectInstances.Add(sfx);
            GameObject temp = new GameObject(sfx.Clip[0].name);
            temp.transform.parent = transform;
            sfx.AudioSource = temp.AddComponent<AudioSource>();
            sfx.AudioSource.playOnAwake = false;
            sfx.AudioSource.loop = false;
            temp.SetActive(false);
        }
    }


    /// <summary>
    ///Plays a 2D sound in the game.
    ///</summary>
    public SFXInstance PlaySound(string key, float volume = 1f, float pitch = 1f, bool loop = false, bool ignoreListenerVolume = false)
    {
        SFXInstance selectedAudio = GetSFXInstance(key);
        if (selectedAudio == default(SFXInstance)) return selectedAudio;
        selectedAudio.AudioSource.gameObject.SetActive(true);
        selectedAudio.AudioSource.spatialBlend = 0;
        selectedAudio.AudioSource.clip = selectedAudio.GetClip;
        selectedAudio.AudioSource.volume = volume * AudioSetting.SFX;
        selectedAudio.AudioSource.pitch = pitch * pitchMultiplier;
        selectedAudio.AudioSource.loop = loop;
        selectedAudio.AudioSource.ignoreListenerVolume = ignoreListenerVolume;
        selectedAudio.AudioSource.Play();
        return selectedAudio;
    }

    ///<summary>
    /// Plays an UI sound that ignores the listener volume.
    ///</summary>
    public SFXInstance PlayUISound(string key, float volume = 1f, float pitch = 1f) {
        return PlaySound(key, volume, pitch, false, true);
    }

    private SFXInstance GetSFXInstance(string key) {
        SFXInstance selectedAudio = soundEffectInstances.Find(x => x.name == key);
        if (selectedAudio == null || mute) {
            Debug.LogWarning("AudioEffect with key " + key + " is null or mute");
            return default(SFXInstance);
        }
        if (selectedAudio.AudioSource == null) {
            Debug.LogError("AudioSource with key " + key + " is null");
            return default(SFXInstance);
        }
        return selectedAudio;
    }
    public SFXInstance Play3DSound(string key, Transform parent, float volume = 1f, float pitch = 1f, bool loop = false, bool asInstance = true, float soundMaxDistance = 100f,  bool ignoreListenerVolume = false) {
        SFXInstance selectedAudio = GetSFXInstance(key);
        if (selectedAudio == default(SFXInstance)) return selectedAudio;
        
        GameObject instance;
        if (asInstance) {
            instance = Instantiate(selectedAudio.AudioSource.gameObject);
        } else {
            instance = selectedAudio.AudioSource.gameObject;
        }
        SFXInstance newSFWIncstance = new SFXInstance() {AudioSource = instance.GetComponent<AudioSource>(), name = key};

        newSFWIncstance.AudioSource.gameObject.SetActive(true);
        newSFWIncstance.AudioSource.clip = selectedAudio.GetClip;
        newSFWIncstance.AudioSource.volume = volume * AudioSetting.SFX;
        newSFWIncstance.AudioSource.pitch = pitch * pitchMultiplier;
        newSFWIncstance.AudioSource.loop = loop;
        newSFWIncstance.AudioSource.ignoreListenerVolume = ignoreListenerVolume;
        newSFWIncstance.AudioSource.Play();

        instance.transform.SetParent(parent); 
        instance.transform.localPosition = Vector3.zero;
        newSFWIncstance.AudioSource.spatialBlend= 1f;
        newSFWIncstance.AudioSource.maxDistance= soundMaxDistance;
        newSFWIncstance.AudioSource.rolloffMode = AudioRolloffMode.Linear;

        if (asInstance && !loop) {
            Destroy(instance,newSFWIncstance.AudioSource.clip.length);
        }
        return newSFWIncstance;
    }
    public void Stop3DSound(SFXInstance instance, bool destroy = true) {
        instance.AudioSource.Stop();
        if (destroy) Destroy(instance.AudioSource.gameObject);
    }

    ///<summary>
    ///Stops the 2D sound
    ///</summary>
    public void StopSound(string key)
    {
        SFXInstance selectedAudio = soundEffectInstances.Find(x => x.name == key);
        if (selectedAudio == null) 
            return;
        selectedAudio.AudioSource.Stop();
    }
    ///<summary>
    ////Plays music that loops.
    ///</summary>
    public void PlayMusic(MusicFiles music, float volume = 1f)
    {
        if (mute) 
            return;
        MusicInstance selectedAudio = musicClips.Find(x => x.Music == music);
        if (selectedAudio == null) return;
        MusicSource.clip = selectedAudio.Clip;
        musicVolume = volume;
        MusicSource.volume = musicVolume * AudioSetting.MUSIC;
        MusicSource.Play();
    }
    ///<summary>
    ////Stops the music
    ///</summary>
    public void StopMusic()
    {
        MusicSource.Stop();
    }


    ///<summary>
    ///Fades one music to 0 and after that fades the new music in with the same volume.
    ///</summary>
    public void FadeMusic(MusicFiles newMusic, float duration, bool waitForOtherMusictoFadeOut = false)
    {
        // StopAllCoroutines();
        // if (musicIsFading && tempSource != null) {
        //     Destroy(MusicSource);
        //     MusicSource = tempSource;
        //     MusicVolume = 1f;
        // }
        StartCoroutine(FadingMusic(newMusic, duration, waitForOtherMusictoFadeOut));
    }
    private AudioSource tempSource;
    private IEnumerator FadingMusic(MusicFiles music, float totalDuration, bool waitForOtherMusictoFadeOut = false)
    {
        musicIsFading = true;
        //TODO: Reson for the 1 is because of a bug that when you call this function fast, the volume gets to be really low in the end.
        float volume = 1 * AudioSetting.MUSIC;// MusicSource.volume * AudioSetting.MUSIC;
        AudioSource tempSource = default(AudioSource);
        if (waitForOtherMusictoFadeOut) yield return StartCoroutine(FadeVolume(MusicSource, MusicSource.volume, 0, totalDuration / 2f));
        else {
            StartCoroutine(FadeVolume(MusicSource, MusicSource.volume, 0, totalDuration / 2f));
        }
            

        MusicInstance selectedMusic = musicClips.Find(x => x.Music == music);
        if (selectedMusic != null)
        {
            if (!waitForOtherMusictoFadeOut) {
                Debug.Log("create other");
                tempSource = gameObject.AddComponent<AudioSource>();
                tempSource.loop = true;
                tempSource.clip = selectedMusic.Clip;
                tempSource.time = MusicSource.time;
                tempSource.Play();
                tempSource.time = MusicSource.time;
                yield return StartCoroutine(FadeVolume(tempSource, 0, volume, totalDuration / 2f));
                Destroy(MusicSource);
                MusicSource = tempSource;
            } else {
                MusicSource.clip = selectedMusic.Clip;
                MusicSource.Play();
                yield return StartCoroutine(FadeVolume(MusicSource, 0, volume, totalDuration / 2f));
            }
        }
        musicIsFading = false;
    }

    ///<summary>
    ///Internal enumerator that fades a AudooSource clip.
    ///</summary>
    public IEnumerator FadeVolume(AudioSource audioS, float begin, float end, float duration)
    {
        float index = 0;

        while (index < duration)
        {
            index += Time.deltaTime;
            audioS.volume = Mathf.Lerp(begin, end * AudioSetting.MUSIC, index / duration);
            yield return new WaitForFixedUpdate();
        }
        audioS.volume = end * AudioSetting.MUSIC;
    }
}
