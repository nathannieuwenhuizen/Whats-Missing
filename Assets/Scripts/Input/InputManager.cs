using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public delegate void ClickAction();
    public static event ClickAction OnClickDown;
    public static event ClickAction OnClickUp;
    public static event ClickAction OnJump;
    public static event ClickAction OnCancel;
    public static event ClickAction OnUndo;
    public static event ClickAction OnReset;
    public static event ClickAction OnStartRunning;
    public static event ClickAction OnEndRunning;
    
    public delegate void AxisAction( Vector2 delta);

    public static event AxisAction OnMove;
    public static event AxisAction OnRotate;

    public static bool KEYBOARD_ENABLED_MIRROR = false;


    private void OnEnable() {
        SettingPanel.OnSave += UpdateSettings;
    }

    private void OnDisable() {
        SettingPanel.OnSave -= UpdateSettings;
    }

    private void Awake() {
        UpdateSettings(Settings.GetSettings());
    }

    private void UpdateSettings(Settings settings) {
        // #if UNITY_EDITOR
        // InputManager.KEYBOARD_ENABLED_MIRROR = true;
// #else
        InputManager.KEYBOARD_ENABLED_MIRROR = settings.controlSettings.Enable_Keyboard_Input;
        Debug.Log("update keyboard settings" + settings.controlSettings.Enable_Keyboard_Input);
// #endif

    }

    void Update()
    {
        //controller
        if (Input.GetButtonUp("Fire1 Controller"))  {
            // OnClickUp?.Invoke();
            if (MirrorButton.SELECTED_BUTTON != null) MirrorButton.SELECTED_BUTTON.gameObject.GetComponent<Button>().onClick.Invoke();
        }
        
        //mouse
        if (Input.GetButtonDown("Fire1")) {
            OnClickDown?.Invoke();
        }
        if (Input.GetButtonUp("Fire1"))  {
            OnClickUp?.Invoke();
        }



        
        //movement
        if (Input.GetButtonDown("Jump")) {
            OnJump?.Invoke();
        }
        if (Time.timeScale == 1) {
            OnMove?.Invoke(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
        }
        else {
            OnMove?.Invoke(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
        }
        //running
        if (Input.GetButtonDown("Run")) {
            OnStartRunning?.Invoke();
        }
        if (Input.GetButtonUp("Run")) {
            OnEndRunning?.Invoke();
        }

        //rotate
        OnRotate?.Invoke(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));

        //Cancel
        if (Input.GetButtonDown("Cancel")) {
            OnCancel?.Invoke();
        }
    
        //Undo
        if (Input.GetKeyDown(KeyCode.Z)) {
            OnUndo?.Invoke();
        }

        // //reset
        // if (Input.GetKeyDown(KeyCode.R) && (Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl))) {
        //     OnReset?.Invoke();
        // }
        if (Input.GetKeyDown(KeyCode.R)) {
            OnReset?.Invoke();
        }

    }
}
