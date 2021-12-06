using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : RoomObject
{

    [SerializeField]
    private MeshRenderer mr;
    private Material material;
    private SFXInstance fireSound;

    [SerializeField]
    private FireSpread[] fireSpreads;

    protected void Awake() {
        material = mr.material;
        ToggleFireSpread(false);
        normalScale = transform.localScale.x;
        largeScale = normalScale * 2f;
    }

    private void OnEnable() {
        TimeProperty.onTimeMissing += UpdateTimeScale;
        TimeProperty.onTimeAppearing += UpdateTimeScale;
        WarmthProperty.OnWarmthMissing += SetFireOff;
        WarmthProperty.OnWarmthAppearing += SetFireOn;
        AirProperty.OnAirMissing += SetFireOff;
        AirProperty.OnAirAppearing += SetFireOn;
    }

    private void OnDisable() {
        TimeProperty.onTimeMissing -= UpdateTimeScale;
        TimeProperty.onTimeAppearing -= UpdateTimeScale;
        WarmthProperty.OnWarmthMissing -= SetFireOff;
        WarmthProperty.OnWarmthAppearing -= SetFireOn;
        AirProperty.OnAirMissing -= SetFireOff;
        AirProperty.OnAirAppearing -= SetFireOn;
    }

    public override void OnRoomEnter()
    {
        base.OnRoomEnter();
        if (fireSound == null) {
            fireSound =  AudioHandler.Instance.Player3DSound(SFXFiles.fire_crackling, transform, .5f, 1f, true, true, 15);
        }
        fireSound.AudioSource.Play();
    }

    private void SetFireOn() {
        foreach(Transform go in transform.GetComponentInChildren<Transform>()) {
            go.gameObject.SetActive(true);
        }
        if (fireSound != null)
            fireSound.AudioSource.Pause();

    }
    private void SetFireOff() {
        foreach(Transform go in transform.GetComponentInChildren<Transform>()) {
            go.gameObject.SetActive(false);
        }
        if (fireSound != null)
            fireSound.AudioSource.Play();

    }


    public void UpdateTimeScale() {
        if (InSpace == false) return;

        material.SetFloat("_RoomTime", Room.TimeScale);
        if (fireSound != null){
            fireSound.AudioSource.mute = Room.TimeScale == 0;
        }
    }

    private void ToggleFireSpread(bool val) {
        foreach(FireSpread fireSpread in fireSpreads) {
            fireSpread.gameObject.SetActive(val);
        }
    }

    public override void OnEnlarge()
    {
        ToggleFireSpread(true);
        foreach(FireSpread fireSpread in fireSpreads) {
            fireSpread.transform.transform.localScale = Vector3.zero;
        }

        base.OnEnlarge();
    }

    public override IEnumerator AnimateEnlarging()
    {
        foreach(FireSpread fireSpread in fireSpreads) {
            StartCoroutine(fireSpread.transform.AnimatingScale(Vector3.one, AnimationCurve.EaseInOut(0,0,1,1), animationDuration));
        }
        return base.AnimateEnlarging();
    }
    public override void OnEnlargingFinish()
    {
        foreach(FireSpread fireSpread in fireSpreads) {
            fireSpread.transform.transform.localScale = Vector3.one;
        }
        base.OnEnlargingFinish();
    }

    public override IEnumerator AnimateEnlargeRevert()
    {
        foreach(FireSpread fireSpread in fireSpreads) {
            StartCoroutine(fireSpread.transform.AnimatingScale(Vector3.zero, AnimationCurve.EaseInOut(0,0,1,1), animationDuration));
        }
        return base.AnimateEnlargeRevert();
    }

    public override void OnFlippingRevertFinish()
    {
        base.OnFlippingRevertFinish();
        ToggleFireSpread(false);
    }




    public override void OnRoomLeave()
    {
        base.OnRoomLeave();
        StopAllCoroutines();
        if (fireSound != null)
            fireSound.AudioSource.Stop();
    }


    private void Reset() {
        Word = "fire";
        AlternativeWords = new string[] {"flame"};
    }
}
