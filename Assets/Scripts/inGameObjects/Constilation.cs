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

    private Vector3 rotationVector;

    private void Start() {
        rotationVector = new Vector3(0,0,rotationSpeed);
    }
    void Update()
    {
        pivot.Rotate(rotationVector * Time.deltaTime * Room.TimeScale);
    }

    private void Reset() {
        Word = "constilation";
        AlternativeWords = new string[]{"galaxy"};
    }

}
