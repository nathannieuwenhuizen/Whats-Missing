using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindMill : RoomObject
{

    [SerializeField]
    private Transform rotationPivot;

    private float speed = 5.5f;
    private void Update() {
        if (IsMissing || !inSpace) return;
        rotationPivot.Rotate(new Vector3(speed,0,0) * Room.TimeScale * Time.deltaTime);
    }
    private void Reset() {
        Word = "windmill";
        AlternativeWords = new string[] {"mill"};
    }

}
