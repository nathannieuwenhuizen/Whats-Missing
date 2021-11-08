using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class RoomDebugger : MonoBehaviour
{
    public delegate void DebuggerAction();
    public static event DebuggerAction OnShow;
    public static event DebuggerAction OnHide;
    public RoomTelevision television;

    private RoomDebuggerBox debuggerBox = new RoomDebuggerBox();
    [SerializeField]
    private Area area;

    [SerializeField]
    private bool visible = true;
    private bool held = false;



#if UNITY_EDITOR

    private void Start() {
        visible = false;
    }
    private void Update() {
        if (Input.GetKey(KeyCode.E) && (Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && held == false) {
            held = true;
            visible = !visible;
            if (visible) {
                OnShow?.Invoke();
            } else 
            {
                OnHide?.Invoke();
            }
        } 
        if (Input.GetKeyUp(KeyCode.E)) {
            held = false;
        }
    }
    private void OnGUI() {
        if (visible) debuggerBox.Draw(area.CurrentRoom, television);
    }
    private void OnDisable() {
        visible = true;
    }
#endif

}
