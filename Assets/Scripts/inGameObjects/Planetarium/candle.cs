using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class candle : RoomObject
{
    [SerializeField]
    private MeshRenderer mr;
    private Material material;

    protected void Awake() {
        material = mr.material;
    }

    private void OnEnable() {
        TimeProperty.onTimeMissing += UpdateTimeScale;
        TimeProperty.onTimeAppearing += UpdateTimeScale;
        TimeProperty.onTimeMissing += UpdateTimeScale;
        TimeProperty.onTimeAppearing += UpdateTimeScale;
        WarmthProperty.OnWarmthMissing += SetFlameOff;
        WarmthProperty.OnWarmthAppearing += SetFlameOn;
        AirProperty.OnAirMissing += SetFlameOff;
        AirProperty.OnAirAppearing += SetFlameOn;

    }

    private void OnDisable() {
        TimeProperty.onTimeMissing -= UpdateTimeScale;
        TimeProperty.onTimeAppearing -= UpdateTimeScale;
        TimeProperty.onTimeMissing -= UpdateTimeScale;
        TimeProperty.onTimeAppearing -= UpdateTimeScale;
        WarmthProperty.OnWarmthMissing -= SetFlameOff;
        WarmthProperty.OnWarmthAppearing -= SetFlameOn;
        AirProperty.OnAirMissing -= SetFlameOff;
        AirProperty.OnAirAppearing -= SetFlameOn;
    }

    private void SetFlameOn() {
        foreach(Transform go in transform.GetComponentInChildren<Transform>()) {
            go.gameObject.SetActive(true);
        }

    }
    private void SetFlameOff() {
        foreach(Transform go in transform.GetComponentInChildren<Transform>()) {
            go.gameObject.SetActive(false);
        }

    }


    public void UpdateTimeScale() {
        if (InSpace == false) return;
        material.SetFloat("_RoomTime", Room.TimeScale);
    }

    public override void OnRoomLeave()
    {
        base.OnRoomLeave();
        StopAllCoroutines();
    }


    private void Reset() {
        Word = "Fire";
        AlternativeWords = new string[] {"Flame"};
    }

}
