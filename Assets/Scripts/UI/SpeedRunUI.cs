using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeedRunUI : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private CanvasGroup cg;

    private void Start() {
        OnSettingsUpdate(Settings.GetSettings());
    }

    private void OnEnable() {
        SpeedrunTimer.OnTimerUpdate += UpdateText;
        SettingPanel.OnSave += OnSettingsUpdate;
    }

    private void OnDisable() {
        SpeedrunTimer.OnTimerUpdate -= UpdateText;
        SettingPanel.OnSave -= OnSettingsUpdate;

    }

    private void OnSettingsUpdate(Settings settings) {
        cg.alpha = settings.cameraSettings.Timer_Enabled ? 1 : 0;
    }

    private void UpdateText(float Timer) {
        Timer /= 1000f;
        int hours = Mathf.FloorToInt(Timer / 60F / 60f);
        int minutes = Mathf.FloorToInt(Timer / 60F);
        int seconds = Mathf.FloorToInt(Timer % 60F);
        int milliseconds = Mathf.FloorToInt((Timer * 100F) % 100F);
        string result = "";
        // if (hours > 0) result += hours.ToString ("00") + ":";
        result += minutes.ToString ("00") + ":" + seconds.ToString ("00") + ":" + milliseconds.ToString("00");

        text.text = result;
    }


}
