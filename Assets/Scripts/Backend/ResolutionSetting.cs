using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static TMPro.TMP_Dropdown;
using System;

public class ResolutionSetting : MonoBehaviour
{
    [SerializeField]
    private ResolutionsObject resolutionObject;

    [SerializeField]
    private TMP_Dropdown qualityDropDown;

    private void Awake() {
        List < OptionData > list = new List < OptionData > ();
        foreach(Vector2 resolution in resolutionObject.SupportedResolutions) {
            list.Add(new OptionData(resolution.x + " x " + resolution.y));
        }
        qualityDropDown.AddOptions(list);
        qualityDropDown.onValueChanged.AddListener(QualitySelected);

        qualityDropDown.value = QualitySettings.GetQualityLevel();
    }

    public void QualitySelected(Int32 val) {
        Screen.SetResolution(
            (int)resolutionObject.SupportedResolutions[val].x,
            (int)resolutionObject.SupportedResolutions[val].y, 
            true
            );
    }

}
