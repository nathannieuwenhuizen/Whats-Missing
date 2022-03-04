using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;

public class InGameEventSystemSwitcher : MonoBehaviour
{

    private InputSystemUIInputModule UIInputModule;
    private CustomInputModule inGameInputModule;

    private bool inGame = true;
    protected void Awake() {
        UIInputModule = GetComponent<InputSystemUIInputModule>();
        inGameInputModule = GetComponent<CustomInputModule>();
    }

    private void OnPause() {
        UIInputModule.enabled = true;
        inGameInputModule.ShowMouse();
        inGameInputModule.enabled = false;
    }
    private void OnResume() {
        UIInputModule.enabled = false;
        inGameInputModule.enabled = true;
        inGameInputModule.HideMouse();
    }

    protected void OnEnable() {
        PauseScreen.OnPause += OnPause;
        PauseScreen.OnResume += OnResume;
    }
    protected void OnDisable() {
        PauseScreen.OnPause -= OnPause;
        PauseScreen.OnResume -= OnResume;        
    }
}
