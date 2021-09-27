using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

///<summary>
/// Handles the front end part of the settings panel. It does calls the settings to be saved in the settings struct
///</summary>
public class SettingPanel : MonoBehaviour
{
    private Settings settings;

    [SerializeField]
    private Slider cameraSensitivitySlider;
    [SerializeField]
    private Toggle cameraXInvert;
    [SerializeField]
    private Toggle cameraYInvert;
    [SerializeField]
    private Toggle depthOfField;
    [SerializeField]
    private Toggle motionBlur;

    private void Awake() {
        settings = Settings.GetSettings();
        UpdateUI();
    }
    
    void Start()
    {
        //sets up the event listeners.
        cameraSensitivitySlider.onValueChanged.AddListener(delegate { 
            Debug.Log("sensetivity changed");
            settings.controlSettings.Camera_sensetivity = cameraSensitivitySlider.value;
            });
        cameraXInvert.onValueChanged.AddListener(delegate{ settings.controlSettings.Camera_x_invert = cameraXInvert.isOn;});
        cameraYInvert.onValueChanged.AddListener(delegate{ settings.controlSettings.Camera_y_invert = cameraYInvert.isOn;});
        depthOfField.onValueChanged.AddListener(delegate{ settings.cameraSettings.Depth_of_field_enabled = depthOfField.isOn;});
        motionBlur.onValueChanged.AddListener(delegate{ settings.cameraSettings.Motion_blur_enabled = motionBlur.isOn;});
    }

    ///<summary>
    /// Updates the UI with the data.
    ///</summary>
    private void UpdateUI() {
        cameraSensitivitySlider.value = settings.controlSettings.Camera_sensetivity;
        cameraXInvert.isOn = settings.controlSettings.Camera_x_invert;
        cameraYInvert.isOn = settings.controlSettings.Camera_y_invert;
        depthOfField.isOn = settings.cameraSettings.Depth_of_field_enabled;
        motionBlur.isOn = settings.cameraSettings.Motion_blur_enabled;
    }
    
    public void Save() {
        settings.Save();
    }

}
