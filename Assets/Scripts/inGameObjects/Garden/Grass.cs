using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LODInfo {
    public float distance;
    public float detail;
}
public class Grass : RoomObject
{
    public List<LODInfo> lods = new List<LODInfo>();


    [SerializeField]
    private Color darkBaseColor;
    [SerializeField]
    private Color darkTipColor;

    private Color startTipColor;
    private Color startBaseColor;

    private Material grassMaterial;
    private float startWindSpeed;

    // public float WindSpeed {
    //     get { return grassMaterial.GetFloat("_WindFrequency"); }
    //     set {  grassMaterial.SetFloat("_WindFrequency", value); }
    // }
    public float WindSpeed {
        get { return grassMaterial.GetFloat("_WindFrequency"); }
        set {  grassMaterial.SetFloat("_WindFrequency", value); }
    }
    public Color BaseColor {
        get { return grassMaterial.GetColor("_BaseColor"); }
        set {  grassMaterial.SetColor("_BaseColor", value); }
    }
    public Color TipColor {
        get { return grassMaterial.GetColor("_TipColor"); }
        set {  grassMaterial.SetColor("_TipColor", value); }
    }

    void Awake()
    {
        grassMaterial = GetComponent<MeshRenderer>().material;
        startTipColor = TipColor;
        startBaseColor = BaseColor;
        startWindSpeed = WindSpeed;

        lods.Add( new LODInfo() {distance = 10f, detail = 0.1f});
        lods.Add( new LODInfo() {distance = 100f, detail = 0.3f});
        lods.Add( new LODInfo() {distance = 200f, detail = 0.5f});
    }

    private void Reset() {
        Word = "grass";
        AlternativeWords = new string[] { "grasses", "gras", "life", "plant", "plants" };
    }

    private void OnEnable() {
        TimeProperty.onTimeMissing += UpdateWindSpeedBasedOnTime;
        TimeProperty.onTimeAppearing += UpdateWindSpeedBasedOnTime;
        DayNightCycle.OnSunIntensityChange += UpdateGrassColor;
    }

    private void OnDisable() {
        TimeProperty.onTimeMissing -= UpdateWindSpeedBasedOnTime;
        TimeProperty.onTimeAppearing -= UpdateWindSpeedBasedOnTime;
        DayNightCycle.OnSunIntensityChange -= UpdateGrassColor;

        TipColor = startTipColor;
        BaseColor = startBaseColor;
    }


    private void setLODOfGrass(LODInfo lodInfo) {
        
    }

    private void UpdateGrassColor(float precentage)
    {
        BaseColor = Color.Lerp(startBaseColor, darkBaseColor, Mathf.Pow(1 - precentage, 2));
        TipColor = Color.Lerp(startTipColor, darkTipColor,  Mathf.Pow(1 - precentage, 2));
    }

    private void UpdateWindSpeedBasedOnTime() {
        WindSpeed = startWindSpeed * Room.TimeScale;
    }


}
