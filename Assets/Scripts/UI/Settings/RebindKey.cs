using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputBinding;

public class RebindKey : MonoBehaviour
{

    public delegate void RebindAction();
    public static RebindAction OnRebindingBegin;
    public static RebindAction OnRebindingEnd;
    [SerializeField]
    private TMP_Text keyText;
    [SerializeField]
    private TMP_Text valueText;
    [SerializeField]
    private BaseButton editButton;

    public BaseButton EditButton {
        get { return editButton;}
    }


    private InputAction action;
    public InputAction Action {
        get { return action;}
        set { 
            action = value; 
            UpdateBindingUI();
        }
    }


    public void UpdateBindingUI() {
        keyText.text = action.name;
        valueText.text = action.bindings[0].ToDisplayString(DisplayStringOptions.DontUseShortDisplayNames);
    }

    public void ChangeBinding() {
        OnRebindingBegin?.Invoke();
        Debug.Log("select new key...");
        action.PerformInteractiveRebinding()
        .OnComplete(callback => {

            //...
            OnRebindingEnd?.Invoke();
            UpdateBindingUI();
            Debug.Log("new key selected: " + action.bindings[0].ToDisplayString(DisplayStringOptions.DontUseShortDisplayNames));
            callback.Dispose();
        }) .Start();
    }
}
