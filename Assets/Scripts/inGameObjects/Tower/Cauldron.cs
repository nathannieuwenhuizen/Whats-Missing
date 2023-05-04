using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cauldron : RoomObject
{
    public Transform  audioPoint;
    SFXInstance boilingSound;
    public override void OnRoomEnter()
    {
        base.OnRoomEnter();
        PlaySound();
    }
    public override void OnRoomLeave()
    {
        base.OnRoomLeave();
        StopSound();
    }

    public void PlaySound() {
        if (boilingSound == null) {
            boilingSound =  AudioHandler.Instance.Play3DSound(SFXFiles.cauldron, audioPoint, .2f, 1f, true, true, 10f);
        }
        boilingSound.Play();
    }

    public void StopSound() {
        if (boilingSound != null) {
            boilingSound.Pause();
        }
    }

    public override void OnMissingFinish()
    {
        StopSound();
        base.OnMissingFinish();
    }

    public override void OnAppearing()
    {
        base.OnAppearing();
        PlaySound();
    }


    private void Reset() {
        Word = "cauldron";
        AlternativeWords = new string[] { "cauldrons" };
    }
}
