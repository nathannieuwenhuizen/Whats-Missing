using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : RoomObject
{
    private float cloudSpeed = .5f;

    [SerializeField]
    private MeshRenderer meshRenderer;

    private Material cloudMaterial;

    [SerializeField]
    private Color darkColor;
    public Cloud() {
        normalScale = 35f;
        shrinkScale = 0.7f;
    }

    private void Reset() {
        Word = "cloud";
        AlternativeWords = new string[] { "clouds", "smoke", "mist" };
    }

    private void Awake() {
        cloudMaterial = meshRenderer.material;
    }

    private void Update() {
        transform.Rotate(new Vector3(0, 0, cloudSpeed * Room.TimeScale * Time.deltaTime));
    }


    private void UpdateCloudColor(float sunIntensity) {
        meshRenderer.material.color =  Color.Lerp(Color.white, darkColor, 1 - Mathf.Pow(sunIntensity, 2));
    }

    private void OnEnable() {
        DayNightCycle.OnSunIntensityChange += UpdateCloudColor;
    }

    private void OnDisable() {
        DayNightCycle.OnSunIntensityChange -= UpdateCloudColor;
    }


}
