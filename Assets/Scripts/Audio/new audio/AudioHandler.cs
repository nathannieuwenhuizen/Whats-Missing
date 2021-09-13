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
            MusicSource.volume = value;
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
            
            Debug.Log(libary);
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

    public void PlaySound(SFXFiles audioEffect, float volume = 1f, float pitch = 1f, bool loop = false)
    {
        SFXInstance selectedAudio = soundEffectInstances.Find(x => x.AudioEffect == audioEffect);
        if (selectedAudio == null || mute) return;
        if (selectedAudio.AudioSource == null) return;
        selectedAudio.AudioSource.gameObject.SetActive(true);
        selectedAudio.AudioSource.spatialBlend = 0;
        selectedAudio.AudioSource.clip = selectedAudio.GetClip;
        selectedAudio.AudioSource.volume = volume * SoundSetting.SFX;
        selectedAudio.AudioSource.pitch = pitch;
        selectedAudio.AudioSource.loop = loop;
        selectedAudio.AudioSource.Play();
    }

    public void StopSound(SFXFiles audioEffect)
    {
        SFXInstance selectedAudio = soundEffectInstances.Find(x => x.AudioEffect == audioEffect);
        if (selectedAudio == null) 
            return;
        selectedAudio.AudioSource.Stop();
    }

    public void PlayMusic(MusicFiles music, float volume = 1f)
    {
        if (mute) 
            return;
        MusicInstance selectedAudio = musicClips.Find(x => x.Music == music);
        if (selectedAudio == null) return;
        MusicSource.clip = selectedAudio.Clip;
        musicVolume = volume;
        MusicSource.volume = musicVolume * SoundSetting.MUSIC;
        MusicSource.Play();
    }

    public void UpdateMusicVolume() {
        MusicSource.volume = musicVolume * SoundSetting.MUSIC;
    }

    public void StopMusic()
    {
        MusicSource.Stop();
    }

    public void ChangeMusicVolume(float volume)
    {
        MusicSource.volume = volume * SoundSetting.MUSIC;
    }
    public void FadeMusic(MusicFiles newMusic, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(FadingMusic(newMusic, duration));
    }
    private IEnumerator FadingMusic(MusicFiles music, float duration)
    {
        musicIsFading = true;
        float volume = MusicSource.volume * SoundSetting.MUSIC;
        yield return StartCoroutine(ChangeVolume(MusicSource.volume, 0, duration / 2f));
        MusicInstance selectedMusic = musicClips.Find(x => x.Music == music);

        if (selectedMusic != null)
        {
            MusicSource.clip = selectedMusic.Clip;
            MusicSource.Play();
            yield return StartCoroutine(ChangeVolume(0, volume, duration / 2f));
        }
        musicIsFading = false;
    }


    private IEnumerator ChangeVolume(float begin, float end, float duration)
    {
        float index = 0;

        while (index < duration)
        {
            index += Time.deltaTime;
            MusicSource.volume = Mathf.Lerp(begin, end * SoundSetting.MUSIC, index / duration);
            yield return new WaitForFixedUpdate();
        }
        MusicSource.volume = end * SoundSetting.MUSIC;
    }
}

public enum AudioEffect
{
    click,
    colorSelect,
    footSteps,
    buildingAppear,
    buildingDisappear,
    minigame_bedShine,
    minigame_bedSlide,
    minigame_canHit,
    minigame_canThrow,
    minigame_lockersDrawing,
    minigame_popcornCatch,
    minigame_ropePull,
    minigame_roseCatch,
    minigame_treeCatMeow,
    minigame_treeShake,
    minigame_weightliftingPush,
    UI_Popup
}
public enum Music
{
    menu,
    question,
    result
}
