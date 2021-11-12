using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

///<summary>
/// A button that plays a sound on click,hover and unhover.
///</summary>
[RequireComponent(typeof(Button))]
public class BaseButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler
{

    private Button button;
    private void Awake() {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick() {
        AudioHandler.Instance.PlayUISound(SFXFiles.ui_button_click);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioHandler.Instance.PlayUISound(SFXFiles.ui_button_hover);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // AudioHandler.Instance.PlayUISound(SFXFiles.ui_button_unhover);
    }

    public void OnSelect(BaseEventData eventData)
    {
        AudioHandler.Instance.PlayUISound(SFXFiles.ui_button_hover);
    }
}
