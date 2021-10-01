using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{


    public static Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2) {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;
        return p;
    }

    public static Vector3 CalculateLinearBezierPoint(float t, Vector3 p0, Vector3 p1) {
        return p0 + t * (p1 - p0);
    }


    public static IEnumerator FadeCanvasGroup( this CanvasGroup group, float end, float duration) {
        float index = 0;
        float begin = group.alpha;
         while (index < duration) {
             yield return new WaitForEndOfFrame();
             index += Time.unscaledDeltaTime;
             group.alpha = Mathf.Lerp(begin, end, index / duration);
         }
        group.alpha = end;
    }

    public static  IEnumerator AnimatingScale(this Transform transform, Vector3 endScale,  AnimationCurve curve, float duration = .5f) {
        float timePassed = 0f;
        Vector3 beginScale = transform.localScale;
        while (timePassed < duration) {
            yield return new WaitForEndOfFrame();
            timePassed += Time.unscaledDeltaTime;
            transform.localScale = Vector3.LerpUnclamped(beginScale, endScale , curve.Evaluate(timePassed / duration));
        }
        transform.localScale = endScale;

    }

    public static  IEnumerator AnimatingDissolveMaterial(this Material mat, float beginVal, float endVal,  AnimationCurve curve, float duration = .5f) {
        mat.SetFloat("EdgeWidth", .1f);
        mat.SetFloat("Dissolve", beginVal);
        float timePassed = 0f;

        while (timePassed < duration) {
            yield return new WaitForEndOfFrame();
            timePassed += Time.unscaledDeltaTime;
            mat.SetFloat("Dissolve", Mathf.LerpUnclamped(beginVal, endVal , curve.Evaluate(timePassed / duration)));
        }
        mat.SetFloat("Dissolve", endVal);
        mat.SetFloat("EdgeWidth", 0);
    }
}
