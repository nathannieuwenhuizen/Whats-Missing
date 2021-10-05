using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : RoomObject
{

    SFXInstance Instance;
    public override void OnRoomEnter()
    {
        if (Instance == null) {
            Instance =  AudioHandler.Instance.Player3DSound(SFXFiles.fire_crackling, transform, .5f, 1f, true, true, 15);
        }
            Instance.AudioSource.Play();
    }

    public override void OnRoomLeave()
    {
        if (Instance == null)
            Instance.AudioSource.Stop();
    }


    private void Reset() {
        Word = "Fire";
        AlternativeWords = new string[] {"Flame"};
    }
}
