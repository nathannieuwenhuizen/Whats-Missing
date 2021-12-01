using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : RoomObject
{

    private Material grassMaterial;
    private float startWindSpeed;

    public float WindSpeed {
        get { return grassMaterial.GetFloat("_WindFrequency"); }
        set {  grassMaterial.SetFloat("_WindFrequency", value); }
    }

    void Awake()
    {
        grassMaterial = GetComponent<MeshRenderer>().material;
        startWindSpeed = WindSpeed;
    }

    private void Reset() {
        Word = "grass";
        AlternativeWords = new string[] { "grasses", "gras", "life", "plant", "plants" };
    }

    private void OnEnable() {
        TimeProperty.onTimeMissing += UpdateWindSpeedBasedOnTime;
        TimeProperty.onTimeAppearing += UpdateWindSpeedBasedOnTime;
    }

    private void OnDisable() {
        
    }

    private void UpdateWindSpeedBasedOnTime() {
        Debug.Log("time scale = " + Room.TimeScale);
        WindSpeed = startWindSpeed * Room.TimeScale;
    }


}
