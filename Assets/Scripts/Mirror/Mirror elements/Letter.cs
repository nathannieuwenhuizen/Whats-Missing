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
        if (!canBeClicked || BUTTON_DRAGGED) return;
        if (hoverCoroutine != null) StopCoroutine(hoverCoroutine);
        hoverCoroutine = StartCoroutine(ScaleAnimation(hoverScale));
    }
    public override void OnUnhover() {
        base.OnUnhover();
        if (!canBeClicked || BUTTON_DRAGGED) return;
        if (hoverCoroutine != null) StopCoroutine(hoverCoroutine);
        hoverCoroutine = StartCoroutine(ScaleAnimation(normalScale));
    }


    public void Select()
    {
        OnUnhover();
        selected = true;
        if (spawnPosition == Vector3.zero) spawnPosition = rt.localPosition;
    }

    public void Deselect()
    {
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
            preClickSelected = true;
            mirrorCanvas.RemoveSelectedLetter(mirrorCanvas.selectedLetterObjects.IndexOf(this));
            StopAllCoroutines();
        } else {
            preClickSelected = false;
        }

        StartCoroutine(Dragging());
    }

    void LetterIsClicked()
    {
        if (!canBeClicked || !pressed) return;
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
        Canvas canvas = MirrorCanvas.GetComponent<Canvas>();
        while(pressed) {
            yield return new WaitForEndOfFrame();

            transform.position = GetCanvasPos(canvas);
            transform.localPosition = new Vector3(
                transform.localPosition.x,
                transform.localPosition.y,
                transform.localPosition.z - 10f
                );
            mirrorCanvas.UpdateLetterDrag(this);

            MirrorButton.SELECTED_BUTTON = this;

            //out of mirror view or click out
            if (mirrorCanvas.IsInteractable == false || !Input.GetButton("Fire1")) {
                Debug.Log("button up!");
                LetterIsClicked();
            }
        }
        MirrorButton.BUTTON_DRAGGED = false;

    }

    private Vector3 GetCanvasPos(Canvas m_Canvas) {
        Plane m_CanvasPlane = new Plane();
        m_CanvasPlane.Set3Points (
            m_Canvas.transform.TransformPoint (new Vector3 (0, 0)), 
            m_Canvas.transform.TransformPoint (new Vector3 (0, 1)),
            m_Canvas.transform.TransformPoint (new Vector3 (1, 0))
        );
        // Raycast from the camera to the plane, to get the screen position on the canvas
        Ray ray = Camera.main.ScreenPointToRay (new Vector3(Screen.width * .5f, Screen.height * .5f, 0));
        Vector3 worldPosOnCanvas = Vector3.zero;
        float rayHitDistance= 20f;
        if (m_CanvasPlane.Raycast (ray, out rayHitDistance)) {
            //RESULT: Here is what you what (in world space coordinate)
            worldPosOnCanvas = ray.GetPoint (rayHitDistance * 0.9f);
        }
        return worldPosOnCanvas;
    }

    public Vector3 Position {
        get { return transform.localPosition; }
    }

    public void MoveTo( Vector3 pos) {
        if (movingCoroutine != null && movingIndex < 1 && movingIndex > 0) {
            StopCoroutine(movingCoroutine);
            movingCoroutine =  StartCoroutine(Moving(pos));
            return;
        }
        movingIndex = 0;
        startMovePos = rt.localPosition;
        movingCoroutine =  StartCoroutine(Moving(pos));
    }
    private IEnumerator Moving(Vector3 pos, float duration = .5f, float delay = 0) {
        yield return new WaitForSeconds(delay);
        while( movingIndex < duration) {
            movingIndex += Time.unscaledDeltaTime;
            rt.localPosition = Vector3.LerpUnclamped(startMovePos, pos, scaleAnimation.Evaluate(movingIndex/ duration));
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