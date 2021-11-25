using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : RoomObject
{
    private SFXInstance fireSound;

    [SerializeField]
    private GameObject sunBody;
    private bool sunEnabled = false;
    public bool SunEnabled {
        get { return sunEnabled;}
        set { 
            if (sunEnabled == value) return;
            sunEnabled = value; 
            sunBody.SetActive(value);
            if (value) {
                if (fireSound == null) 
                    fireSound = AudioHandler.Instance?.Player3DSound(SFXFiles.sun_burning, transform, .1f, 1f, true, true, 30);
                fireSound.AudioSource.Play();
            } else {
                fireSound?.AudioSource.Stop();
            }

        }
    }
    private void Start() {
         SunEnabled = true;
    }

    public override void OnRoomEnter()
    {
        base.OnRoomEnter();
        // sunEnabled = false;
    }
    public override void OnRoomLeave()
    {
        sunEnabled = false;
        base.OnRoomLeave();
    }

    public override void OnShrinking()
    {
        base.OnShrinking();
    }


    private void Reset() {
        Word = "sun";
        AlternativeWords = new string[] { "star", "stars", "suns" };
    }

}
