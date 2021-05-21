using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicObject : MonoBehaviour
{
    public static float MUCIC_VOLUME = 1f;
    private AudioSource audioSource;

    public static MusicObject INSTANCE;
    public float startVolume;
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        MusicObject.INSTANCE = this;
        Play(audioSource.clip, startVolume);
    }

    public void Play(AudioClip clip = null, float volume = 1f) {
        if (audioSource == null) return;

        if (clip != null) audioSource.clip = clip;

        if (audioSource.clip == null) {
            Debug.LogWarning("No sound clip found at " + gameObject.name);
            return;
        }

        audioSource.volume = volume * MusicObject.MUCIC_VOLUME;
        audioSource.loop = true;
        audioSource.Play();
    }

    public float Volume {
        get => audioSource.volume;
        set => audioSource.volume = value * MusicObject.MUCIC_VOLUME;
    }
    public float Pitch {
        get => audioSource.pitch;
        set => audioSource.pitch = value;
    }

}
