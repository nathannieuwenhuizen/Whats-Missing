using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : RoomObject
{
    [SerializeField]
    private Color darkColor;

    private Color startTipColor;
    private Color startBaseColor;

    private Material grassMaterial;
    private float startWindSpeed;

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

    private void UpdateGrassColor(float precentage)
    {
        BaseColor = Color.Lerp(startBaseColor, darkColor, 1 - precentage);
        TipColor = Color.Lerp(startTipColor, darkColor, 1 - precentage);
    }

    private void UpdateWindSpeedBasedOnTime() {
        Debug.Log("time scale = " + Room.TimeScale);
        WindSpeed = startWindSpeed * Room.TimeScale;
    }


}
