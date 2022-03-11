using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static TMPro.TMP_Dropdown;

public class QualitySetting: MonoBehaviour {

    [SerializeField]
    private TMP_Dropdown qualityDropDown;

    private void Awake() {
        List < OptionData > list = new List < OptionData > ();
        foreach(string name in QualitySettings.names) {
            list.Add(new OptionData(name));
        }
        qualityDropDown.AddOptions(list);
        qualityDropDown.onValueChanged.AddListener(QualitySelected);

        if(QUALITY_INDEX != -1) QualitySettings.SetQualityLevel(QUALITY_INDEX, true);
        qualityDropDown.value = QualitySettings.GetQualityLevel();

        Debug.Log("chosen quality" + QUALITY_INDEX);


    }
    public static int QUALITY_INDEX 
    {
        get {
            int val = PlayerPrefs.GetInt("Quality", -1);
            return val;
        }
        set {
            PlayerPrefs.SetInt("Quality", value);
        }
    }


    public void QualitySelected(Int32 val) {
        QUALITY_INDEX = val;
        QualitySettings.SetQualityLevel(val, true);
    }
}
