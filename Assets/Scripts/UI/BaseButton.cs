using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

///<summary>
/// A button that plays a sound on click,hover and unhover.
///</summary>
[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class BaseButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private Animator shineAnimator;

    [SerializeField]
    private Sprite selectedSprite;
    [SerializeField]
    private Sprite clickedSprite;
    private Sprite idleSprite;

    private Button button;
    public Button Button {
        get { return button;}
    }
    private Image image;
    private void Awake() {
        image = GetComponent<Image>();
        idleSprite = image.sprite;

        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick() {
        // image.sprite = clickedSprite;
        AudioHandler.Instance.PlayUISound(SFXFiles.ui_button_click);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // shineAnimator.SetTrigger("shine");
        image.sprite = selectedSprite;
        AudioHandler.Instance.PlayUISound(SFXFiles.ui_button_hover, .05f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.sprite = idleSprite;
        // AudioHandler.Instance.PlayUISound(SFXFiles.ui_button_unhover);
    }

    public void OnSelect(BaseEventData eventData)
    {
        // shineAnimator.SetTrigger("shine");
        image.sprite = selectedSprite;
        AudioHandler.Instance.PlayUISound(SFXFiles.ui_button_hover, .05f);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        image.sprite = idleSprite;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        image.sprite = clickedSprite;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        image.sprite = selectedSprite;
    }
}
