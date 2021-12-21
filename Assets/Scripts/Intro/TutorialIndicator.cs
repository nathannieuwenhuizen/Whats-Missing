using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Animator))]
public class TutorialIndicator : Singleton<AudioHandler>
{

    private bool tutorialIsVisible = false;

    [SerializeField]
    private GameObject mouseUI;
    [SerializeField]
    private GameObject keyboardUI;
    [SerializeField]
    private GameObject spacebarUI;
    [SerializeField]
    private GameObject shiftUI;

    [SerializeField]
    private TMP_Text hintText;


    [SerializeField]
    private Animator animator;
    private bool mirrorComplete = false;
    private bool moved = false;
    private bool hasJumped = false;
    private bool hasShifted = false;

    private Coroutine fadeCoroutine;

    [SerializeField]
    private RectTransform mask;
    [SerializeField]
    private RectTransform maskChild;


    private void OnEnable() {
        MirrorCanvas.OnShowHint += ToggleHint;
        Area.OnFirstAreaEnter += StartWaitingForMove;
        Area.OnSecondAreaEnter += StartWaitingForShift;
        AreaTextMeshFader.onMirrorTutorialShow += StartWaitingMirrorComplete;
        Gravity.onGravityMissing += StartWaitingForJump;
        Letter.OnLetterClickAction += EnableClick;
        
        Room.OnRoomComplete += HideTutorial;
        Room.OnRoomLeaving += HideTutorial;
    }


    private void OnDisable() {
        MirrorCanvas.OnShowHint -= ToggleHint;
        Area.OnFirstAreaEnter -= StartWaitingForMove;
        Area.OnSecondAreaEnter -= StartWaitingForShift;
        AreaTextMeshFader.onMirrorTutorialShow -= StartWaitingMirrorComplete;
        Letter.OnLetterClickAction -= EnableClick;
        InputManager.OnMove -= EnableMove;
        InputManager.OnJump -= EnableJump;
        InputManager.OnStartRunning -= EnableShift;
        Gravity.onGravityMissing -= StartWaitingForJump;
        Room.OnRoomComplete -= HideTutorial;
        Room.OnRoomLeaving -= HideTutorial;
    }


    #region move
    private void EnableMove(Vector2 delta) {
        if (delta.magnitude != 0)
        {
            moved = true;
        }
    }
    public IEnumerator WaitForMove() {
        // moved = false;
        yield return new WaitForSeconds(7);
        if (!moved) {
            ShowTutorial(keyboardUI);
            while (!moved) {
                yield return new WaitForEndOfFrame();
            }
            HideTutorial();
        }
    }
    public void StartWaitingForMove() {
        InputManager.OnMove += EnableMove;
        StartCoroutine(WaitForMove());
    }
    #endregion

    #region  click
    public void StartWaitingMirrorComplete() {
        StartCoroutine(WaitingForMirrorComplete());
    }

    private void EnableClick(Letter letter) {
        mirrorComplete = true;
    }

    public IEnumerator WaitingForMirrorComplete() {
        yield return new WaitForSeconds(3);
        if (!mirrorComplete) {
            ShowTutorial(mouseUI);
            while (!mirrorComplete) {
                yield return new WaitForEndOfFrame();
            }
            HideTutorial();
        }
    }
    #endregion

    #region  jump
    private void EnableJump() {
        hasJumped = true;
    }

    public void StartWaitingForJump() {
        if (hasJumped) return;
        InputManager.OnJump += EnableJump;
        StartCoroutine(WaitForJump());
    }
    public IEnumerator WaitForJump() {
        yield return new WaitForSeconds(5);
        if (!hasJumped) {
            ShowTutorial(spacebarUI);
            while (!hasJumped) {
                yield return new WaitForEndOfFrame();
            }
            HideTutorial();
        }
    }

    #endregion
    #region  shift
    private void EnableShift() {
        hasShifted = true;
    }

    public void StartWaitingForShift() {
        if (hasShifted) return;
        InputManager.OnStartRunning += EnableShift;
        StartCoroutine(WaitForShift());
    }
    public IEnumerator WaitForShift() {
        yield return new WaitForSeconds(10);
        if (!hasShifted) {
            ShowTutorial(shiftUI);
            while (!hasShifted) {
                yield return new WaitForEndOfFrame();
            }
            HideTutorial();
        }
    }

    #endregion

    #region  hint
    public void ToggleHint(string value) {
        if (tutorialIsVisible) {
            HideTutorial();
        } else {
            hintText.text = value;
            ShowTutorial(hintText.gameObject);
            StartCoroutine(WaitBeforeDisappearing());
        }
    }

    private IEnumerator WaitBeforeDisappearing(){
        yield return new WaitForSeconds(5f);
        if (tutorialIsVisible) {
            HideTutorial();
        }
    }

    #endregion



    private void Update() {
        maskChild.transform.localScale = new Vector3(1,1 / mask.transform.localScale.y,1);
    }
    

    private void ShowTutorial(GameObject gameObject) {
        tutorialIsVisible = true;
        animator.SetBool("Show", true);

        mouseUI.SetActive(false);
        spacebarUI.SetActive(false);
        keyboardUI.SetActive(false);
        if(shiftUI != null) shiftUI.SetActive(false);
        hintText.gameObject.SetActive(false);

        gameObject.SetActive(true);
    }

    private void HideTutorial() {
        tutorialIsVisible = false;
        animator.SetBool("Show", false);
    }


}
