using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputBinding;

public class RebindKey : MonoBehaviour
{

    public delegate void RebindAction(RebindKey rebindKey);
    public static RebindAction OnRebindingBegin;
    [SerializeField]
    private TMP_Text keyText;
    [SerializeField]
    private TMP_Text valueText;
    [SerializeField]
    public BaseButton editButton;

    public BaseButton EditButton {
        get { return editButton;}
    }


    private InputAction action;
    public InputAction Action {
        get { return action;}
        set { 
            action = value; 
            UpdateUI();
        }
    }

    public int GetBindingIndex {
        get {
            return ControllerCheck.AnyControllerConnected() ? (Action.bindings.Count - 1) : 0;
        }
    }


    public void UpdateUI() {
        keyText.text = action.name;
        for(int i = 0 ; i < action.bindings.Count; i++) {
            // Debug.Log("bindings name: " + action.bindings[i].ToDisplayString(DisplayStringOptions.DontUseShortDisplayNames));
        }
        valueText.text = action.bindings[GetBindingIndex].ToDisplayString(DisplayStringOptions.DontUseShortDisplayNames);
    }

    public void ChangeBinding() {
        OnRebindingBegin?.Invoke(this);
    }
}
