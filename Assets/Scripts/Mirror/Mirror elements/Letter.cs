using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
public class Letter : MirrorButton, IPointerDownHandler
{
    public delegate void LetterClickedEvent(Letter letter);
    public static event LetterClickedEvent OnLetterClickAction;
    
    public delegate void LetterClickedDragEvent(Letter letter, bool canBeDragged);
    public event LetterClickedDragEvent onLetterClick;

    private Button button;

    private bool selected = false;
    public bool Selected { get => selected; }
    private bool preClickSelected = false;
    public bool PreClickSelected { get => preClickSelected; set => preClickSelected = value; }

    private float movingDuration = .5f;

    private bool pressed = false;
    public float pressedTime;
    private float pressedTimeBeforeDrag = 0.4f;
    private MirrorCanvas mirrorCanvas;
    public MirrorCanvas MirrorCanvas {
        get { return mirrorCanvas;}
        set { mirrorCanvas = value; }
    }

    private Coroutine movingCoroutine;
    private float movingIndex;
    private Vector3 startMovePos;
    [SerializeField]
    private TMP_Text text; 
    public TMP_Text Text {
        get { return text;}
    }
    [SerializeField]
    private ParticleSystem clickParticle;

    private Coroutine hoverCoroutine;
    private Vector3 hoverScale = new Vector3(1.3f,1.3f,1.3f);
    

    private Vector3 spawnPosition = Vector3.zero;

    private string letterValue;

    // sets the value of the letter
    public string LetterValue {
        get {return letterValue;}
        set {
            letterValue = value;
            text.text = value;
        }
    }


    public override void Awake()
    {
        base.Awake();
        button = GetComponent<Button>();
        button.onClick.AddListener(() => LetterIsClicked());
        // text = GetComponent<TMP_Text>();
    }


    public override void OnHover() {
        base.OnHover();
        if (!interactable || BUTTON_DRAGGED) return;
        if (hoverCoroutine != null) StopCoroutine(hoverCoroutine);
        hoverCoroutine = StartCoroutine(ScaleAnimation(hoverScale));
    }
    public override void OnUnhover() {
        base.OnUnhover();
        if (!interactable || BUTTON_DRAGGED) return;
        if (hoverCoroutine != null) StopCoroutine(hoverCoroutine);
        hoverCoroutine = StartCoroutine(ScaleAnimation(normalScale));
    }


    public void Select()
    {
        OnUnhover();
        selected = true;
        if (spawnPosition == Vector3.zero) spawnPosition = rt.localPosition;
    }

    public void Deselect(bool withStartPositionAsign = false)
    {
        if (withStartPositionAsign)startMovePos = rt.localPosition;
        selected = false;
        Color = DefaultColor;
        MoveTo(spawnPosition);
    }

    public Size size {
        get {
            text.ForceMeshUpdate();
            return new Size() {width = text.GetRenderedValues(true).x, height = text.GetRenderedValues(true).y};
        }
    }

    public Color Color {
        get => text.color;
        set => text.color = value;
    }
    private Color defaultColor = Color.white;
    public Color DefaultColor {
        get { return defaultColor;}
        set { 
            defaultColor = value; 
            Color = value;
        }
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        LetterClickStart();
    }
    public void LetterClickStart() {
        pressed = true;
        pressedTime = Time.time;
        if (spawnPosition == Vector3.zero) spawnPosition = rt.localPosition;
        if (Selected) {
            PreClickSelected = true;
            mirrorCanvas.RemoveSelectedLetter(mirrorCanvas.selectedLetterObjects.IndexOf(this));
            StopAllCoroutines();
        } else {
            PreClickSelected = false;
        }

        StartCoroutine(Dragging());
    }

    void LetterIsClicked()
    {
        if (!interactable || !pressed) return;
        pressed = false;
        bool dragged = Dragged();
        OnLetterClickAction?.Invoke(this);
        onLetterClick?.Invoke(this, true);
        clickParticle.Emit(20);
        pressedTime = 0;
    }

    public bool Dragged() {
        return (pressedTime != 0 &&  Time.time - pressedTime > pressedTimeBeforeDrag);
    }
    private IEnumerator Dragging() {
        MirrorButton.BUTTON_DRAGGED = true;
        Canvas canvas = MirrorCanvas.Canvas;
        while(pressed) {

            transform.position = canvas.MouseToWorldPosition();
            transform.localPosition = new Vector3(
                transform.localPosition.x,
                transform.localPosition.y,
                transform.localPosition.z - 10f
                );
            mirrorCanvas.UpdateLetterDrag(this);

            MirrorButton.SELECTED_BUTTON = this;

            //out of mirror view or click out
            if (mirrorCanvas.IsInteractable == false || !Extensions.IsPressed(ControllerRebinds.controls.Player.Click)) {
                LetterIsClicked();
            }
            yield return new WaitForEndOfFrame();
        }
        MirrorButton.BUTTON_DRAGGED = false;

    }


    public Vector3 Position {
        get { return transform.localPosition; }
    }

    public void MoveTo( Vector3 pos) {
        // in animation
        if (movingCoroutine != null) StopCoroutine(movingCoroutine);
        if (movingCoroutine != null && movingIndex < movingDuration && movingIndex > 0) {
            movingCoroutine =  StartCoroutine(Moving(pos));
            return;
        }
        movingIndex = 0;
        startMovePos = rt.localPosition;
        movingCoroutine =  StartCoroutine(Moving(pos));
    }
    private IEnumerator Moving(Vector3 pos) {
        while( movingIndex < movingDuration) {
            movingIndex += Time.unscaledDeltaTime;
            rt.localPosition = Vector3.LerpUnclamped(startMovePos, pos, scaleAnimationCurve.Evaluate(movingIndex/ movingDuration));
            yield return new WaitForEndOfFrame();
        }
        movingIndex = 1;
        rt.localPosition = pos;
    }
}

[System.Serializable]
public class Size {
    public float width;
    public float height;
}