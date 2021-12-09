using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

///<summary>
/// Handles the front end part of the settings panel. It does calls the settings to be saved in the settings struct
///</summary>
public class SettingPanel : AnimatedPopup
{
    private Settings settings;
    public delegate void SettingsAction(Settings settings);
    public static event SettingsAction OnSave;


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
    [SerializeField]
    private Toggle enableKeyboardInput;

    private void Awake() {
        settings = Settings.GetSettings();
        UpdateUI();
    }
    
    void Start()
    {
        //sets up the event listeners.
        cameraSensitivitySlider.onValueChanged.AddListener(delegate { 
            settings.controlSettings.Camera_sensetivity = cameraSensitivitySlider.value;
            });
        cameraXInvert.onValueChanged.AddListener(delegate{ settings.controlSettings.Camera_x_invert = cameraXInvert.isOn;});
        cameraYInvert.onValueChanged.AddListener(delegate{ settings.controlSettings.Camera_y_invert = cameraYInvert.isOn;});
        depthOfField.onValueChanged.AddListener(delegate{ settings.cameraSettings.Depth_of_field_enabled = depthOfField.isOn;});
        motionBlur.onValueChanged.AddListener(delegate{ settings.cameraSettings.Motion_blur_enabled = motionBlur.isOn;});
        enableKeyboardInput.onValueChanged.AddListener(delegate{ settings.controlSettings.Enable_Keyboard_Input = enableKeyboardInput.isOn;});
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
        enableKeyboardInput.isOn = settings.controlSettings.Enable_Keyboard_Input;
    }
    
    public void Save() {
        settings.Save();
        OnSave?.Invoke(settings);
    }

    public void Open() {
        ShowAnimation(true);
        ControllerCheck.SelectUIGameObject(cameraSensitivitySlider.gameObject);
    }
    public void Close() {
        Save();
        ShowAnimation(false);
    }

}
