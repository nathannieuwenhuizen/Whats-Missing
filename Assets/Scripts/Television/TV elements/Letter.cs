using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Letter : TelevisionButton
{
    
    private Button button;
    private Text text;
    private RectTransform rt;

    private Coroutine movingCoroutine;
    private float movingIndex;
    private Vector3 startMovePos;

    private Coroutine hoverCoroutine;
    private Vector3 normalScale = Vector3.one;
    private Vector3 hoverScale = new Vector3(1.3f,1.3f,1.3f);
    
    [SerializeField]
    private AnimationCurve slideAnimation;

    private Vector3 spawnPosition;

    private string letterValue;

    // sets the value of the letter
    public string LetterValue {
        get {return letterValue;}
        set {
            letterValue = value;
            text.text = value;
            rt.sizeDelta = new Vector2(size.width, size.height);
        }
    }

    public delegate void LetterClickedEvent(Letter letter);
    public event LetterClickedEvent onLetterClick;

    // Start is called before the first frame update
    void Awake()
    {
        button = GetComponent<Button>();
        rt = GetComponent<RectTransform>();
        button.onClick.AddListener(() => LetterIsClicked());
        text = GetComponent<Text>();
    }

    private void OnEnable() {
        rt.localScale = Vector3.zero;
        StartCoroutine(ScaleAnimation(normalScale, 1f));
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

    public void Selected()
    {
        OnUnhover();
        canBeClicked = false;
        spawnPosition = rt.localPosition;
    }

    public void Deselected()
    {
        canBeClicked = true;
        MoveTo(spawnPosition);
    }

    public Size size {
        get {
            TextGenerator textGen = new TextGenerator();
            TextGenerationSettings generationSettings = text.GetGenerationSettings(text.rectTransform.rect.size); 
            float width = textGen.GetPreferredWidth(letterValue, generationSettings);
            float height = textGen.GetPreferredHeight(letterValue, generationSettings);
            return new Size() {width = width, height = height};
        }
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

    private IEnumerator ScaleAnimation(Vector3 endScale, float duration = .2f) {
        float index = 0;
        Vector3 begin = rt.localScale;
        while( index < duration) {
            index += Time.unscaledDeltaTime;
            rt.localScale = Vector3.LerpUnclamped(begin, endScale, slideAnimation.Evaluate(index/ duration));
            yield return new WaitForEndOfFrame();
        }
        rt.localScale = endScale;
    }

}

[System.Serializable]
public class Size {
    public float width;
    public float height;
}