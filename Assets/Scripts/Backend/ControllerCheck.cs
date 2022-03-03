using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public static class ControllerCheck
{
    private static bool Xbox_One_Controller = false;
    private static bool PS4_Controller = false;

    public static bool AnyControllerConnected()
    {
        checkJoystickNames();
        return Xbox_One_Controller || PS4_Controller;
    }
    public static void SelectUIGameObject(GameObject go, Action callBack = null) {
        if (ControllerCheck.AnyControllerConnected()) {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(go);
            if (callBack != null)
                callBack();
        }

    }

    public static bool IsInputFromGamePad(InputAction.CallbackContext obj) {
        var inputAction = obj.action;
        var binding = inputAction.GetBindingForControl(inputAction.activeControl).Value;
        return binding.groups == "GamePad";
    }
    private static void checkJoystickNames()
    {

        string[] names = Input.GetJoystickNames();
        for (int x = 0; x < names.Length; x++)
        {
            if (names[x].Length == 19)
            {
                PS4_Controller = true;
                Xbox_One_Controller = false;
            }
            if (names[x].Length == 33)
            {
                //set a controller bool to true
                PS4_Controller = false;
                Xbox_One_Controller = true;

            }
        }
    }
}
