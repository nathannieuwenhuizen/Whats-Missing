using System.Collections.Generic;
using UnityEngine;


///<summary>
/// An SFX instance is an gameObject that has the clip and the enum the clip is connected to.
///</summary>
[System.Serializable]
public class SFXInstance
{
    [SerializeField]
    public string name;
    [HideInInspector]
    public AudioSource AudioSource;
    public AudioClip[] Clip;

    public FMOD.Studio.EventInstance FMODInstance;
    // [HideInInspector]
    public bool isFMOD = false;
    private float oldFMODVolume;

    public float Volume {
        get { 
            if (isFMOD) {
                float result = 0f;
                FMODInstance.getVolume(out result);
                return result;
            }
            return AudioSource.volume;
        }
        set { 
            if (isFMOD) FMODInstance.setVolume(value);
            else AudioSource.volume = value * AudioSetting.SFX;
        }
    }
    public float Pitch {
        get { 
            if (isFMOD) {
                float result = 0f;
                FMODInstance.getPitch(out result);
                return result;
            }
            return AudioSource.pitch;
        }
        set { 
            if (isFMOD) FMODInstance.setPitch(value);
            else AudioSource.pitch = value * AudioSetting.SFX;
        }
    }

    public void Play() {
        if (isFMOD) {
            FMODInstance.setPaused(false);
            FMODInstance.start();
        } else {
            AudioSource.Play();
        }
    }

    public void Pause () {
        if (isFMOD) {
            FMODInstance.setPaused(true);
        } else {
            AudioSource.Pause();
        }

    }
    public void Stop (bool withDestroy = false) {
        if (isFMOD) {
            FMODInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            // if (withDestroy) FMODInstance.release();
        } else {
            if (AudioSource != null) AudioSource.Stop();
        }

    }

    public bool Mute {
        set {
            if (isFMOD) {
                if (value) {
                    float temp;
                    FMODInstance.getVolume(out temp);
                    if (temp != 0) oldFMODVolume = temp;
                }
                FMODInstance.setVolume(value ? 0 : oldFMODVolume);
            } else {
                AudioSource.mute = value;
            }

        }
    }
    public AudioClip GetClip
    {
        get
        {
            if (Clip.Length <= 1)
            {
                return Clip[0];
            }
            else
            {
                return Clip[Random.Range(0, Clip.Length)]; //return random sound
            }
        }
    }

}

