using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// Handles the day and night cylce. It rotates the directional light on the x-axis
///</summary>
public class DayNightCycle : MonoBehaviour
{

    public delegate void SunIntensityEvent(float precentage);
    public static SunIntensityEvent OnSunIntensityChange;

    [SerializeField]
    private Light directionalLight;
    private float startIntensity;

    [SerializeField]
    private Transform sunPivot;

    public bool EbableRotation { get; set; } = true;
    private float oldSunRotation;

    [SerializeField]
    private float dayDurationInMinutes = 10;

    private float sunXRotation;
    private float sunYRotation;
    private float sunZRotation;
    private float sunRotation = 0;
    public float SunRotation {
        get => sunRotation;
        set {
            sunRotation = value % 360;
            Quaternion temp = sunPivot.rotation;
            temp.eulerAngles =  new Vector3(value, sunYRotation, sunZRotation);
            sunPivot.rotation = temp;

            UpdateSkybox();
            UpdateDirectionalLight();
        }
    }

    private void Awake() {
        startIntensity = directionalLight.intensity;
        sunXRotation = sunPivot.localRotation.eulerAngles.x;
        sunYRotation = sunPivot.localRotation.eulerAngles.y;
        sunZRotation = sunPivot.localRotation.eulerAngles.z;
        SunRotation = sunXRotation;
        // Debug.Log("rotation = " + SunRotation);
    }

    private void SunIsMissing() {
        if (SunRotation != -90) oldSunRotation = SunRotation;
        EbableRotation = false;
        SunRotation = -90;
    }

    private void SunIsAppearing() {
        SunRotation = oldSunRotation;
        EbableRotation = true;
    }

    private void Update() {
        if (Room.TimeScale == 0 || Time.timeScale == 0 || !EbableRotation) return;

        SunRotation += ((Time.deltaTime * Mathf.Pow(Room.TimeScale, 2)) * 360f) * (1f / (dayDurationInMinutes * 60f));

    }

    private void UpdateDirectionalLight() {
        directionalLight.intensity = Mathf.Max(Mathf.Sin(sunRotation / 360 * Mathf.PI * 2) * startIntensity, 0.01f);
        OnSunIntensityChange?.Invoke(directionalLight.intensity / startIntensity);
    }

    private void UpdateSkybox() {

    }

    private void OnEnable() {
        Sun.OnSunShrinking += SunIsMissing;
        Sun.OnSunShrinkingRevert += SunIsAppearing;
    }

    private void OnDisable() {
        Sun.OnSunShrinking -= SunIsMissing;
        Sun.OnSunShrinkingRevert -= SunIsAppearing;
    }
}
