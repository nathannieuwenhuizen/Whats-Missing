using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rain : RoomObject
{
    private SFXInstance waterSound;
    [SerializeField]
    private Transform audioContainer;
    private Coroutine fadeCoroutine;

    [SerializeField]
    private ParticleSystem ps;

    private float intensity = 0f;

    public override void OnRoomEnter()
    {
        base.OnRoomEnter();
        UpdateIntensity();

        if (waterSound == null) {
            // Debug.Log("water sound should be playing");
            waterSound =  AudioHandler.Instance.Play3DSound(SFXFiles.rain, audioContainer, musicVolume(), 1, true, true, 40f, false);
        }
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(Extensions.AnimateCallBack(0f, musicVolume(), AnimationCurve.EaseInOut(0,0,1,1), (float v) => {
            waterSound.Volume = v;
        }, .5f));
        waterSound.Play();
    }

    public float musicVolume() {
        return .1f + intensity * .9f;
    }

    public void UpdateIntensity() {
        intensity = (float)Array.IndexOf(Room.Area.RoomLevels,Room.roomLevel) / (float)Room.Area.RoomLevels.Length;
        ps.emissionRate = 50 + 2000 * intensity;
    }

    public override void OnRoomLeave()
    {
        base.OnRoomLeave();
        if (waterSound != null) {
            // waterSound.Pause();
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(Extensions.AnimateCallBack(musicVolume(), 0f, AnimationCurve.EaseInOut(0,0,1,1), (float v) => {
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
