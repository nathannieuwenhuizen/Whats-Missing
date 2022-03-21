using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HeaderText : MonoBehaviour
{
    public Vector3 Position {
        get { return transform.localPosition; }
    }

    private float movingDuration = .5f;
    private Coroutine movingCoroutine;
    private float movingIndex;
    private Vector3 startMovePos;
    private TMP_Text text; 
    public TMP_Text Text {
        get { return text;}
    }
    protected AnimationCurve scaleAnimation = AnimationCurve.EaseInOut(0,0,1,1);


    protected RectTransform rt;

    public virtual void Awake() {
        text = GetComponent<TMP_Text>();
        rt = GetComponent<RectTransform>();

    }


    public void MoveTo( Vector3 pos) {
        // in animation
        if (movingCoroutine != null) StopCoroutine(movingCoroutine);
        if (movingCoroutine != null && movingIndex < movingDuration && movingIndex > 0) {
            movingCoroutine =  StartCoroutine(Moving(pos));
            return;
        }
        movingIndex = 0;
        startMovePos = rt.localPosition;
        movingCoroutine =  StartCoroutine(Moving(pos));
    }
    private IEnumerator Moving(Vector3 pos) {
        while( movingIndex < movingDuration) {
            movingIndex += Time.unscaledDeltaTime;
            rt.localPosition = Vector3.LerpUnclamped(startMovePos, pos, scaleAnimation.Evaluate(movingIndex/ movingDuration));
            yield return new WaitForEndOfFrame();
        }
        movingIndex = 1;
        rt.localPosition = pos;
    }
}
