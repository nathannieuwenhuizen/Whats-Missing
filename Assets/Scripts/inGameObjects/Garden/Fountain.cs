using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fountain : RoomObject
{

    [SerializeField]
    private Animator animator;

    private SFXInstance waterSound;

    [SerializeField]
    private ParticleSystem waterParticles;
    private ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime;


    void Awake()
    {
        velocityOverLifetime = waterParticles.velocityOverLifetime;
    }
    public Fountain () {
        largeScale = 1.8f;
    }

    public override void OnEnlargingFinish()
    {
        base.OnEnlargingFinish();
        waterParticles.transform.localScale = Vector3.one * largeScale;
    }

    public override void OnEnlargeRevertFinish()
    {
        base.OnEnlargeRevertFinish();
        waterParticles.transform.localScale = Vector3.one * normalScale;
    }

    public override void OnRoomEnter()
    {
        base.OnRoomEnter();
        if (waterSound == null) {
            waterSound =  AudioHandler.Instance.Play3DSound(SFXFiles.fountain, transform, .2f, 1f, true, true, 30);
        }
        waterSound.Play();
    }
    public override void OnRoomLeave()
    {
        base.OnRoomLeave();
        if (waterSound != null)
            waterSound.Pause();

    }
    
    private void FasterWind() {
        velocityOverLifetime.x = 10f;
    }
    private void NormalWind() {
        velocityOverLifetime.x = 0f;
    }

    private void OnEnable() {
        Wind.OnWindEnlarged += FasterWind;
        Wind.OnWindNormal += NormalWind;
    }

    private void OnDisable() {
        Wind.OnWindEnlarged -= FasterWind;
        Wind.OnWindNormal -= NormalWind;        
        if (waterSound != null)
            waterSound.Stop(true);

    }

    

    private void Reset() {
        Word = "fountain";
        AlternativeWords = new string[] { "fountains", "spring" };
    }

}
