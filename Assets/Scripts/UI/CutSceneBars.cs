using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutSceneBars : MonoBehaviour
{
    [SerializeField]
    private RectTransform topBar;
    [SerializeField]
    private RectTransform bottomBar;

    private AnimationCurve curve = AnimationCurve.EaseInOut(0,0,1,1);
    public float BarHeight {
        get { return topBar.sizeDelta.y;}
        set { 
            topBar.sizeDelta = new Vector2(topBar.sizeDelta.x, value); 
            bottomBar.sizeDelta = new Vector2(bottomBar.sizeDelta.x, value); 
        }
    }

    public IEnumerator AnimateBars(float end, float duration, float delay = 0) {
        yield return new WaitForSeconds(delay);
        float index = 0;
        float begin = BarHeight;
         while (index < duration) {
             yield return new WaitForEndOfFrame();
             index += Time.unscaledDeltaTime;
             BarHeight = Mathf.Lerp(begin, end, index / duration);
         }
        BarHeight = end;
    }

    private void ShowBars() {
        StopAllCoroutines();
        StartCoroutine(AnimateBars(40f, 1f, 0));
    }
    private void HideBars() {
        StopAllCoroutines();
        StartCoroutine(AnimateBars(0f, 1f, 0));
    }

    private void Start() {
        BarHeight = 0;
    }

    private void OnEnable() {
        CharacterAnimationPlayer.OnCutsceneStart += ShowBars;
        CharacterAnimationPlayer.OnCutsceneEnd += HideBars;
        CreditsRoller.OnCreditsStart += HideBars;
    }
    private void OnDisable() {
        CharacterAnimationPlayer.OnCutsceneStart -= ShowBars;
        CharacterAnimationPlayer.OnCutsceneEnd -= HideBars;
        CreditsRoller.OnCreditsStart -= HideBars;
    }


    
}
