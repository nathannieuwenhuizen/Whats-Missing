using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindMill : RoomObject
{

    [SerializeField]
    private Transform rotationPivot;

    private float speed = 15f;

    public WindMill() {
        shrinkScale = .2f;
    }
    private void Update() {
        if (IsMissing || !inSpace) return;
        rotationPivot.Rotate(new Vector3(speed,0,0) * Mathf.Pow(Room.TimeScale, 2) * Time.deltaTime);
    }
    private void Reset() {
        Word = "windmill";
        AlternativeWords = new string[] {"mill"};
    }

    private void FasterWind() {
        speed = 600f;
    }
    private void NormalWind() {
        speed = 15f;
    }
    

    private void OnEnable() {
        Wind.OnWindEnlarged += FasterWind;
        Wind.OnWindNormal += NormalWind;
    }

    private void OnDisable() {
        Wind.OnWindEnlarged -= FasterWind;
        Wind.OnWindNormal -= NormalWind;

    }

}
