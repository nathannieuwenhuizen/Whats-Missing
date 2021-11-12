using UnityEngine;
using UnityEngine.EventSystems;

public class CustomInputModule : StandaloneInputModule
{
    // Current cursor lock state (memory cache)
    [SerializeField]
    private CursorLockMode _currentLockState = CursorLockMode.None;
    [SerializeField]
    private bool mouseIsHidden = true;

    /// <summary>
    /// Process the current tick for the module.
    /// </summary>
    public override void Process()
    {
        if (mouseIsHidden) {
            if (ControllerCheck.AnyControllerConnected()) {
                submitButton = "Empty";
            } else {
                submitButton = "Submit";
            }
            _currentLockState = Cursor.lockState;
            Cursor.lockState = CursorLockMode.Confined;
            base.Process();
            Cursor.lockState = _currentLockState;
        } else {
            submitButton = "Submit";
            base.Process();
        }
    }

    protected override void OnEnable() {
        
        base.OnEnable();
        PauseScreen.OnPause += ShowMouse;
        PauseScreen.OnResume += HideMouse;
        RoomDebugger.OnShow += ShowMouse;
        RoomDebugger.OnHide += HideMouse;
    }
    protected override void OnDisable() {
        base.OnDisable();
        PauseScreen.OnPause -= ShowMouse;
        PauseScreen.OnResume -= HideMouse;        
        RoomDebugger.OnShow -= ShowMouse;
        RoomDebugger.OnHide -= HideMouse;

    }
    private void ShowMouse() {
        if (ControllerCheck.AnyControllerConnected() == false) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        mouseIsHidden = false;
    }
    private void HideMouse() {
        Cursor.lockState = CursorLockMode.Locked;
        mouseIsHidden = true;
        Cursor.visible = false;
    }
}
