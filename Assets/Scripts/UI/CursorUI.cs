using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorUI : MonoBehaviour
{
    private CanvasGroup group;

    [SerializeField]
    private AnimationCurve rotationCurve;
    private bool hidden = false;
    private bool canHighLight = true;

    public bool CanHighLight {
        get { return canHighLight;}
        set { canHighLight = value; }
    }
    private float idleAlpha = .6f;
    private float highLightedAlpha = .6f;
    private float highLightDuration = .3f;

    private void Awake() {
        group = GetComponent<CanvasGroup>();
        group.alpha = idleAlpha;
    }

    private void HideCursorUI() {
        hidden = true;
        StopAllCoroutines();
        StartCoroutine(group.FadeCanvasGroup(0, 1f, 0));
    }
    private void ShowCursorUI() {
        hidden = false;
        if (canHighLight) ResetToIdle();
        StopAllCoroutines();
        StartCoroutine(group.FadeCanvasGroup(idleAlpha, 1f, 0));
    }
    private void OnEnable() {
        PauseScreen.OnPause += HideCursorUI;
        PauseScreen.OnResume += ShowCursorUI;
        CharacterAnimationPlayer.OnCutsceneStart += HideCursorUI;
        CharacterAnimationPlayer.OnCutsceneEnd += ShowCursorUI;
        Hands.OnFocus += HighLightCursorUI;
        Hands.OnUnfocus += UnhighlightCursorUI;
    }
    private void OnDisable() {
        PauseScreen.OnPause -= HideCursorUI;
        PauseScreen.OnResume -= ShowCursorUI;
        CharacterAnimationPlayer.OnCutsceneStart -= HideCursorUI;
        CharacterAnimationPlayer.OnCutsceneEnd -= ShowCursorUI;
        Hands.OnFocus -= HighLightCursorUI;
        Hands.OnUnfocus -= UnhighlightCursorUI;
    }

    private void ResetToIdle() {
        group.alpha = idleAlpha;
        group.GetComponent<RectTransform>().rotation = Quaternion.Euler(0,0,90f);
        group.GetComponent<RectTransform>().localScale = Vector3.one;
    }

    private void HighLightCursorUI(bool whiteColor) {
        if (hidden || !canHighLight) return;
        StopAllCoroutines();
        StartCoroutine(group.FadeCanvasGroup(highLightedAlpha, highLightDuration));
        StartCoroutine(group.GetComponent<RectTransform>().AnimatingLocalScale(Vector3.one * 1.5f, AnimationCurve.EaseInOut(0,0,1,1), highLightDuration));
        StartCoroutine(group.GetComponent<RectTransform>().AnimatingLocalRotation(Quaternion.Euler(0,0,90f), rotationCurve, highLightDuration));
    }
    private void UnhighlightCursorUI(bool whiteColor) {
        if (hidden || !canHighLight) return;
        StopAllCoroutines();
        StartCoroutine(group.FadeCanvasGroup(idleAlpha, highLightDuration));
        StartCoroutine(group.GetComponent<RectTransform>().AnimatingLocalScale(Vector3.one, AnimationCurve.EaseInOut(0,0,1,1), highLightDuration));
        StartCoroutine(group.GetComponent<RectTransform>().AnimatingLocalRotation(Quaternion.Euler(0,0,0), rotationCurve, highLightDuration));
    }

}
