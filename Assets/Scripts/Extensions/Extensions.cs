using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{


    public static IEnumerator Fade( this CanvasGroup group, float end, float duration) {
        float index = 0;
        float begin = group.alpha;
         while (index < duration) {
             yield return new WaitForEndOfFrame();
             index += Time.deltaTime;
             group.alpha = Mathf.Lerp(begin, end, index / duration);
         }
        group.alpha = end;
    }
}
