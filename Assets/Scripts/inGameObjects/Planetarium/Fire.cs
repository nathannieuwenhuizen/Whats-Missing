using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : RoomObject
{

    [SerializeField]
    private MeshRenderer mr;
    private Material material;
    private SFXInstance fireSound;

    protected void Awake() {
        material = mr.material;
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

    public override void OnRoomLeave()
    {
        base.OnRoomLeave();
        StopAllCoroutines();
        if (fireSound != null)
            fireSound.AudioSource.Stop();
    }


    private void Reset() {
        Word = "Fire";
        AlternativeWords = new string[] {"Flame"};
    }
}
