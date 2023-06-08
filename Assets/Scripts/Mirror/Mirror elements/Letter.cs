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

    public static readonly Color CorrectColor = new Color(.2f,1,.2f, .4f);
    public static readonly Color IncorrectColor = new Color(1,.2f,.2f, .5f);
    [HideInInspector]
    public Color DefaultGlowColor;

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

    private Vector3 oldDragPos;

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

    public bool Visible {
        set { 
            Color temp = Color;
            temp.a = value ? 1 : 0;
            Color = temp;
         }
    }


    public override void Awake()
    {
        base.Awake();
        button = GetComponent<Button>();
        button.onClick.AddListener(() => LetterIsClicked());
        DefaultGlowColor = text.fontMaterial.GetColor("_GlowColor");
        DefaultGlowColor.a = .5f;

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
        GlowColor = DefaultGlowColor;
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

    public Color GlowColor {
        get => text.fontMaterial.GetColor("_GlowColor");
        set => text.fontMaterial.SetColor("_GlowColor", value);
    }

    private Color defaultColor = Color.white;
    public Color DefaultColor {
        get { return defaultColor;}
        set { 
            defaultColor = value; 
            Color = value;
        }
    }


    private Coroutine colorCoroutine;
    ///<summary>
    /// Animates the color
    ///</summary>
    public void AnimateGlowColor (float _duration, Color _endColor) {
        if (colorCoroutine != null) StopCoroutine(colorCoroutine);
        colorCoroutine = StartCoroutine(AnimatingGlowColor(_duration, _endColor));
    }

    private IEnumerator AnimatingGlowColor(float _duration, Color _end) {
        float index = 0;
        Color begin = GlowColor;
         while (index < _duration) {
             yield return new WaitForEndOfFrame();
             index += Time.unscaledDeltaTime;
             GlowColor = Color.LerpUnclamped(begin, _end, index / _duration);
         }
        GlowColor = _end;
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
            mirrorCanvas.RemoveSelectedLetter(mirrorCanvas.SelectedLetters.IndexOf(this));
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

    #region  dragging
    public bool Dragged() {
        return (pressedTime != 0 &&  Time.time - pressedTime > pressedTimeBeforeDrag);
    }

    private IEnumerator Dragging() {
        MirrorButton.BUTTON_DRAGGED = true;
        Canvas canvas = MirrorCanvas.Canvas;
        while(pressed) {

            transform.position = Vector3.Lerp(transform.position, canvas.MouseToWorldPosition(), Time.deltaTime * 10f);
            Zdistance = -10;
            mirrorCanvas.UpdateLetterDrag(this);
            UpdateDragRotation();
            oldDragPos = Position;

            MirrorButton.SELECTED_BUTTON = this;

            //out of mirror view or click out
            if (mirrorCanvas.IsFocused == false || !Extensions.IsPressed(ControllerRebinds.controls.Player.Click)) {
                LetterIsClicked();
            }
            yield return new WaitForEndOfFrame();
        }
        transform.localRotation = Quaternion.Euler(0,0,0);
        StartCoroutine(PlaceAnimationRotation());
        MirrorButton.BUTTON_DRAGGED = false;

    }
    private void UpdateDragRotation() {
        Vector3 delta = Position - oldDragPos;
        float angle = 45f;
        delta *= 2f;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler( -Mathf.Clamp( delta.y, -angle, angle), Mathf.Clamp( delta.x, -angle, angle), 0), Time.deltaTime * 10f);
        Debug.Log("drag delta " + delta);
    }
    #endregion

    public Vector3 Position {
        get { return transform.localPosition; }
        set { transform.localPosition = value; }
    }
    
    public float  Zdistance {
        get { return Position.z;}
        set { 
            Position = new Vector3(
                transform.localPosition.x,
                transform.localPosition.y,
                value
            );
         }
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
        while( movingIndex < movingDuration && mirrorCanvas.Mirror.InSpace) {
            movingIndex += Time.unscaledDeltaTime;
            rt.localPosition = Vector3.LerpUnclamped(startMovePos, pos, scaleAnimationCurve.Evaluate(movingIndex/ movingDuration));
            yield return new WaitForEndOfFrame();
        }
        movingIndex = 1;
        rt.localPosition = pos;
    }

    public IEnumerator ZDistanceBounce( float _duration, float _zDistance  = -10, float _delay = 0) {
        yield return new WaitForSeconds(_delay);

        float index = 0; 
        while (index < _duration) {
            index += Time.deltaTime;
            Zdistance = Mathf.Sin(Mathf.PI * (index / _duration)) * _zDistance;
            yield return new WaitForEndOfFrame();
        }
        Zdistance = 0;
    }
    public IEnumerator PlaceAnimationRotation( ) {
        float index = 0; 
        float duration = .3f;
        while (index < duration && Zdistance > 0) {
            index += Time.deltaTime;
            transform.localRotation = Quaternion.Euler( Mathf.Sin(Mathf.PI * (.5f + .5f *(index / duration))) * -20f, 0, 0);
            yield return new WaitForEndOfFrame();
        }
        transform.localRotation = Quaternion.Euler(0,0,0);
    }
 
}

[System.Serializable]
public class Size {
    public float width;
    public float height;
}