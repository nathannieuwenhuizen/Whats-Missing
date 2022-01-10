using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fountain : RoomObject
{

    [SerializeField]
    private Animator animator;

    private SFXInstance waterSound;


    void Start()
    {
        UpdateAnimatorTimeScale();
    }
    public Fountain () {
        largeScale = 1.8f;
    }

    public override void OnRoomEnter()
    {
        base.OnRoomEnter();
        if (waterSound == null) {
            waterSound =  AudioHandler.Instance.Play3DSound(SFXFiles.fountain, transform, .2f, 1f, true, true, 30);
        }
        waterSound.AudioSource.Play();
    }
    private void OnEnable() {
        TimeProperty.onTimeMissing += UpdateAnimatorTimeScale;
        TimeProperty.onTimeAppearing += UpdateAnimatorTimeScale;
    }

    private void OnDisable() {
        TimeProperty.onTimeMissing -= UpdateAnimatorTimeScale;
        TimeProperty.onTimeAppearing -= UpdateAnimatorTimeScale;
    }
    public override void OnRoomLeave()
    {
        base.OnRoomLeave();
        if (waterSound != null)
            waterSound.AudioSource.Play();

    }

    private void UpdateAnimatorTimeScale() {
        // animator.speed = Room.TimeScale;
    }

    private void Reset() {
        Word = "fountain";
        AlternativeWords = new string[] { "fountains", "spring" };
    }

}
