using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
public class Letter : TelevisionButton
{
    
    private Button button;

    private Coroutine movingCoroutine;
    private float movingIndex;
    private Vector3 startMovePos;
    [SerializeField]
    private TMP_Text text; 

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
            //rt.sizeDelta = new Vector2(size.width, size.height);
        }
    }

    public delegate void LetterClickedEvent(Letter letter);
    public event LetterClickedEvent onLetterClick;

    public override void Awake()
    {
        base.Awake();
        button = GetComponent<Button>();
        button.onClick.AddListener(() => LetterIsClicked());
        // text = GetComponent<TMP_Text>();
    }


    public override void OnHover() {
        base.OnHover();
        Debug.Log("hover: " + LetterValue);
        if (!canBeClicked) return;
        if (hoverCoroutine != null) StopCoroutine(hoverCoroutine);
        hoverCoroutine = StartCoroutine(ScaleAnimation(hoverScale));
    }
    public override void OnUnhover() {
        base.OnUnhover();
        Debug.Log("unhover: " + LetterValue);
        if (!canBeClicked) return;
        if (hoverCoroutine != null) StopCoroutine(hoverCoroutine);
        hoverCoroutine = StartCoroutine(ScaleAnimation(normalScale));
    }


    public void Selected()
    {
        OnUnhover();
        button.interactable = false;
        canBeClicked = false;
        spawnPosition = rt.localPosition;
    }

    public void Deselected()
    {
        canBeClicked = true;
        button.interactable = true;
        MoveTo(spawnPosition);
    }

    public Size size {
        get {
            return new Size() {width = text.GetRenderedValues(true).x, height = text.GetRenderedValues(true).y};

            // TextGenerator textGen = new TextGenerator();
            // TextGenerationSettings generationSettings = text.GetGenerationSettings(text.rectTransform.rect.size); 
            // float width = textGen.GetPreferredWidth(letterValue, generationSettings);
            // float height = textGen.GetPreferredHeight(letterValue, generationSettings);
            // return new Size() {width = width, height = height};
        }
    }

    public Color Color {
        get => text.color;
        set => text.color = value;
    }

    void LetterIsClicked()
    {
        if (!canBeClicked) return;
        onLetterClick?.Invoke(this);
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