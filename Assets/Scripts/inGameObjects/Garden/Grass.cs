using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LODInfo {
    public float distance;
    public float detail;
}
public class Grass : RoomObject
{
    public List<LODInfo> lods = new List<LODInfo>();

    [SerializeField]
    private Room room;

    [SerializeField]
    private Color darkBaseColor;
    [SerializeField]
    private Color darkTipColor;

    private Color startTipColor;
    private Color startBaseColor;

    private Material grassMaterial;
    private float startWindSpeed;

    private MeshRenderer meshRenderer;

    public float TessellationGrassDistance {
        get { return grassMaterial.GetFloat("_TessellationGrassDistance"); }
        set {  grassMaterial.SetFloat("_TessellationGrassDistance", value); }
    }
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
        meshRenderer = GetComponent<MeshRenderer>();
        grassMaterial = meshRenderer.material;
        startTipColor = TipColor;
        startBaseColor = BaseColor;
        startWindSpeed = WindSpeed;

        InitializeLODs();

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

    private void Update() {
        CheckLODDistance();
    }

    public void CheckLODDistance() {
        if (room == null || room.Player == null) return;

        for(int i = 0; i <  lods.Count; i++) {
            if (Vector3.Distance(meshRenderer.bounds.center, room.Player.transform.position ) < lods[i].distance) {
                setLODOfGrass(lods[i]);
                return;
            }
        }
    }

    private void setLODOfGrass(LODInfo lodInfo) {
        // Debug.Log("set lod update! " + lodInfo.detail);;
        TessellationGrassDistance = lodInfo.detail;
    }

    private void UpdateGrassColor(float precentage)
    {
        BaseColor = Color.Lerp(startBaseColor, darkBaseColor, Mathf.Pow(1 - precentage, 2));
        TipColor = Color.Lerp(startTipColor, darkTipColor,  Mathf.Pow(1 - precentage, 2));
    }

    private void UpdateWindSpeedBasedOnTime() {
        WindSpeed = startWindSpeed * Room.TimeScale;
    }

    private void  InitializeLODs() {
        if (lods.Count != 0) lods = new List<LODInfo>();
        lods.Add( new LODInfo() {distance = 50f, detail = 0.1f});
        lods.Add( new LODInfo() {distance = 60f, detail = 0.13f});
        lods.Add( new LODInfo() {distance = 70f, detail = 0.15f});
        lods.Add( new LODInfo() {distance = 80f, detail = 0.3f});
        lods.Add( new LODInfo() {distance = 200f, detail = 0.5f});
    }

    private void OnDrawGizmosSelected() {
        meshRenderer = GetComponent<MeshRenderer>();
        InitializeLODs();

        for (int i = 0; i < lods.Count; i++) {
            // Gizmos.DrawWireSphere(meshRenderer.bounds.center, lods[i].distance);
        }
    }


}
