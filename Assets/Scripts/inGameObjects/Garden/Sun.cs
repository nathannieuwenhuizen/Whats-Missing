using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : RoomObject
{
    [SerializeField]
    private DeathTrigger deathTrigger;
    [SerializeField]
    private Room room;

    private SFXInstance fireSound;
    protected Coroutine soundCoroutine;

    protected float shrinkSoundVolume = .1f;
    protected float soundVolume = 1f;

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
                    fireSound = AudioHandler.Instance?.Player3DSound(SFXFiles.sun_burning, transform, shrinkSoundVolume, 1f, true, true, 30);
                fireSound.AudioSource.Play();
            } else {
                fireSound?.AudioSource.Stop();
            }

        }
    }
    public Sun() {
        shrinkScale = 0.01f;
        animationDuration = 20f;
    }


    private void Start() {
        deathTrigger.OnAreaEnterEvent.AddListener(DeathBySun);
        SunEnabled = true;
    }

    private void DeathBySun() {
        room.Animated = false;
        Mirror sunMirror = room.Mirrors.Find((mirror) => mirror.isQuestion == true );
        Debug.Log(sunMirror);
        sunMirror.MirrorCanvas.DeselectLetters();
        sunMirror.Confirm();
        // room.RemoveMirrorChange(sunMirror);
        room.Animated = true;

    }

    public override void OnRoomEnter()
    {
        base.OnRoomEnter();
        normalScale = 1f;
        // sunEnabled = false;
    }
    public override void OnRoomLeave()
    {
        normalScale = shrinkScale;

        sunEnabled = false;
        base.OnRoomLeave();
    }

    public override void OnShrinking()
    {
        base.OnShrinking();
    }
    public override IEnumerator AnimateShrinkRevert()
    {
        if (fireSound != null) soundCoroutine = StartCoroutine(fireSound.AudioSource.FadeSFXVolume(1f, AnimationCurve.EaseInOut(0,0,1,1), animationDuration * .5f));

        return base.AnimateShrinkRevert();
    }
    public override void OnShrinkingFinish()
    {
        base.OnShrinkingFinish();
        if(soundCoroutine != null) StopCoroutine(soundCoroutine);
        if (fireSound != null) fireSound.AudioSource.volume = shrinkSoundVolume;

    }
    


    private void Reset() {
        Word = "sun";
        AlternativeWords = new string[] { "star", "stars", "suns" };
    }

}
