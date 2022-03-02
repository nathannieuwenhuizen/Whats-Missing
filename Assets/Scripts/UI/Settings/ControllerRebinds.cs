using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerRebinds : MonoBehaviour
{

    private GamePlayControls controls;

    [SerializeField]
    private GameObject prefabTemplate;

    [SerializeField]
    private GameObject rebindPopup;

    List<RebindKey> rebindKeys = new List<RebindKey>();

    private void Awake() {
        controls = new GamePlayControls();
        controls.Enable();
        InstantiateUI();
        rebindPopup.SetActive(false);
    }

    private void OnEnable() {
        RebindKey.OnRebindingBegin += OnRebindBegin;
        RebindKey.OnRebindingEnd += OnRebindEnd;
    }

    private void OnDisable() {
        RebindKey.OnRebindingBegin += OnRebindBegin;
        RebindKey.OnRebindingEnd += OnRebindEnd;
    }

    public void OnRebindBegin() {
        rebindPopup.SetActive(true);
        EnableEditButtons(false);
        controls.Disable();
    }
    public void OnRebindEnd() {
        rebindPopup.SetActive(false);
        EnableEditButtons(true);
        controls.Enable();
    }

    public void EnableEditButtons(bool val) {
        foreach (RebindKey  key in rebindKeys) {
            key.EditButton.Button.interactable = val;
        }
    }

    private void InstantiateUI() {
        rebindKeys.Add(InstantiateRebindKey(controls.Player.Jump));
        rebindKeys.Add(InstantiateRebindKey(controls.Player.Click));
        rebindKeys.Add(InstantiateRebindKey(controls.Player.Run));
        // rebindKeys.Add(InstantiateRebindKey(controls.Player.Movement));
        prefabTemplate.SetActive(false);
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
