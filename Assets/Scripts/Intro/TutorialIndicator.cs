using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class TutorialIndicator : Singleton<AudioHandler>
{

    [SerializeField]
    private GameObject mouseUI;
    [SerializeField]
    private GameObject keyboardUI;
    [SerializeField]
    private GameObject spacebarUI;

    [SerializeField]
    private Animator animator;
    private bool mirrorComplete = false;
    private bool moved = false;
    private bool hasJumped = false;

    private Coroutine fadeCoroutine;

    [SerializeField]
    private RectTransform mask;
    [SerializeField]
    private RectTransform maskChild;


    private void OnEnable() {
        Area.OnFirstRoomEnter += StartWaitingForMove;
        AreaTextMeshFader.onMirrorTutorialShow += StartWaitingMirrorComplete;
        Gravity.onGravityMissing += StartWaitingForJump;
        Room.OnRoomComplete += EnableClick;
    }


    private void OnDisable() {
        Area.OnFirstRoomEnter -= StartWaitingForMove;
        AreaTextMeshFader.onMirrorTutorialShow -= StartWaitingMirrorComplete;
        Room.OnRoomComplete -= EnableClick;
        InputManager.OnMove -= EnableMove;
        InputManager.OnJump -= EnableJump;
        Gravity.onGravityMissing -= StartWaitingForJump;
    }

    private void EnableClick() {
        mirrorComplete = true;
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
    public void StartWaitingMirrorComplete() {
        StartCoroutine(WaitingForMirrorComplete());
    }
    public void StartWaitingForJump() {
        InputManager.OnJump += EnableJump;
        StartCoroutine(WaitForJump());
    }

    private void Update() {
        maskChild.transform.localScale = new Vector3(1,1 / mask.transform.localScale.y,1);
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

    private void ShowTutorial(GameObject gameObject) {
        animator.SetBool("Show", true);

        mouseUI.SetActive(false);
        spacebarUI.SetActive(false);
        keyboardUI.SetActive(false);

        gameObject.SetActive(true);
    }

    private void HideTutorial() {
        animator.SetBool("Show", false);
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

    public IEnumerator WaitForJump() {
        // hasJumped = false;
        yield return new WaitForSeconds(5);
        if (!hasJumped) {
            ShowTutorial(spacebarUI);
            while (!hasJumped) {
                yield return new WaitForEndOfFrame();
            }
            HideTutorial();
        }
    }
}
