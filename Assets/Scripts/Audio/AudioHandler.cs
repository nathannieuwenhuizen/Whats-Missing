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
        SFXInstance selectedAudio = GetSFXInstance(audioEffect);
        if (selectedAudio == default(SFXInstance)) return;
        selectedAudio.AudioSource.gameObject.SetActive(true);
        selectedAudio.AudioSource.spatialBlend = 0;
        selectedAudio.AudioSource.clip = selectedAudio.GetClip;
        selectedAudio.AudioSource.volume = volume * AudioSetting.SFX;
        selectedAudio.AudioSource.pitch = pitch;
        selectedAudio.AudioSource.loop = loop;
        selectedAudio.AudioSource.Play();
    }

    private SFXInstance GetSFXInstance(SFXFiles audioEffect) {
        SFXInstance selectedAudio = soundEffectInstances.Find(x => x.AudioEffect == audioEffect);
        if (selectedAudio == null || mute) {
            Debug.LogWarning("AudioEffect of type" + audioEffect + " is null or mute");
            return default(SFXInstance);
        }
        if (selectedAudio.AudioSource == null) {
            Debug.LogError("AudioSource of the audioeffect" + audioEffect + " is null");
            return default(SFXInstance);
        }
        return selectedAudio;
    }
    public SFXInstance Player3DSound(SFXFiles audioEffect, Transform parent, float volume = 1f, float pitch = 1f, bool loop = false, bool asInstance = true, float soundMaxDistance = 100f) {
        SFXInstance selectedAudio = GetSFXInstance(audioEffect);
        if (selectedAudio == default(SFXInstance)) return selectedAudio;
        
        GameObject instance;
        if (asInstance) {
            instance = Instantiate(selectedAudio.AudioSource.gameObject);
        } else {
            instance = selectedAudio.AudioSource.gameObject;
        }
        SFXInstance newSFWIncstance = new SFXInstance() {AudioSource = instance.GetComponent<AudioSource>(), AudioEffect = audioEffect};

        newSFWIncstance.AudioSource.gameObject.SetActive(true);
        newSFWIncstance.AudioSource.clip = selectedAudio.GetClip;
        newSFWIncstance.AudioSource.volume = volume * AudioSetting.SFX;
        newSFWIncstance.AudioSource.pitch = pitch;
        newSFWIncstance.AudioSource.loop = loop;
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
