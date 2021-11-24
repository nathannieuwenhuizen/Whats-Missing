using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public static class UIExtensions
{
    public static IEnumerator FadeCanvasGroup( this CanvasGroup group, float end, float duration, float delay = 0) {
        yield return new WaitForSeconds(delay);
        float index = 0;
        float begin = group.alpha;
         while (index < duration) {
             yield return new WaitForEndOfFrame();
             index += Time.unscaledDeltaTime;
             group.alpha = Mathf.Lerp(begin, end, index / duration);
         }
        group.alpha = end;
    }
    public static IEnumerator FadeText( this TMP_Text text, float end, float duration, float delay = 0) {
        yield return new WaitForSeconds(delay);
        float index = 0;
        float begin = text.alpha;
         while (index < duration) {
             yield return new WaitForEndOfFrame();
             index += Time.unscaledDeltaTime;
             text.alpha = Mathf.Lerp(begin, end, index / duration);
         }
        text.alpha = end;
    }

}
