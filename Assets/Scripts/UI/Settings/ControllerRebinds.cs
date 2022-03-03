using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputBinding;

public class ControllerRebinds : MonoBehaviour
{

    public delegate void RebindChangedEvent(GamePlayControls _controls);
    public static RebindChangedEvent OnRebindChanged;

    private readonly string PLAYER_PREF_REIND_KEY = "input_rebinds";
    public static GamePlayControls controls;

    [SerializeField]
    private GameObject prefabTemplate;

    [SerializeField]
    private GameObject rebindPopup;

    List<RebindKey> rebindKeys = new List<RebindKey>();

    private void Awake() {
        ControllerRebinds.controls = LoadRebinds();
        ControllerRebinds.controls.Player.Enable();

        InstantiateUI();
        rebindPopup.SetActive(false);
    }

    private void OnEnable() {
        RebindKey.OnRebindingBegin += OnRebind;
    }

    private void OnDisable() {
        RebindKey.OnRebindingBegin -= OnRebind;
    }

    public void OnRebind(RebindKey rebindKey) {
        Debug.Log("rebinding key event");
        StartCoroutine(OnRebinding(rebindKey));
    }

    IEnumerator OnRebinding(RebindKey rebindKey) {
        ControllerRebinds.controls.Player.Disable();
        Debug.Log("select new key...");
        rebindPopup.SetActive(true);
        EnableEditButtons(false);

        rebindKey.Action.PerformInteractiveRebinding(rebindKey.GetBindingIndex)
        .OnComplete(callback => {
            OnRebindEnd();
            rebindKey.UpdateUI();
            Debug.Log("new key selected: " + rebindKey.Action.bindings[rebindKey.GetBindingIndex].ToDisplayString(DisplayStringOptions.DontUseShortDisplayNames));
            callback.Dispose();
        }) .Start();

        yield return null;
    }
    public void OnRebindEnd() {
        rebindPopup.SetActive(false);
        EnableEditButtons(true);
        controls.Enable();
        SaveRebinds();
    }

    public void EnableEditButtons(bool val) {
        foreach (RebindKey  key in rebindKeys) {
            key.EditButton.Button.interactable = val;
        }
    }

    private void SaveRebinds() {
        
        string data = controls.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString(PLAYER_PREF_REIND_KEY, data);
        Debug.Log("data = " + data);
        OnRebindChanged?.Invoke(controls);
    }

    ///<summary>
    /// Loads the rebinds from the playerpref
    ///</summary>
    private GamePlayControls LoadRebinds() {
        controls = new GamePlayControls();
        // controls.Enable();

        string data = PlayerPrefs.GetString(PLAYER_PREF_REIND_KEY, "");
        if (data != "") {
            controls.LoadBindingOverridesFromJson(data);
        }

        OnRebindChanged?.Invoke(controls);

        return controls;
    }

    public void ResetRebinds() {
        PlayerPrefs.SetString(PLAYER_PREF_REIND_KEY, "");
        ControllerRebinds.controls.Disable();
        ControllerRebinds.controls = LoadRebinds();

        ControllerRebinds.controls.Player.Enable();

        DestroyUI();
        InstantiateUI();

        //stupid scale bug fix
        foreach(RebindKey key in rebindKeys) {
            key.transform.localScale = Vector3.one;
            key.GetComponent<RectTransform>().localScale = Vector3.one;
        }
    }

    private void InstantiateUI() {
        prefabTemplate.SetActive(true);
        rebindKeys.Add(InstantiateRebindKey(controls.Player.Jump));
        rebindKeys.Add(InstantiateRebindKey(controls.Player.Click));
        rebindKeys.Add(InstantiateRebindKey(controls.Player.Run));
        rebindKeys.Add(InstantiateRebindKey(controls.Player.Cancel));
        // rebindKeys.Add(InstantiateRebindKey(controls.Player.Movement));
        prefabTemplate.SetActive(false);
    }

    private void DestroyUI() {
        foreach(RebindKey key in rebindKeys) {
           Destroy(key.gameObject);
        }
        rebindKeys = new List<RebindKey>();
    }

    private RebindKey InstantiateRebindKey(InputAction _action) {
        GameObject temp = Instantiate(prefabTemplate);
        temp.name = _action.name;
        temp.transform.SetParent(prefabTemplate.transform.parent);
        RebindKey newRebindKey = temp.GetComponent<RebindKey>();
        newRebindKey.Action = _action;
        return newRebindKey;
    }
}
