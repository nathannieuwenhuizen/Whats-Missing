using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class RoomDebugger : MonoBehaviour
{
    private RoomDebuggerBox roomDebugger = new RoomDebuggerBox();
    [SerializeField]
    private Area area;

    [SerializeField]
    private bool visible = true;
    private bool held = false;

    private void Update() {
        if (Input.GetKey(KeyCode.D) && (Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && held == false) {
            held = true;
            visible = !visible;
        } 
        if (Input.GetKeyUp(KeyCode.D)) {
            held = false;
        }
    }
    private void OnGUI() {
#if UNITY_EDITOR
        if (visible) roomDebugger.Draw(area.CurrentRoom);
#endif
    }

}
