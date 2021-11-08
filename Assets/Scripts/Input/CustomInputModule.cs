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
            _currentLockState = Cursor.lockState;

            //TODO: in edit mode lockmode is contained
            Cursor.lockState = CursorLockMode.Confined;

            base.Process();

            Cursor.lockState = _currentLockState;
        } else {
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
        Cursor.lockState = CursorLockMode.None;
        mouseIsHidden = false;
        Cursor.visible = true;
    }
    private void HideMouse() {
        Cursor.lockState = CursorLockMode.Locked;
        mouseIsHidden = true;
        Cursor.visible = false;
    }
}
