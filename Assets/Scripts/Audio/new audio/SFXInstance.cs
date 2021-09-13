using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SFXInstance
{
    public string name;
    [SerializeField]
    public SFXFiles AudioEffect;
    [HideInInspector]
    public AudioSource AudioSource;
    public AudioClip[] Clip;

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
