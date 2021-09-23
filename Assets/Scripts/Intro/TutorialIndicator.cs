using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialIndicator : MonoBehaviour
{

    [SerializeField]
    private CanvasGroup mouseUI;
    [SerializeField]
    private CanvasGroup keyboardUI;
    private bool mouseClicked = false;
    private bool moved = false;

    private Coroutine fadeCoroutine;

    public IEnumerator WaitForMouseClick() {
        mouseClicked = false;
        yield return new WaitForSeconds(2);
        if (!mouseClicked) {
            fadeCoroutine = StartCoroutine(mouseUI.FadeCanvasGroup(1f, 1f));
            while (!mouseClicked) {
                yield return new WaitForEndOfFrame();
            }
            StopAllCoroutines();
            fadeCoroutine = StartCoroutine(mouseUI.FadeCanvasGroup(0f, 1f));
        }
    }

    private void OnEnable() {
        InputManager.OnClickDown += EnableClick;
        InputManager.OnMove += EnableMove;
    }

    private void OnDisable() {
        InputManager.OnClickDown -= EnableClick;
        InputManager.OnMove -= EnableMove;
    }

    private void EnableClick() {
        mouseClicked = true;
    }
    private void EnableMove(Vector2 delta) {
        if (delta.magnitude != 0)
        {
            moved = true;
        }
    }

    
    public IEnumerator WaitForMove() {
        moved = false;
        yield return new WaitForSeconds(2);
        if (!moved) {
            fadeCoroutine = StartCoroutine(keyboardUI.FadeCanvasGroup(1f, 1f));
            while (!moved) {
                yield return new WaitForEndOfFrame();
            }
            StopAllCoroutines();
            fadeCoroutine = StartCoroutine(keyboardUI.FadeCanvasGroup(0f, 1f));
        }
    }
}
