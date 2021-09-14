using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static TMPro.TMP_Dropdown;

public class QualitySetting: MonoBehaviour {

    [SerializeField]
    private TMP_Dropdown qualityDropDown;

    private void Start() {
        List < OptionData > list = new List < OptionData > ();
        foreach(string name in QualitySettings.names) {
            list.Add(new OptionData(name));
        }
        qualityDropDown.AddOptions(list);
        qualityDropDown.onValueChanged.AddListener(QualitySelected);

        qualityDropDown.value = QualitySettings.GetQualityLevel();
    }

    public void QualitySelected(Int32 val) {
        QualitySettings.SetQualityLevel(val, true);
    }
}
