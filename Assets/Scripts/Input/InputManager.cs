using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    private GamePlayControls controls;
    private float movementGravity = 10f;
    private float movementDeadzone = .01f;
    private Vector2 movementVector = Vector2.zero;
    private Vector2 MovementVector {
        get => movementVector;
        set {
            //setting deadzones for keyboard buttons
            if (value.x < movementDeadzone || value.x > 1f - movementDeadzone) {
                if (movementVector.x < movementDeadzone) movementVector.x = 0;
                if (movementVector.x > 1f - movementDeadzone) movementVector.x = 1f;
            }
            if (value.y < movementDeadzone || value.y > 1f - movementDeadzone) {
                if (movementVector.y < movementDeadzone) movementVector.y = 0;
                if (movementVector.y > 1f - movementDeadzone) movementVector.y = 1f;
            }
            movementVector = value;
        }
    }
    private Vector2 cameraVector = Vector2.zero;

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
        ControllerRebinds.OnRebindChanged +=  UpdateControllerRebind;
    }   

    private void UpdateControllerRebind(GamePlayControls _controls) {
        if (controls != null) {
            controls.Player.Jump.started -= Jump;
            controls.Player.Run.started -= RunStart;
            controls.Player.Run.canceled -= RunEnd;
            controls.Player.Click.started -= ClickStart;
            controls.Player.Click.canceled -= ClickEnd;
            controls.Player.Cancel.started -= Cancel;


        }
        controls = _controls;
        controls.Player.Jump.started += Jump;
        controls.Player.Run.started += RunStart;
        controls.Player.Run.canceled += RunEnd;
        controls.Player.Click.started += ClickStart;
        controls.Player.Click.canceled += ClickEnd;
        controls.Player.Cancel.started += Cancel;

    } 

    private void OnDisable() {
        SettingPanel.OnSave -= UpdateSettings;
        ControllerRebinds.OnRebindChanged -=  UpdateControllerRebind;
    }

    private void Awake() {
        UpdateSettings(Settings.GetSettings());
    }

    private void UpdateSettings(Settings settings) {
        // #if UNITY_EDITOR
        // InputManager.KEYBOARD_ENABLED_MIRROR = true;
// #else
        InputManager.KEYBOARD_ENABLED_MIRROR = settings.controlSettings.Enable_Keyboard_Input;
// #endif

    }

    void Update()
    {
        //controller
        
        //mouse
        // if (Input.GetButtonDown("Fire1")) {
        //     OnClickDown?.Invoke();
        // }
        // if (Input.GetButtonUp("Fire1"))  {
        //     OnClickUp?.Invoke();
        // }

        
        //movement
        // if (Input.GetButtonDown("Jump")) {
        //     OnJump?.Invoke();
        // }
        // if (Time.timeScale == 1) {
        //     OnMove?.Invoke(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
        // }
        // else {
        //     OnMove?.Invoke(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
        // }
        // //running
        // if (Input.GetButtonDown("Run")) {
        //     OnStartRunning?.Invoke();
        // }
        // if (Input.GetButtonUp("Run")) {
        //     OnEndRunning?.Invoke();
        // }

        //rotate
        // OnRotate?.Invoke(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));

        //Cancel
        // if (Input.GetButtonDown("Cancel")) {
        //     OnCancel?.Invoke();
        // }
    
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

        MovementVector = Vector2.Lerp(movementVector, controls.Player.Movement.ReadValue<Vector2>(), Time.deltaTime * movementGravity);
        OnMove?.Invoke(MovementVector);
        cameraVector = controls.Player.Camera.ReadValue<Vector2>();
        OnRotate?.Invoke(cameraVector);

    }



    public void Jump(InputAction.CallbackContext context) {
        OnJump?.Invoke();
    }
    public void RunStart(InputAction.CallbackContext context) {
        OnStartRunning?.Invoke();
    }
    public void RunEnd(InputAction.CallbackContext context) {
        OnEndRunning?.Invoke();
    }
    public void ClickStart(InputAction.CallbackContext context) {
        OnClickDown?.Invoke();
        if (MirrorButton.SELECTED_BUTTON != null && ControllerCheck.IsInputFromGamePad(context)) {
            ExecuteEvents.Execute (MirrorButton.SELECTED_BUTTON.gameObject, new PointerEventData (EventSystem.current), ExecuteEvents.pointerDownHandler);
        }
    }
    public void ClickEnd(InputAction.CallbackContext context) {
        OnClickUp?.Invoke();
        if (MirrorButton.SELECTED_BUTTON != null && ControllerCheck.IsInputFromGamePad(context)) {
            ExecuteEvents.Execute (MirrorButton.SELECTED_BUTTON.gameObject, new PointerEventData (EventSystem.current), ExecuteEvents.pointerClickHandler);
            ExecuteEvents.Execute (MirrorButton.SELECTED_BUTTON.gameObject, new PointerEventData (EventSystem.current), ExecuteEvents.pointerUpHandler);
        }

    }
    public void Cancel(InputAction.CallbackContext context) {
        OnCancel?.Invoke();
    }


}
