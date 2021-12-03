using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Water : RoomObject
{

    private Material watermaterial;

    private void Awake() {
        watermaterial = GetComponent<MeshRenderer>().material;
    }


    private void OnEnable() {
        TimeProperty.onTimeMissing += UpdateTimeScale;
        TimeProperty.onTimeAppearing += UpdateTimeScale;
    }

    private void OnDisable() {
        TimeProperty.onTimeMissing -= UpdateTimeScale;
        TimeProperty.onTimeAppearing -= UpdateTimeScale;
    }


    public void UpdateTimeScale() {
        // if (InSpace == false) return;
        Debug.Log("set water time");
        watermaterial.SetFloat("_RoomTime", Room.TimeScale);
    }

    private void Reset() {
        Word = "water";
        AlternativeWords = new string[] { "waters", "lake", "pond", "pool", "liquid" };
    }

}
