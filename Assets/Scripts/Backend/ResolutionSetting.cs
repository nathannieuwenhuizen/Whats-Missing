using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static TMPro.TMP_Dropdown;
using System;

public class ResolutionSetting : MonoBehaviour
{
    public static Vector2 CHOSEN_RESOLUTION = Vector2.zero;

    [SerializeField]
    private ResolutionsObject resolutionObject;

    [SerializeField]
    private TMP_Dropdown resolutionDropDown;

    private void Awake() {

        List < OptionData > list = new List < OptionData > ();
        foreach(Vector2 resolution in resolutionObject.SupportedResolutions) {
            list.Add(new OptionData(resolution.x + " x " + resolution.y));
        }
        resolutionDropDown.AddOptions(list);
        resolutionDropDown.onValueChanged.AddListener(ResolutionSelected);
        CHOSEN_RESOLUTION_INDEX = GetResolutionIndex();
    }
    public static int CHOSEN_RESOLUTION_INDEX 
    {
        get {
            int val = PlayerPrefs.GetInt("RESOLUTION", -1);
            return val;
        }
        set {
            PlayerPrefs.SetInt("RESOLUTION", value);
        }
    }

    private void Start() {
        if (CHOSEN_RESOLUTION_INDEX != -1) {
            resolutionDropDown.value = CHOSEN_RESOLUTION_INDEX;
            SetResolution(resolutionObject.SupportedResolutions[CHOSEN_RESOLUTION_INDEX]);
        }
    }

    public void ResolutionSelected(Int32 val) {

        CHOSEN_RESOLUTION_INDEX = val;
        CHOSEN_RESOLUTION = resolutionObject.SupportedResolutions[val];
        SetResolution(resolutionObject.SupportedResolutions[val]);
    }

    public int GetResolutionIndex() {
        int result = 0;
        float closestDelta = Mathf.Infinity;
        int i = 0;
        foreach(Vector2 v in resolutionObject.SupportedResolutions) {
            if (Mathf.Abs(v.x - Screen.width) < closestDelta) {
                result = i;
                closestDelta = Mathf.Abs(v.x - Screen.width);
            }
            i++;
        }
        return result;
    }

    public void SetResolution( Vector2 resolution) {
        Screen.SetResolution( (int)resolution.x, (int)resolution.y,Screen.fullScreen);
    }

}
