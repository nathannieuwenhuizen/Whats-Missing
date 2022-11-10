using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rain : RoomObject
{
    private SFXInstance waterSound;
    [SerializeField]
    private Transform audioContainer;

    public override void OnRoomEnter()
    {
        base.OnRoomEnter();
        if (waterSound == null) {
            // Debug.Log("water sound should be playing");
            waterSound =  AudioHandler.Instance.Play3DSound(SFXFiles.rain, audioContainer);
        }
        waterSound.Play();
    }
    public override void OnRoomLeave()
    {
        base.OnRoomLeave();
        if (waterSound != null)
            waterSound.Pause();
    }
    private void Reset() {
        Word = "rain";
        AlternativeWords = new string[] { "raining" };
    }
}
