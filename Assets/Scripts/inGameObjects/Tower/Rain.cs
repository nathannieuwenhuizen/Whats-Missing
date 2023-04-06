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
            waterSound =  AudioHandler.Instance.Play3DSound(SFXFiles.rain, audioContainer, 1f, 1, true, true, 40f, false);
        }
        waterSound.Play();
    }
    public override void OnRoomLeave()
    {
        base.OnRoomLeave();
        if (waterSound != null) {
            // waterSound.Pause();
            StartCoroutine(Extensions.AnimateCallBack(1f, 0f, AnimationCurve.EaseInOut(0,0,1,1), (float v) => {
                waterSound.Volume = v;
            }, .5f));
            StartCoroutine(DelayPause());
        }
    }

    public IEnumerator DelayPause() {
        yield return new WaitForSeconds(.5f);
        waterSound.Pause();
    }


    private void Reset() {
        Word = "rain";
        AlternativeWords = new string[] { "raining" };
    }
}
