using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
///<summary>
/// UI slider that makes sounds when you click on it.
///</summary>
[RequireComponent(typeof(Slider))]
public class BaseSlider : MonoBehaviour, IPointerUpHandler, ISelectHandler, IDeselectHandler, IPointerDownHandler
{

    private bool canPlaySound = true;

    public IEnumerator DelayBeforeAbleToPlaySound() {
        canPlaySound = false;
        yield return new WaitForSeconds(.5f);
        canPlaySound = true;
    }

    [SerializeField]
    private Image knob;

    [SerializeField]
    private Sprite selectedKnobSprite;

    private Sprite idleSprite;


    private Slider slider;
    // Start is called before the first frame update

    private void Awake() {
        StartCoroutine(DelayBeforeAbleToPlaySound());
        idleSprite = knob.sprite;
    }
    void Start()
    {
        slider = GetComponent<Slider>();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        knob.sprite = idleSprite;
        if (canPlaySound) AudioHandler.Instance.PlayUISound(SFXFiles.slider_select, 1f,  .8f + .4f *(slider.value / slider.maxValue));
    }

    public void OnSelect(BaseEventData eventData)
    {
        knob.sprite = selectedKnobSprite;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        knob.sprite = idleSprite;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        knob.sprite = selectedKnobSprite;
    }
}
