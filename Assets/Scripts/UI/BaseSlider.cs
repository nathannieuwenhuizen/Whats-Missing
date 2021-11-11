using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
///<summary>
/// UI slider that makes sounds when you click on it.
///</summary>
[RequireComponent(typeof(Slider))]
public class BaseSlider : MonoBehaviour, IPointerUpHandler 
{
    private Slider slider;
    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
    }

    private void OnToggle(bool val) {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        AudioHandler.Instance.PlayUISound(SFXFiles.slider_select, 1f,  .8f + .4f *(slider.value / slider.maxValue));
    }
}
