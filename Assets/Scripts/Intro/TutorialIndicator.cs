using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Animator))]
public class TutorialIndicator : MonoBehaviour
{

    private bool tutorialIsVisible = false;
    [Header("Keyboard")]
    [SerializeField]
    private GameObject mouseUI;
    [SerializeField]
    private GameObject keyboardUI;
    [SerializeField]
    private GameObject spacebarUI;
    [SerializeField]
    private GameObject gravityUI;
    [SerializeField]
    private GameObject shiftUI;
    [Header("Controller")]
    [SerializeField]
    private GameObject mouseControllerUI;
    [SerializeField]
    private GameObject keyboardControllerUI;
    [SerializeField]
    private GameObject spacebarControllerUI;
    [SerializeField]
    private GameObject gravityControllerUI;
    [SerializeField]
    private GameObject shiftControllerUI;
    [Header("Other")]

    [SerializeField]
    private TMP_Text hintText;


    [SerializeField]
    private Animator animator;
    private bool mirrorComplete = false;
    private bool moved = false;
    private bool hasJumped = false;
    private bool hasShifted = false;
    private bool hasGravityJumped = false;

    private Coroutine fadeCoroutine;

    [SerializeField]
    private RectTransform mask;
    [SerializeField]
    private RectTransform maskChild;


    public void OnEnable() {
        MirrorCanvas.OnShowHint += ToggleHint;
        Area.OnFirstAreaEnter += StartWaitingForMove;
        Area.OnSecondAreaEnter += StartWaitingForShift;
        AreaTextMeshFader.onMirrorTutorialShow += StartWaitingMirrorComplete;
        Floor.OnFloorMissing += StartWaitingForJump;
        Letter.OnLetterClickAction += EnableClick;
        FPMovement.OnFloating += StartWaitingForGravityJump;
        Gravity.onGravityMissingInThirdArea += StartWaitingForGravityJump;
        
        Room.OnRoomComplete += HideTutorial;
        Room.OnRoomLeaving += HideTutorial;
    }


    public void OnDisable() {
        MirrorCanvas.OnShowHint -= ToggleHint;
        Area.OnFirstAreaEnter -= StartWaitingForMove;
        Area.OnSecondAreaEnter -= StartWaitingForShift;
        AreaTextMeshFader.onMirrorTutorialShow -= StartWaitingMirrorComplete;
        Letter.OnLetterClickAction -= EnableClick;
        InputManager.OnMove -= EnableMove;
        InputManager.OnJump -= EnableJump;
        InputManager.OnStartRunning -= EnableShift;
        Floor.OnFloorMissing -= StartWaitingForJump;
        Room.OnRoomComplete -= HideTutorial;
        Room.OnRoomLeaving -= HideTutorial;
    }


    #region move
    private void EnableMove(Vector2 delta) {
        if (delta.magnitude != 0)
            moved = true;
    }
    public IEnumerator WaitForMove() {
        // moved = false;
        yield return new WaitForSeconds(7);
        if (!moved) {
            ShowTutorial(ControllerCheck.AnyControllerConnected() ? keyboardControllerUI: keyboardUI);
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
        mirrorComplete = false;
        yield return new WaitForSeconds(3);
        if (!mirrorComplete) {
            ShowTutorial(ControllerCheck.AnyControllerConnected() ? mouseControllerUI : mouseUI);
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
            ShowTutorial(ControllerCheck.AnyControllerConnected() ? spacebarControllerUI : spacebarUI);
            while (!hasJumped) {
                yield return new WaitForEndOfFrame();
            }
            HideTutorial();
        }
    }
    #endregion
    #region  gravity jump
    private void EnableGravity() {
        hasGravityJumped = true;
    }

    public void StartWaitingForGravityJump() {
        if (hasGravityJumped) return;
        FPMovement.OnFloatDownJump += EnableGravity;
        StartCoroutine(WaitForGravityJump());
    }
    public IEnumerator WaitForGravityJump() {
        yield return new WaitForSeconds(2f);
        if (!hasGravityJumped) {
            ShowTutorial(ControllerCheck.AnyControllerConnected() ? gravityControllerUI : gravityUI);
            while (!hasGravityJumped) {
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
            ShowTutorial(ControllerCheck.AnyControllerConnected() ? shiftControllerUI : shiftUI);
            while (!hasShifted) {
                yield return new WaitForEndOfFrame();
            }
            HideTutorial();
        }
    }

    #endregion

    #region  hint
    public void ToggleHint(string value, float duration) {
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



    public void Update() {
        maskChild.transform.localScale = new Vector3(1,1 / mask.transform.localScale.y,1);
    }
    

    private void ShowTutorial(GameObject selectedUI) {
        tutorialIsVisible = true;
        animator.SetBool("Show", true);

        mouseUI.SetActive(false);
        spacebarUI.SetActive(false);
        keyboardUI.SetActive(false);
        shiftUI.SetActive(false);
        gravityUI.SetActive(false);

        mouseControllerUI.SetActive(false);
        spacebarControllerUI.SetActive(false);
        keyboardControllerUI.SetActive(false);
        shiftControllerUI.SetActive(false);
        gravityControllerUI.SetActive(false);

        if(shiftUI != null) shiftUI.SetActive(false);
        if(shiftControllerUI != null) shiftControllerUI.SetActive(false);
        hintText.gameObject.SetActive(false);

        selectedUI.SetActive(true);
    }

    private void HideTutorial() {
        tutorialIsVisible = false;
        animator.SetBool("Show", false);
    }


}
