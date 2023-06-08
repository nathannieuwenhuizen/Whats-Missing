using System.Collections;
using System.Collections.Generic;
using Coffee.UIEffects;
using UnityEngine;
using UnityEngine.UI;

public class CutSceneBars : MonoBehaviour
{
    [SerializeField]
    private RectTransform topBar;
    [SerializeField]
    private RectTransform bottomBar;

    private AnimationCurve curve = AnimationCurve.EaseInOut(0,0,1,1);
    private float barHeight = 0;
    public float BarHeight {
        get { 
            return barHeight;
        }
        set { 
            barHeight = value;
            Debug.Log("value cutscene: " + value);
            topBar.GetComponent<UITransitionEffect>().effectFactor = value;
            topBar.sizeDelta = new Vector2(topBar.sizeDelta.x, 60); 
            bottomBar.GetComponent<UITransitionEffect>().effectFactor = value;
            bottomBar.sizeDelta = new Vector2(bottomBar.sizeDelta.x, 60); 
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
        StartCoroutine(AnimateBars(1f, 1f, 0));
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
