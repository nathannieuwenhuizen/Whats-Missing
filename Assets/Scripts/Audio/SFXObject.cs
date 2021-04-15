using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class SFXObject : MonoBehaviour
{
    public static float SFXVolume = 1f;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Play(AudioClip clip = null, float volume = 1f, bool force = true) {
        if (audioSource == null) return;
        if (audioSource.isPlaying && force == false) return;

        if (clip != null) audioSource.clip = clip;
        if (audioSource.clip == null) {
            Debug.LogWarning("No sound clip found at " + gameObject.name);
            return;
        }

        audioSource.volume = volume * SFXObject.SFXVolume;
        audioSource.Play();
    }
}
