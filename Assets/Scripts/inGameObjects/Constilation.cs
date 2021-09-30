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

    private Vector3 rotationVector;
    private Vector3 mainRotationVector;

    private void Start() {
        rotationVector = new Vector3(0,0,rotationSpeed);
        mainRotationVector = new Vector3(0,0, mainRotationSpeed);
    }
    void Update()
    {
        pivot.Rotate(rotationVector * Time.deltaTime * Room.TimeScale);
        mainPivot.Rotate(mainRotationVector * Time.deltaTime * Room.TimeScale);
    }

    private void Reset() {
        Word = "constilation";
        AlternativeWords = new string[]{"galaxy"};
    }

}
