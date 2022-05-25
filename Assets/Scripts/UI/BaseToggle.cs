using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

///<summary>
/// A toggle that plays a sound on click,hover and unhover.
///</summary>
[RequireComponent(typeof(Toggle))]
public class BaseToggle : MonoBehaviour, IPointerEnterHandler, ISelectHandler, IDeselectHandler, IPointerExitHandler
{

    private bool canPlaySound = true;
    public IEnumerator DelayBeforeAbleToPlaySound() {
        canPlaySound = false;
        yield return new WaitForSeconds(.5f);
        canPlaySound = true;
    }

    [SerializeField]
    private Sprite sprite_idle_Off;
    [SerializeField]
    private Sprite sprite_idle_On;
    [SerializeField]
    private Sprite sprite_selected_Off;
    [SerializeField]
    private Sprite sprite_selected_On;

    [SerializeField]
    private Image image;
    
    private Toggle toggle;
    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine(DelayBeforeAbleToPlaySound());
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnToggle);
        UpdateImage(false);

    }

    private void OnToggle(bool val) {
        if (canPlaySound) if (AudioHandler.Instance != null) AudioHandler.Instance.PlayUISound(val ? SFXFiles.toggle_on : SFXFiles.toggle_off,  1f, val ?  1f : .8f);
        UpdateImage(true);
    }

    private void UpdateImage(bool selected) {
        if (selected) {
            image.sprite = !toggle.isOn ? sprite_selected_Off : sprite_selected_On;
        } else {
            image.sprite = !toggle.isOn ? sprite_idle_Off : sprite_idle_On;
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        UpdateImage(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        UpdateImage(false);
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        UpdateImage(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UpdateImage(false);
    }
}
