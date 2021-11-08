using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorUI : MonoBehaviour
{
    private CanvasGroup group;

    private void Awake() {
        group = GetComponent<CanvasGroup>();
    }

    private void HideCursorUI() {
        StopAllCoroutines();
        StartCoroutine(group.FadeCanvasGroup(0, 1f, 0));
    }
    private void ShowCursorUI() {
        StopAllCoroutines();
        StartCoroutine(group.FadeCanvasGroup(1, 1f, 0));
    }
        private void OnEnable() {
        Player.OnCutsceneStart += HideCursorUI;
        Player.OnCutsceneEnd += ShowCursorUI;
    }
    private void OnDisable() {
        Player.OnCutsceneStart -= HideCursorUI;
        Player.OnCutsceneEnd -= ShowCursorUI;
    }

}
