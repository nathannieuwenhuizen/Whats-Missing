using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MirrorButton : MonoBehaviour, IPointerUpHandler
{
    ///<summary>
    /// Purely nessecary for the gamepad input of the world canvas
    ///</summary>
    public static MirrorButton SELECTED_BUTTON;
    
    public static bool BUTTON_DRAGGED = false;
    protected bool canBeClicked = true;
    protected Vector3 normalScale = Vector3.one;

    protected AnimationCurve scaleAnimation = AnimationCurve.EaseInOut(0,0,1,1);

    protected RectTransform rt;

    public virtual void OnHover() {
        if (!canBeClicked || BUTTON_DRAGGED) return;
        MirrorButton.SELECTED_BUTTON = this;
    }
    public virtual void OnUnhover() {
        if (!canBeClicked || BUTTON_DRAGGED ) return;
        if (MirrorButton.SELECTED_BUTTON == this) MirrorButton.SELECTED_BUTTON = null;
    }
    public virtual void Awake() {
        rt = GetComponent<RectTransform>();
        // rt.localScale = Vector3.zero;
    }

    protected IEnumerator ScaleAnimation(Vector3 endScale, float duration = .2f) {
        float index = 0;
        Vector3 begin = rt.localScale;
        while( index < duration) {
            index += Time.unscaledDeltaTime;
            rt.localScale = Vector3.LerpUnclamped(begin, endScale, scaleAnimation.Evaluate(index/ duration));
            yield return new WaitForEndOfFrame();
        }
        rt.localScale = endScale;
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
    }
}
