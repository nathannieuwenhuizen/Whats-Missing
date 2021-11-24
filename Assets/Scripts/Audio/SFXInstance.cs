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

    public float Volume {
        get { return AudioSource.volume;}
        set { AudioSource.volume = value * AudioSetting.SFX; }
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

