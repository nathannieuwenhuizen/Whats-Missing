using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MirrorButton : MonoBehaviour
{
    public static MirrorButton SELECTED_BUTTON;
    protected bool canBeClicked = true;
    protected Vector3 normalScale = Vector3.one;

    protected AnimationCurve scaleAnimation = AnimationCurve.EaseInOut(0,0,1,1);

    protected RectTransform rt;

    public virtual void OnHover() {
        if (!canBeClicked) return;
        MirrorButton.SELECTED_BUTTON = this;
    }
    public virtual void OnUnhover() {
        if (!canBeClicked) return;
        if (MirrorButton.SELECTED_BUTTON == this) MirrorButton.SELECTED_BUTTON = null;
    }
    public virtual void Awake() {
        rt = GetComponent<RectTransform>();
        // rt.localScale = Vector3.zero;
    }
    public virtual void Start() {
        // rt.localScale = Vector3.zero;
    }
    public virtual void OnEnable() {
        // rt.localScale = Vector3.one;
        // rt.localScale = Vector3.zero;
        // StartCoroutine(ScaleAnimation(normalScale, 1f));
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


}
