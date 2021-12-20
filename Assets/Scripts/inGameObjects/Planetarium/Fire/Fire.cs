using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : RoomObject
{

    [SerializeField]
    private MeshRenderer mr;
    private Material material;
    private SFXInstance fireSound;

    private float fireSpreadScale;
    [SerializeField]
    private Transform fireSpreadObject;
    [SerializeField]
    private ParticleSystem smokeParticle;
    [SerializeField]
    private GameObject colliderObj;
    [SerializeField]
    private ParticleSystem fireParticle;

    private Coroutine smokeCoroutine;

    protected void Awake() {
        material = mr.material;
        ToggleFireSpread(false);
        normalScale = transform.localScale.x;
        largeScale = normalScale * 2f;
        if (fireSpreadObject != null)
        fireSpreadScale = fireSpreadObject.transform.localScale.x;
    }

    private void OnEnable() {
        TimeProperty.onTimeMissing += UpdateTimeScale;
        TimeProperty.onTimeAppearing += UpdateTimeScale;
        WarmthProperty.OnWarmthMissing += SetFireOff;
        Water.OnWaterBig += SetFireOff;
        WarmthProperty.OnWarmthAppearing += SetFireOn;
        AirProperty.OnAirMissing += SetFireOff;
        AirProperty.OnAirAppearing += SetFireOn;
    }

    private void OnDisable() {
        TimeProperty.onTimeMissing -= UpdateTimeScale;
        TimeProperty.onTimeAppearing -= UpdateTimeScale;
        Water.OnWaterBig -= SetFireOff;
        WarmthProperty.OnWarmthMissing -= SetFireOff;
        WarmthProperty.OnWarmthAppearing -= SetFireOn;
        AirProperty.OnAirMissing -= SetFireOff;
        AirProperty.OnAirAppearing -= SetFireOn;
    }

    public override void OnRoomEnter()
    {
        base.OnRoomEnter();
        if (fireSound == null) {
            fireSound =  AudioHandler.Instance.Play3DSound(SFXFiles.fire_crackling, transform, .5f, 1f, true, true, 15);
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
        if (!InSpace) return;
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
        if (fireSpreadObject == null) return;

        fireSpreadObject.gameObject.SetActive(val);
        smokeParticle.gameObject.SetActive(val);
        colliderObj.gameObject.SetActive(val);
        if (val) {
            smokeParticle.Play();
            fireParticle.Play();
        }
    }

    public override void OnEnlarge()
    {
        if (smokeCoroutine != null) StopCoroutine(smokeCoroutine);

        ToggleFireSpread(true);
        fireSpreadObject.transform.transform.localScale = Vector3.zero;

        base.OnEnlarge();
    }

    public override IEnumerator AnimateEnlarging()
    {
        StartCoroutine(fireSpreadObject.AnimatingScale(Vector3.one * fireSpreadScale, AnimationCurve.EaseInOut(0,0,1,1), animationDuration));
        return base.AnimateEnlarging();
    }
    public override void OnEnlargingFinish()
    {
        fireSpreadObject.localScale = Vector3.one * fireSpreadScale;
        base.OnEnlargingFinish();
    }

    public override void OnEnlargeRevert()
    {
        colliderObj.gameObject.SetActive(false);
        fireParticle.Stop();
        base.OnEnlargeRevert();
    }

    public override void OnEnlargeRevertFinish()
    {
        base.OnEnlargeRevertFinish();
        smokeCoroutine = StartCoroutine(deactivateSmoke());
        // ToggleFireSpread(false);
    }

    private IEnumerator deactivateSmoke() {
        yield return new WaitForSeconds(5f);
        smokeParticle.Stop();
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
