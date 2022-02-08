using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Constilation : RoomObject
{
    [SerializeField]
    private Transform pivot;
    [Range(0,10f)]
    [SerializeField]
    private float rotationSpeed;

    [SerializeField]
    private Transform mainPivot;
    [Range(0,10f)]
    [SerializeField]
    private float mainRotationSpeed;

    private Quaternion startRotation;

    private Vector3 rotationVector;
    private Vector3 mainRotationVector;

    private Quaternion mianstartRotation;

    private void Awake() {
        mianstartRotation = mainPivot.rotation;
        startRotation = pivot.rotation;
    }

    private void Start() {
        rotationVector = new Vector3(0,0,rotationSpeed);
        mainRotationVector = new Vector3(0,0, mainRotationSpeed);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) {
            mainPivot.rotation = mianstartRotation;
            pivot.rotation = startRotation;
        }

        pivot.Rotate(rotationVector * Time.deltaTime * Room.TimeScale);
        mainPivot.Rotate(mainRotationVector * Time.deltaTime * Room.TimeScale);
    }

    public override void OnRoomEnter()
    {
        base.OnRoomEnter();
        pivot.rotation = startRotation;
    }

    private void Reset() {
        Word = "constilation";
        AlternativeWords = new string[]{"galaxy"};
    }

}
