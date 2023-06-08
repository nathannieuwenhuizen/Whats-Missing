using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rain : RoomObject
{
    private SFXInstance waterSound;
    [SerializeField]
    private Transform audioContainer;
    private Coroutine fadeCoroutine;

    public override void OnRoomEnter()
    {
        Debug.Log("water rain enter");
        base.OnRoomEnter();
        if (waterSound == null) {
            // Debug.Log("water sound should be playing");
            waterSound =  AudioHandler.Instance.Play3DSound(SFXFiles.rain, audioContainer, 1f, 1, true, true, 40f, false);
        }
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(Extensions.AnimateCallBack(0f, 1f, AnimationCurve.EaseInOut(0,0,1,1), (float v) => {
            waterSound.Volume = v;
        }, .5f));
        waterSound.Play();
    }
    public override void OnRoomLeave()
    {
        Debug.Log("water rain leave");
        base.OnRoomLeave();
        if (waterSound != null) {
            // waterSound.Pause();
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(Extensions.AnimateCallBack(1f, 0f, AnimationCurve.EaseInOut(0,0,1,1), (float v) => {
                waterSound.Volume = v;
            }, .5f));
            StartCoroutine(DelayPause());
        }
    }

    public IEnumerator DelayPause() {
        yield return new WaitForSeconds(.5f);
        if (!inSpace) waterSound.Pause();
    }


    private void Reset() {
        Word = "rain";
        AlternativeWords = new string[] { "raining" };
    }
}
