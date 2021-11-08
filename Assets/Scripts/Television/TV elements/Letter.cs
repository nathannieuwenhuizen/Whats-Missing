using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
public class Letter : TelevisionButton
{
    public delegate void LetterClickedEvent(Letter letter);
    public event LetterClickedEvent onLetterClick;
    public static event LetterClickedEvent OnLetterClickAction;
    
    private Button button;

    private bool selected = false;
    public bool Selected { get => selected; }

    private Coroutine movingCoroutine;
    private float movingIndex;
    private Vector3 startMovePos;
    [SerializeField]
    private TMP_Text text; 
    [SerializeField]
    private ParticleSystem clickParticle;

    private Coroutine hoverCoroutine;
    private Vector3 hoverScale = new Vector3(1.3f,1.3f,1.3f);
    

    private Vector3 spawnPosition;

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
        if (!canBeClicked) return;
        if (hoverCoroutine != null) StopCoroutine(hoverCoroutine);
        hoverCoroutine = StartCoroutine(ScaleAnimation(hoverScale));
    }
    public override void OnUnhover() {
        base.OnUnhover();
        if (!canBeClicked) return;
        if (hoverCoroutine != null) StopCoroutine(hoverCoroutine);
        hoverCoroutine = StartCoroutine(ScaleAnimation(normalScale));
    }


    public void Select()
    {
        OnUnhover();
        selected = true;
        spawnPosition = rt.localPosition;
    }

    public void Deselect()
    {
        selected = false;
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

    void LetterIsClicked()
    {
        if (!canBeClicked) return;
        
        OnLetterClickAction?.Invoke(this);
        onLetterClick?.Invoke(this);
        clickParticle.Emit(20);

    }

    public void MoveTo( Vector3 pos) {
        if (movingCoroutine != null && movingIndex < 1) {
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
            rt.localPosition = Vector3.LerpUnclamped(startMovePos, pos, slideAnimation.Evaluate(movingIndex/ duration));
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