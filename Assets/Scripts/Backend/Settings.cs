using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// Struct for holding the user settings in plater prefs.
///</summary>
public struct Settings
{
    ///<summary>
    /// Returns a setting class while it automaticly loads the user data.
    ///</summary>
    public static Settings GetSettings() {
        Settings settings = new Settings();
        settings.Load();
        return settings;
    }

    public ControlSettings controlSettings;
    public CameraSettings cameraSettings;
    ///<summary>
    /// Loads the settings into the game.
    ///</summary>
    public void Load() {
        controlSettings = new ControlSettings(){
            Camera_sensetivity = PlayerPrefs.GetFloat("Camera_sensetivity", 1f),
            Camera_x_invert = PlayerPrefs.GetInt("Camera_x_invert", 0) == 1,
            Camera_y_invert = PlayerPrefs.GetInt("Camera_y_invert", 0) == 1
        };
        cameraSettings = new CameraSettings() {
            Depth_of_field_enabled = PlayerPrefs.GetInt("Depth_of_field_enabled", 1) == 1,
            Motion_blur_enabled = PlayerPrefs.GetInt("Motion_blur_enabled", 1) == 1
        };
    }
    ///<summary>
    /// Saves the settings into the playerprefs
    ///</summary>
    public void Save() {
        Debug.Log("save" + controlSettings.Camera_sensetivity);
        PlayerPrefs.SetFloat("Camera_sensetivity", controlSettings.Camera_sensetivity);
        PlayerPrefs.SetInt("Camera_x_invert", controlSettings.Camera_x_invert ? 1 : 0);
        PlayerPrefs.SetInt("Camera_y_invert", controlSettings.Camera_y_invert ? 1 : 0);

        PlayerPrefs.SetInt("Depth_of_field_enabled", cameraSettings.Depth_of_field_enabled ? 1 : 0);
        PlayerPrefs.SetInt("Motion_blur_enabled", cameraSettings.Motion_blur_enabled ? 1 : 0);
    }
}

///<summary>
/// Holds the data on how the camera should work based on the user settings.
///</summary>
public struct ControlSettings {
    public float Camera_sensetivity;
    public bool Camera_y_invert;
    public bool Camera_x_invert;
}

///<summary>
/// The post processing user serttings.
///</summary>
public struct CameraSettings {
    public bool Depth_of_field_enabled;
    public bool Motion_blur_enabled;
}