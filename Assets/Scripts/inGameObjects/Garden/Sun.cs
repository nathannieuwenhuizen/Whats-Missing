using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : RoomObject
{
    [SerializeField]
    private DeathTrigger deathTrigger;
    [SerializeField]
    private Room room;

    [SerializeField]
    private LightningProperty lightningProperty;

    private SFXInstance fireSound;
    protected Coroutine soundCoroutine;

    protected float shrinkSoundVolume = .1f;
    protected float soundVolume = 1f;

    [SerializeField]
    private Light light;
    private float lightIntensityShrink;
    private float lightIntensityLarge;


    public delegate void OnSunShrinkEvent();
    public static OnSunShrinkEvent OnSunShrinking;
    public static OnSunShrinkEvent OnSunShrinkingRevert;
    

    [SerializeField]
    private GameObject sunBody;
    private bool sunEnabled = false;
    public bool SunEnabled {
        get { return sunEnabled;}
        set { 
            // if (sunEnabled == value) return;
            sunEnabled = value; 
            sunBody.SetActive(value);
            if (value) {
                if (fireSound == null) 
                    fireSound = AudioHandler.Instance?.Play3DSound(SFXFiles.sun_burning, transform, shrinkSoundVolume, 1f, true, true, 200);
                fireSound.AudioSource.Play();
            } else {
                fireSound?.AudioSource.Stop();
            }

        }
    }
    public Sun() {
        shrinkScale = 0.02f;
        animationDuration = 20f;
    }

    private void Awake() {
        lightIntensityShrink = light.intensity;
        lightIntensityLarge = lightIntensityShrink * 3f;
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
        StopAllCoroutines();
        normalScale = shrinkScale;
        
        transform.localScale = Vector3.one * normalScale;
        SunEnabled = IsShrinked;
        if (fireSound != null) fireSound.Volume = shrinkSoundVolume;
        light.intensity = lightIntensityShrink;

    }
    public override void OnRoomLeave()
    {
        normalScale = shrinkScale;
        sunEnabled = false;

        lightningProperty.OnAppearingFinish();
        OnSunShrinkingRevert?.Invoke();

        base.OnRoomLeave();
    }

    public override void OnShrinking()
    {
        OnSunShrinking?.Invoke();
        SunEnabled = true;
        base.OnShrinking();
    }
    public override void OnShrinkingFinish()
    {
        base.OnShrinkingFinish();
        if(soundCoroutine != null) StopCoroutine(soundCoroutine);
        if (fireSound != null) fireSound.AudioSource.volume = shrinkSoundVolume;
        light.intensity = lightIntensityShrink;

        lightningProperty.OnMissingFinish();
    }

    public override void OnShrinkRevert()
    {
        normalScale = Animated ? 1f : shrinkScale;
        base.OnShrinkRevert();
    }

    public override void OnShrinkingRevertFinish()
    {
        OnSunShrinkingRevert?.Invoke();
        base.OnShrinkingRevertFinish();
    }

    public override IEnumerator AnimateShrinkRevert()
    {
        if (fireSound != null) soundCoroutine = StartCoroutine(fireSound.AudioSource.FadeSFXVolume(1f, AnimationCurve.EaseInOut(0,0,1,1), animationDuration * .5f));
        StartCoroutine(light.AnimateLightIntensity(lightIntensityLarge, AnimationCurve.EaseInOut(0,0,1,1), animationDuration * .5f));
        return base.AnimateShrinkRevert();
    }


    private void Reset() {
        Word = "sun";
        AlternativeWords = new string[] { "star", "stars", "suns" };
    }

}
