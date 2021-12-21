using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// Handles the day and night cylce. It rotates the directional light on the x-axis
///</summary>
public class DayNightCycle : MonoBehaviour
{
    [SerializeField]
    private AmbientColors nightColors = new AmbientColors() {
        groundColor = Color.black, 
        equatorColor = Color.black, 
        skyColor = Color.black,
        sunColor = Color.black};

    private AmbientColors dayColors;
    private AmbientColors sceneColors;
    public AmbientColors SceneColors {
        get => sceneColors;
        set {
            sceneColors = value;
            UpdateRenderSettings();
        }
    }
    private void UpdateRenderSettings() {
        RenderSettings.ambientGroundColor = SceneColors.groundColor;
        RenderSettings.ambientEquatorColor = SceneColors.equatorColor;
        RenderSettings.ambientSkyColor = SceneColors.skyColor;
        RenderSettings.sun.color = SceneColors.sunColor;

    }


    

    public delegate void SunIntensityEvent(float precentage);
    public static SunIntensityEvent OnSunIntensityChange;

    [SerializeField]
    private Light directionalLight;
    private float startIntensity;

    [SerializeField]
    private Transform sunPivot;

    public bool EbableRotation { get; set; } = true;
    private float oldSunRotation;

    private float dayDurationInMinutes = 30;

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
        dayColors = new AmbientColors() {
            groundColor = RenderSettings.ambientGroundColor,
            equatorColor = RenderSettings.ambientEquatorColor,
            skyColor = RenderSettings.ambientSkyColor,
            sunColor = RenderSettings.sun.color
        };

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

    private void SetRotationToMidday() {
        sunRotation = 60;
    }

    private void Update() {
        if (Room.TimeScale == 0 || Time.timeScale == 0 || !EbableRotation) return;

        SunRotation += ((Time.deltaTime * Mathf.Pow(Room.TimeScale, 2.3f)) * 360f) * (1f / (dayDurationInMinutes * 60f));

    }

    private void UpdateDirectionalLight() {
        directionalLight.intensity = Mathf.Max(Mathf.Sin(sunRotation / 360 * Mathf.PI * 2) * startIntensity, 0.01f);
        float precentage = directionalLight.intensity / startIntensity;
        OnSunIntensityChange?.Invoke(precentage);
        

        SceneColors = new AmbientColors() {
            sunColor = dayColors.sunColor,
            skyColor = Color.LerpUnclamped(dayColors.skyColor, nightColors.skyColor, 1 - precentage),
            equatorColor = Color.LerpUnclamped(dayColors.equatorColor, nightColors.equatorColor, 1 - precentage),
            groundColor = Color.LerpUnclamped(dayColors.groundColor, nightColors.groundColor, 1 - precentage)
        };
    
    }

    private void UpdateSkybox() {

    }

    private void OnEnable() {
        Sun.OnSunShrinking += SunIsMissing;
        Sun.OnSunShrinkingRevert += SunIsAppearing;
        ColorProperty.OnColorEnlarged += SetRotationToMidday;
        TeddyBear.OnTeddyBearEnlarged += SetRotationToMidday;
        Room.OnRoomEntering += SetRotationToMidday;

    }

    private void OnDisable() {
        Sun.OnSunShrinking -= SunIsMissing;
        Sun.OnSunShrinkingRevert -= SunIsAppearing;
        ColorProperty.OnColorEnlarged -= SetRotationToMidday;
        TeddyBear.OnTeddyBearEnlarged -= SetRotationToMidday;
        Room.OnRoomEntering -= SetRotationToMidday;


    }
}
