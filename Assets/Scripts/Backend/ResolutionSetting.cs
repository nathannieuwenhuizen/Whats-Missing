using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static TMPro.TMP_Dropdown;
using System;

public class ResolutionSetting : MonoBehaviour
{
    public static Vector2 CHOSEN_RESOLUTION = Vector2.zero;
    public static int CHOSEN_RESOLUTION_INDEX = -1;

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

        if (CHOSEN_RESOLUTION != Vector2.zero) {
            SetResolution(CHOSEN_RESOLUTION);
        }
        if (CHOSEN_RESOLUTION_INDEX != -1) {
            resolutionDropDown.value = CHOSEN_RESOLUTION_INDEX;
        }
    }

    public void ResolutionSelected(Int32 val) {
        CHOSEN_RESOLUTION_INDEX = val;
        CHOSEN_RESOLUTION = resolutionObject.SupportedResolutions[val];
        SetResolution(resolutionObject.SupportedResolutions[val]);
    }

    public void SetResolution( Vector2 resolution) {
        Debug.Log("resolution is selected" + resolution);
        Screen.SetResolution( (int)resolution.x, (int)resolution.y, true);
    }

}
