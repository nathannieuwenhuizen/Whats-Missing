using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioHandler : Singleton<AudioHandler>
{

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
        // #if UNITY_EDITOR    // library
        //     libraries = new AudioLibrary[1] { UnityEditor.AssetDatabase.LoadAssetAtPath<AudioLibrary>("Assets/Audio.asset")};
        // #endif // UNITY_EDITOR

        MusicSource = GetComponent<AudioSource>();
        MusicSource.loop = true;

        // mute = SoundSetting.MUTE;

        Initialize();
    }
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
    public void PlaySound(SFXFiles audioEffect, float volume = 1f, float pitch = 1f, bool loop = false)
    {
        SFXInstance selectedAudio = soundEffectInstances.Find(x => x.AudioEffect == audioEffect);
        if (selectedAudio == null || mute) return;
        if (selectedAudio.AudioSource == null) return;
        selectedAudio.AudioSource.gameObject.SetActive(true);
        selectedAudio.AudioSource.spatialBlend = 0;
        selectedAudio.AudioSource.clip = selectedAudio.GetClip;
        selectedAudio.AudioSource.volume = volume * AudioSetting.SFX;
        selectedAudio.AudioSource.pitch = pitch;
        selectedAudio.AudioSource.loop = loop;
        selectedAudio.AudioSource.Play();
    }

    ///<summary>
    ///Stops the 2D sound
    ///</summary>
    public void StopSound(SFXFiles audioEffect)
    {
        SFXInstance selectedAudio = soundEffectInstances.Find(x => x.AudioEffect == audioEffect);
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
        StopAllCoroutines();
        StartCoroutine(FadingMusic(newMusic, duration, waitForOtherMusictoFadeOut));
    }
    private IEnumerator FadingMusic(MusicFiles music, float totalDuration, bool waitForOtherMusictoFadeOut = false)
    {
        musicIsFading = true;
        float volume = MusicSource.volume * AudioSetting.MUSIC;
        AudioSource tempSource = default(AudioSource);
        if (waitForOtherMusictoFadeOut) yield return StartCoroutine(FadeVolume(MusicSource, MusicSource.volume, 0, totalDuration / 2f));
        else {
            StartCoroutine(FadeVolume(MusicSource, MusicSource.volume, 0, totalDuration / 2f));
        }
            

        MusicInstance selectedMusic = musicClips.Find(x => x.Music == music);
        if (selectedMusic != null)
        {
            if (!waitForOtherMusictoFadeOut) {
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
    ///Internal enumerator that fades a music clip.
    ///</summary>
    private IEnumerator FadeVolume(AudioSource audioS, float begin, float end, float duration)
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
