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
    
    public delegate void AxisAction( Vector2 delta);

    public static event AxisAction OnMove;
    public static event AxisAction OnRotate;


    // Update is called once per frame
    void Update()
    {
        //mouse
        if (Input.GetButtonDown("Fire1")) {
            OnClickDown?.Invoke();
        }
        if (Input.GetButtonUp("Fire1"))  {
            OnClickUp?.Invoke();
        }


        //controller
        if (Input.GetButtonDown("Fire1 Controller")) {
            OnClickDown?.Invoke();
        }
        if (Input.GetButtonUp("Fire1 Controller"))  {
            OnClickUp?.Invoke();
            if (TelevisionButton.SELECTED_BUTTON != null) TelevisionButton.SELECTED_BUTTON.gameObject.GetComponent<Button>().onClick.Invoke();
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

        //reset
        if (Input.GetKeyDown(KeyCode.R) && (Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl))) {
            OnReset?.Invoke();
        }

    }
}
