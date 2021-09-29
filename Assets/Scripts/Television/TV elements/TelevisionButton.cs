using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TelevisionButton : MonoBehaviour
{
    public static TelevisionButton SELECTED_BUTTON;
    protected bool canBeClicked = true;
    protected Vector3 normalScale = Vector3.one;

    [SerializeField]
    protected AnimationCurve slideAnimation;

    protected RectTransform rt;

    public virtual void OnHover() {
        if (!canBeClicked) return;
        TelevisionButton.SELECTED_BUTTON = this;
    }
    public virtual void OnUnhover() {
        if (!canBeClicked) return;
        if (TelevisionButton.SELECTED_BUTTON == this) TelevisionButton.SELECTED_BUTTON = null;
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
            rt.localScale = Vector3.LerpUnclamped(begin, endScale, slideAnimation.Evaluate(index/ duration));
            yield return new WaitForEndOfFrame();
        }
        rt.localScale = endScale;
    }


}
