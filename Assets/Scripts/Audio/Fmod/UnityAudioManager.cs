using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityAudioManager : MonoBehaviour, IAudioManager
{
    [HideInInspector]
    public AudioSource MusicSource;

    private float musicVolume = 1f;
    public float MusicVolume {
        get => musicVolume;
        set {
            musicVolume = value;
            MusicSource.volume = value * AudioSetting.MUSIC;
        }
    }

    public float pitchMultiplier { get; set; } = 1f;
    public float AudioListenerVolume {
        get { return AudioListener.volume; }
        set { AudioListener.volume = value; }
    }

    SFXInstance IAudioManager.Music => default(SFXInstance);


    public SFXInstance GetSFXInstance(string key)
    {
        SFXInstance selectedAudio = soundEffectInstances.Find(x => x.name == key);
        if (selectedAudio == null) {
            Debug.LogWarning("AudioEffect with key " + key + " is null");
            return default(SFXInstance);
        }
        if (selectedAudio.AudioSource == null) {
            Debug.LogError("AudioSource with key " + key + " is null");
            return default(SFXInstance);
        }
        return selectedAudio;
    }

    private List<SFXInstance> soundEffectInstances;
    private List<MusicInstance> musicClips;


    public void Initialize(AudioLibrary[] libraries)
    {
        MusicSource = gameObject.AddComponent<AudioSource>();
        MusicSource.loop = true;
        MusicSource.playOnAwake = false;
        MusicSource.loop = true;

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

    public void InitializeAudioSource(SFXInstance sfx)
    {
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

    public SFXInstance PlaySound(string key, float volume = 1, float pitch = 1, bool loop = false, bool ignoreListenerVolume = false)
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

    public SFXInstance PlayUISound(string key, float volume = 1, float pitch = 1)
    {
        return PlaySound(key, volume, pitch, false, true);
    }

    public SFXInstance Play3DSound(string key, Transform parent, float volume = 1, float pitch = 1, bool loop = false, bool asInstance = true, float soundMaxDistance = 100, bool ignoreListenerVolume = false)
    {
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

    public void Stop3DSound(SFXInstance instance, bool destroy = true)
    {
        instance.AudioSource.Stop();
        if (destroy) Destroy(instance.AudioSource.gameObject);
    }

    public void StopSound(string key)
    {
        SFXInstance selectedAudio = soundEffectInstances.Find(x => x.name == key);
        if (selectedAudio == null) 
            return;
        selectedAudio.AudioSource.Stop();
    }

    public void PlayMusic(MusicFiles music, float volume = 1)
    {
        MusicInstance selectedAudio = musicClips.Find(x => x.Music == music);
        if (selectedAudio == null) return;
        MusicSource.clip = selectedAudio.Clip;
        musicVolume = volume;
        MusicSource.volume = musicVolume * AudioSetting.MUSIC;
        MusicSource.Play();    
    }

    public void StopMusic()
    {
        MusicSource.Stop();
    }

    public void FadeMusic(MusicFiles newMusic, float duration, bool waitForOtherMusictoFadeOut = false)
    {
        StartCoroutine(FadingMusic(newMusic, duration, waitForOtherMusictoFadeOut));
    }
    
    private bool musicIsFading = false;
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
    public IEnumerator FadeVolume(SFXInstance audioS, float begin, float end, float duration)
    {
        yield return FadeVolume(audioS.AudioSource, begin, end, duration);
    }

    public void PauseMusic()
    {
        MusicSource.Pause();
    }

    public void ResumeMusic()
    {
        MusicSource.Play();
    }

    public IEnumerator FadeMusicVolume(float end, float duration)
    {
        yield return FadeVolume(MusicSource, MusicSource.volume, end, duration);
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

    public void StopAllAudio()
    {

    }
}
