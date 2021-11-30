using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// Handles the day and night cylce. It rotates the directional light on the x-axis
///</summary>
public class DayNightCycle : MonoBehaviour
{

    [SerializeField]
    private Light directionalLight;
    private float startIntensity;

    [SerializeField]
    private Transform sunPivot;

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

    private void Update() {
        if (Room.TimeScale == 0 || Time.timeScale == 0) return;

        SunRotation += ((Time.deltaTime * Room.TimeScale) * 360f) * (1f / (dayDurationInMinutes * 60f));

    }

    private void UpdateDirectionalLight() {
        directionalLight.intensity = Mathf.Max(Mathf.Sin(sunRotation / 360 * Mathf.PI * 2) * startIntensity, 0.01f);
    }

    private void UpdateSkybox() {

    }
}
