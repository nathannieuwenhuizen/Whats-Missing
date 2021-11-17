using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

    public class AudioSetting: MonoBehaviour {

    private float steps = 10;

    [SerializeField]
    private Slider SFXSlider;

    [SerializeField]
    private Slider musicSlider;

    public static bool MUTE 
    {
        get {
            int val = PlayerPrefs.GetInt("Mute", 1);
            return val == 1;
        }
        set {
            PlayerPrefs.SetInt("Mute", value ? 1 : 0);
        }
    }
    public static float SFX 
    {
        get {
            float val = PlayerPrefs.GetFloat("SFX", 1);
            return val;
        }
        set {
            PlayerPrefs.SetFloat("SFX", Mathf.Clamp(value, 0, 1));
        }
    }
    public static float MUSIC 
    {
        get {
            float val = PlayerPrefs.GetFloat("Music", 1);
            return val;
        }
        set {
            PlayerPrefs.SetFloat("Music", Mathf.Clamp(value, 0, 1));
        }
    }
    private void Awake() {
        // gameObject.SetActive(false);
        SFXSlider.minValue = 0;
        SFXSlider.maxValue = steps;
        // SFXSlider.wholeNumbers = true;
        SFXSlider.onValueChanged.AddListener(SetSFX);
        musicSlider.minValue = 0;
        musicSlider.maxValue = steps;
        // musicSlider.wholeNumbers = true;

        musicSlider.onValueChanged.AddListener(SetMusic);
    }
    private void OnEnable() 
    {
        // SetMute(MUTE);
        SFXSlider.value = AudioSetting.SFX * steps;
        musicSlider.value = AudioSetting.MUSIC * steps;
    }
    // public void SetMute(bool val) {
    //     MUTE = val;
    //     if (AudioHandler.Instance) AudioHandler.Instance.Mute = val;
    //     UpdateImage();
    // }

    public void SetSFX(float value) 
    {
        AudioSetting.SFX = value / steps;
        if (AudioHandler.Instance) {
            // AudioHandler.Instance.PlaySound(SFXFiles.plop, .5f);
        }

    }
    public void SetMusic(float value) 
    {
        AudioSetting.MUSIC = value / steps;
        if (AudioHandler.Instance)
            AudioHandler.Instance.MusicVolume = AudioHandler.Instance.MusicVolume;
    }

    // public void ToggleMute() {
    //     SetMute(!MUTE);
    // }

    // private void UpdateImage() {
    //     image.sprite = MUTE ? muteSprite : unmuteSprite;
    // }
}

