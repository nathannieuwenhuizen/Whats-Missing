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
    public delegate void SettingsButtonAction();
    public static SettingsButtonAction OnSettingsClose;

    [SerializeField]
    private TMPro.TMP_Text settingsText;

    [Header("header buttons")]
    [SerializeField]
    private Button generalButton;
    [SerializeField]
    private GameObject generalList;
    [SerializeField]
    private Button gameplayButton;
    [SerializeField]
    private GameObject gameplayList;
    [SerializeField]
    private Button backButton;



    [Header("setting elements")]
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

    [Header("Controls")]

    [SerializeField]
    private Button controlButton;
    [SerializeField]
    private ControllerRebinds controlList;

    private void Awake() {
        settings = Settings.GetSettings();
        UpdateUI();
        backButton.onClick.AddListener(CloseSettings);
    }

    public void ToggleGeneral() {

        generalList.SetActive(true);
        gameplayList.SetActive(false);
        controlList.gameObject.SetActive(true);
        controlList.Hide();
        // generalButton.interactable = false;
        // gameplayButton.interactable = true;
        // controlButton.interactable = true;
        settingsText.text = "General";
        Debug.Log("should be general by now");
        ControllerCheck.SelectUIGameObject(generalButton.gameObject);
    }

    public void ToggleGameplay() {

        generalList.SetActive(false);
        gameplayList.SetActive(true);
        // generalButton.interactable = true;
        // gameplayButton.interactable = false;
        controlList.Hide();
        // controlButton.interactable = true;
        settingsText.text = "Gameplay";
        ControllerCheck.SelectUIGameObject(gameplayButton.gameObject);
    }
    public void ToggleControls() {

        generalList.SetActive(false);
        gameplayList.SetActive(false);
        // generalButton.interactable = true;
        // gameplayButton.interactable = true;
        controlList.Show();
        // controlButton.interactable = false;
        settingsText.text = "Controls";
        ControllerCheck.SelectUIGameObject(controlButton.gameObject);
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
        ToggleGeneral();
    }
    public void Close() {
        Save();
        ShowAnimation(false);
    }

    public void CloseSettings() {
        OnSettingsClose?.Invoke();
    }

    private void OnEnable() {
        PauseScreen.OnSettingsOpen += Open;
        PauseScreen.OnSettingsClose += Close;
        Menu.OnSettingsOpen += Open;
        Menu.OnSettingsClose += Close;
    }

    private void OnDisable() {
        PauseScreen.OnSettingsOpen -= Open;
        PauseScreen.OnSettingsClose -= Close;
        Menu.OnSettingsOpen -= Open;
        Menu.OnSettingsClose -= Close;

    }

}
