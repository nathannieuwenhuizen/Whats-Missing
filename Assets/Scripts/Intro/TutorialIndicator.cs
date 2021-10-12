using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialIndicator : Singleton<AudioHandler>
{

    [SerializeField]
    private CanvasGroup mouseUI;
    [SerializeField]
    private CanvasGroup keyboardUI;
    [SerializeField]
    private CanvasGroup spacebarUI;
    private bool mouseClicked = false;
    private bool moved = false;
    private bool hasJumped = false;

    private Coroutine fadeCoroutine;

    public IEnumerator WaitForMouseClick() {
        // mouseClicked = false;
        yield return new WaitForSeconds(3);
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
        Area.OnFirstRoomEnter += StartWaitingForMove;
        AreaTextMeshFader.onTVTurialShow += StartWaitingForClick;
        Gravity.onGravityMissing += StartWaitingForJump;
    }


    private void OnDisable() {
        Area.OnFirstRoomEnter -= StartWaitingForMove;
        AreaTextMeshFader.onTVTurialShow -= StartWaitingForClick;
        Letter.OnLetterClickAction -= EnableClick;
        InputManager.OnMove -= EnableMove;
        InputManager.OnJump -= EnableJump;
        Gravity.onGravityMissing -= StartWaitingForJump;
    }

    private void EnableClick(Letter letter) {
        mouseClicked = true;
    }
    private void EnableMove(Vector2 delta) {
        if (delta.magnitude != 0)
        {
            moved = true;
        }
    }

    private void EnableJump() {
        hasJumped = true;
    }

    public void StartWaitingForMove() {
        InputManager.OnMove += EnableMove;
        StartCoroutine(WaitForMove());
    }
    public void StartWaitingForClick() {
        Letter.OnLetterClickAction += EnableClick;
        StartCoroutine(WaitForMouseClick());
    }
    public void StartWaitingForJump() {
        InputManager.OnJump += EnableJump;
        StartCoroutine(WaitForJump());
    }
    
    public IEnumerator WaitForMove() {
        // moved = false;
        yield return new WaitForSeconds(3);
        if (!moved) {
            fadeCoroutine = StartCoroutine(keyboardUI.FadeCanvasGroup(1f, 1f));
            while (!moved) {
                yield return new WaitForEndOfFrame();
            }
            StopAllCoroutines();
            fadeCoroutine = StartCoroutine(keyboardUI.FadeCanvasGroup(0f, 1f));
        }
    }
    public IEnumerator WaitForJump() {
        // hasJumped = false;
        yield return new WaitForSeconds(5);
        if (!hasJumped) {
            fadeCoroutine = StartCoroutine(spacebarUI.FadeCanvasGroup(1f, 1f));
            while (!hasJumped) {
                yield return new WaitForEndOfFrame();
            }
            StopAllCoroutines();
            fadeCoroutine = StartCoroutine(spacebarUI.FadeCanvasGroup(0f, 1f));
        }
    }
}
